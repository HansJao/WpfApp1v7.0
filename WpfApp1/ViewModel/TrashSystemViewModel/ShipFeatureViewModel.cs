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
        private IEnumerable<TextileNameMapping> TextileNameMappings { get; set; }

        public ShipFeatureViewModel(string shipFeatureDate)
        {
            string defaultDate = DateTime.Now.ToString("yyyyMMdd");
            string fileNameDate = string.Concat(AppSettingConfig.FilePath(), "\\出貨單", defaultDate, shipFeatureDate, ".xlsx");
            //IEnumerable<string> shipFileName = Directory.GetFiles(AppSettingConfig.FilePath(), fileNameDate).Select(System.IO.Path.GetFileName).OrderByDescending(o => o);
            IWorkbook workbook = null;  //新建IWorkbook對象  
            using (FileStream fs = new FileStream(fileNameDate, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(fs);
            }

            IEnumerable<TrashItem> trashItems = TrashModule.GetTrashItems().Where(w => w.I_03 != null).OrderBy(o => o.I_01);

            ExternalDataHelper externalDataHelper = new ExternalDataHelper();
            TextileNameMappings = externalDataHelper.GetTextileNameMappings();

            for (int sheetCount = 0; sheetCount < workbook.NumberOfSheets; sheetCount++)
            {
                ISheet sheet = workbook.GetSheetAt(sheetCount);  //獲取第i個工作表  
                string textileName = null;
                sheet.SetColumnWidth(4, 12200);
                for (int rowCount = 6; rowCount <= 18; rowCount++)
                {

                    IRow row = sheet.GetRow(rowCount);
                    ICell textileNameCell = row.GetCell(1);
                    if (textileNameCell != null && (textileNameCell.StringCellValue != null | textileNameCell.StringCellValue != ""))
                    {
                        textileName = textileNameCell.StringCellValue;
                    }

                    ICell colorCell = row.GetCell(4);
                    if (colorCell == null | colorCell.StringCellValue == null | colorCell.StringCellValue == "")
                    {
                        break;
                    }

                    string textileColor = colorCell.StringCellValue.Split('-')[0];
                    TextileNameMapping textileNameMapping = TextileNameMappings.ToList().Find(f => f.Inventory.Contains(textileName)) ?? new TextileNameMapping();
                    string accountTextileNameMapping = textileNameMapping.Account == null ? string.Empty : textileNameMapping.Account.FirstOrDefault().Split('*')[0];

                    TrashItem trashItem = new TrashItem();
                    trashItem = trashItems.Where(w => w.I_03 == string.Concat(accountTextileNameMapping, textileColor)).FirstOrDefault();
                    if (accountTextileNameMapping != string.Empty && (trashItem == null || trashItem.I_03 == null))
                        trashItem = trashItems.Where(w => w.I_03.Contains(accountTextileNameMapping) && w.I_03.Contains(textileColor)).FirstOrDefault();
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

                    colorCell.SetCellValue(colorCell.StringCellValue + "*" + accountFactoryID + "_" + accountTextileID + "-" + accountTextileName);
                }


            }

            using (FileStream fs = new FileStream(fileNameDate, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(fs);
            }
        }
    }
}
