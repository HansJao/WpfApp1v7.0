using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.Fabric;
using WpfApp1.DataClass.Shipping;
using WpfApp1.Utility;

namespace WpfApp1.ViewModel.InventoryViewModel
{
    public class DeductInventoryViewModel : ViewModelBase
    {

        public DateTime ShippingDate { get; set; } = DateTime.Now;
        public DeductInventoryViewModel()
        {
            DirectoryInfo d = new DirectoryInfo(AppSettingConfig.FilePath()); //Assuming Test is your Folder

            IEnumerable<FileInfo> fileInfos = d.GetFiles("*.xlsx").Where(w => w.Name.Contains(string.Concat("出貨測試", ShippingDate.ToString("yyyyMMdd"), "-")));
            List<ShippingSheetStructure> ShippingSheetStructures = new List<ShippingSheetStructure>();

            ExternalDataHelper externalDataHelper = new ExternalDataHelper();
            IEnumerable<TextileNameMapping> textileNameMappings = externalDataHelper.GetTextileNameMappings();

            foreach (FileInfo fileInfo in fileInfos)
            {
                IWorkbook shippingWorkbook = null;
                using (FileStream fs = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read))
                {
                    shippingWorkbook = new XSSFWorkbook(fs);
                }
                for (int sheetCount = 1; sheetCount < shippingWorkbook.NumberOfSheets; sheetCount++)
                {
                    ISheet sheet = shippingWorkbook.GetSheetAt(sheetCount);
                    ShippingSheetStructure shippingSheetStructure = new ShippingSheetStructure
                    {
                        Customer = sheet.SheetName,
                        TextileShippingDatas = new List<TextileShippingData>()
                    };
                    for (int rowCount = 6; rowCount <= sheet.LastRowNum; rowCount++)
                    {
                        IRow row = sheet.GetRow(rowCount);
                        string textileName = row.GetCell(1).StringCellValue;
                        if (textileName.Contains("配件"))
                        {
                            textileName = textileNameMappings.ToList().Find(f => f.Inventory.Contains(textileName)).Inventory.ToList().Find(f => f.Contains("碼布")) ?? textileName;
                        }
                        string colorFullName = row.GetCell(4).StringCellValue;
                        //判斷顏色是否含有數量數字
                        string colorName = colorFullName.Split('-').Count() == 2 ? colorFullName.Split('*')[0] : colorFullName.Split('-')[0];
                        int quantity = colorFullName.Split('-').Count() == 2 ? 1 : colorFullName.Split('-')[1].Split('*')[0].ToInt();
                        int totalQuantity = CountTotalQuantity(sheet, rowCount);
                        //判斷有幾行，然後直接跳行
                        rowCount = (decimal)totalQuantity / 7 > 1 ? rowCount + (totalQuantity / 7) : rowCount;

                        //判斷此客戶是否已有加入布種，或是前後布種名稱不一樣時，則加入新的布種, 如布種名稱為空格，則為上一個布種
                        if (shippingSheetStructure.TextileShippingDatas.Count == 0 || (shippingSheetStructure.TextileShippingDatas.Last().TextileName != textileName && textileName != string.Empty))
                        {
                            shippingSheetStructure.TextileShippingDatas.Add(new TextileShippingData()
                            {
                                TextileName = textileName,
                                ShippingSheetDatas = new List<ShippingSheetData>()
                            });
                        }

                        shippingSheetStructure.TextileShippingDatas.Last().ShippingSheetDatas.Add(new ShippingSheetData()
                        {
                            ColorName = colorName,
                            //計算出貨總數
                            ShippingNumber = totalQuantity,
                            //原出貨數
                            CountInventory = quantity,
                        });
                    }
                    ShippingSheetStructures.Add(shippingSheetStructure);
                }
            }

            string InventoryfileName = string.Concat(AppSettingConfig.FilePath(), "\\庫存管理", ".xlsx");
            IWorkbook inventoryWorkbook = null;

            using (FileStream fs = new FileStream(InventoryfileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                inventoryWorkbook = new XSSFWorkbook(fs);
            }

            //取得每一個客戶的出貨清單
            foreach (ShippingSheetStructure structure in ShippingSheetStructures)
            {
                //取得客戶出貨的布種
                foreach (TextileShippingData textileShippingData in structure.TextileShippingDatas)
                {
                    //取得庫存布種的活頁薄
                    ISheet sheet = inventoryWorkbook.GetSheet(textileShippingData.TextileName);

                    //取得客戶的出貨布種顏色與數量
                    foreach (ShippingSheetData shippingSheetData in textileShippingData.ShippingSheetDatas)
                    {
                        for (int rowCount = 1; rowCount < sheet.LastRowNum; rowCount++)
                        {
                            IRow row = sheet.GetRow(rowCount);
                            if (row == null) break;
                            ICell cell = row.GetCell(1);
                            if (cell.StringCellValue.Split('-')[0] == shippingSheetData.ColorName)
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }


        private int CountTotalQuantity(ISheet sheet, int rowCount)
        {
            IRow row = sheet.GetRow(rowCount);
            int quantityCheck = 0;
            //數量確認
            for (int quantityCount = 6; quantityCount <= 12; quantityCount++)
            {
                if (row.GetCell(quantityCount) != null && row.GetCell(quantityCount).CellType == CellType.Numeric)
                    quantityCheck++;
                else
                    break;
            }
            IRow rowNext = sheet.GetRow(rowCount + 1);
            if (rowNext == null)
                return quantityCheck;

            ICell cellNext = rowNext.GetCell(4);

            if (cellNext == null)
                return quantityCheck;

            string colorFullNameNext = cellNext.StringCellValue;

            if (quantityCheck % 7 == 0 && colorFullNameNext == string.Empty)
            {
                rowCount++;
                return quantityCheck + CountTotalQuantity(sheet, rowCount);
            }
            else
            {
                return quantityCheck;
            }
        }
    }
}
