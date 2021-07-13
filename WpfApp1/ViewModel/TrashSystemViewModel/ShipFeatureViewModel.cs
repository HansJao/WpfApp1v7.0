using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.Fabric;
using WpfApp1.DataClass.TrashSystem;
using WpfApp1.Modules.TrashModule;
using WpfApp1.Modules.TrashModule.Implement;
using WpfApp1.Utility;

namespace WpfApp1.ViewModel.TrashSystemViewModel
{
    public class ShipFeatureViewModel
    {
        protected ITrashModule TrashModule { get; } = new TrashModule();

        public ShipFeatureViewModel(string shipFeatureDate)
        {
            string defaultDate = DateTime.Now.ToString("yyyyMMdd");

            string[] dateNumber = shipFeatureDate.Split('-');

            string fileNameDate = string.Concat(AppSettingConfig.FilePath(), "\\出貨單", dateNumber[0].Length > 5 ? dateNumber[0] : defaultDate, "-", dateNumber[0].Length > 5 ? dateNumber[1] : dateNumber[0], ".xlsx");
            //IEnumerable<string> shipFileName = Directory.GetFiles(AppSettingConfig.FilePath(), fileNameDate).Select(System.IO.Path.GetFileName).OrderByDescending(o => o);
            IWorkbook workbook = null;  //新建IWorkbook對象  
            using (FileStream fs = new FileStream(fileNameDate, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(fs);
            }

            IEnumerable<TrashItem> trashItems = TrashModule.GetTrashItems().Where(w => w.I_03 != null).OrderBy(o => o.I_01);

            ExternalDataHelper externalDataHelper = new ExternalDataHelper();
            IEnumerable<TextileNameMapping> textileNameMappings = externalDataHelper.GetTextileNameMappings();

            for (int sheetCount = 0; sheetCount < workbook.NumberOfSheets; sheetCount++)
            {
                ISheet sheet = workbook.GetSheetAt(sheetCount);  //獲取第i個工作表  
                string textileName = null;
                sheet.SetColumnWidth(4, 12200);

                ICell customerCell = sheet.GetRow(4).GetCell(2);
                IEnumerable<TrashCustomer> trashCustomers = TrashModule.GetCustomerList().Where(w => customerCell.StringCellValue.Contains(w.C_NAME));
                if (trashCustomers != null && trashCustomers.FirstOrDefault() != null)
                    customerCell.SetCellValue(string.Concat(customerCell.StringCellValue, "-", trashCustomers.FirstOrDefault().CARD_NO, trashCustomers.FirstOrDefault().C_NAME));

                for (int rowCount = 6; rowCount <= 18; rowCount++)
                {

                    IRow row = sheet.GetRow(rowCount);
                    if (row == null) break;
                    ICell textileNameCell = row.GetCell(1);
                    if (textileNameCell != null && textileNameCell.StringCellValue != null && textileNameCell.StringCellValue != "")
                    {
                        textileName = textileNameCell.StringCellValue;
                    }

                    ICell colorCell = row.GetCell(4);
                    string colorCellValue = colorCell.CellType == CellType.String ? colorCell.StringCellValue : colorCell.NumericCellValue.ToString();
                    if (colorCell == null || colorCellValue == null || colorCellValue == "")
                    {
                        break;
                    }

                    string textileColor = colorCellValue.Split('-')[0];
                    int textileColorNumber = colorCellValue.Split('-').Length >= 2 ? colorCellValue.Split('-')[1].ToInt() : 0;
                    if (textileColorNumber / 7 >= 1)
                    {
                        rowCount = rowCount + textileColorNumber / 7;
                    }
                    TrashItem trashItem = externalDataHelper.GetTrashItemFromInventoryMapping(trashItems, textileName, textileColor, textileNameMappings);
                    string accountFactoryID = string.Empty;
                    string accountTextileID = string.Empty;
                    string accountTextileName = string.Empty;
                    if (trashItem == null)
                    {
                    }
                    else
                    {
                        accountFactoryID = trashItem.F_01;
                        accountTextileID = trashItem.I_01;
                        accountTextileName = trashItem.I_03;
                    }
                    string shipFeatureString = colorCellValue + "*" + accountFactoryID + "_" + accountTextileID + "-" + accountTextileName;
                    colorCell.SetCellValue(shipFeatureString);
                    IFont font = workbook.CreateFont();
                    font.Color = HSSFColor.Blue.Index2;
                    font.FontName = "新細明體";
                    font.IsBold = true;
                    colorCell.RichStringCellValue.ApplyFont(
                                shipFeatureString.Length - 1 - accountTextileID.Length - accountTextileName.Length
                                , shipFeatureString.Length - accountTextileName.Length - 1, font);
                }


            }

            using (FileStream fs = new FileStream(fileNameDate, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(fs);
            }
        }
    }
}
