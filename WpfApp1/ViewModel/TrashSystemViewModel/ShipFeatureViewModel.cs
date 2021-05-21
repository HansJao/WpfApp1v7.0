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

        public ShipFeatureViewModel()
        {
            string date = DateTime.Now.ToString("yyyyMMdd");
            string fileNameDate = string.Concat(AppSettingConfig.FilePath(), "\\出貨單", date, "-1.xlsx");
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
                for (int rowCount = 6; rowCount <= 18; rowCount++)
                {

                    IRow row = sheet.GetRow(rowCount);
                    ICell textileNameCell = row.GetCell(1);
                    if (textileNameCell != null && (textileNameCell.StringCellValue != null | textileNameCell.StringCellValue != ""))
                    {
                        textileName = textileNameCell.StringCellValue;
                    }

                    ICell colorCell = row.GetCell(4);
                    if (colorCell == null |  colorCell.StringCellValue == null | colorCell.StringCellValue == "")
                    {
                        break;
                    }

                    string textileColor = colorCell.StringCellValue.Split('-')[0];
                    var textileNameMapping = TextileNameMappings.ToList().Find(f => f.Inventory.Contains(textileName));
                    TrashItem trashItem = trashItems.Where(w => w.I_03.Contains(textileName) && w.I_03.Contains(textileColor)).FirstOrDefault();
                }


            }

            using (FileStream fs = new FileStream(fileNameDate, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(fs);
            }
        }
    }
}
