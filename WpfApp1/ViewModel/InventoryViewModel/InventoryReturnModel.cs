using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.IO;
using System.Windows;
using WpfApp1.Utility;
using static WpfApp1.DataClass.Enumeration.ExcelEnum;

namespace WpfApp1.ViewModel.InventoryViewModel
{
    public class InventoryReturnModel
    {
        public InventoryReturnModel()
        {
            string fileName = string.Concat(AppSettingConfig.FilePath(), "\\庫存管理", ".xlsx");
            IWorkbook workbook = null;  //新建IWorkbook對象  

            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                workbook = new XSSFWorkbook(fs);
            }

            for (int sheetCount = 1; sheetCount < workbook.NumberOfSheets; sheetCount++)
            {
                ISheet sheet = workbook.GetSheetAt(sheetCount);  //獲取第i個工作表  
                sheet.ForceFormulaRecalculation = true;

                for (int rowCount = 1; rowCount <= sheet.LastRowNum; rowCount++)
                {
                    IRow row = sheet.GetRow(rowCount);
                    //確認該列是否有值
                    if (row == null) break;

                    ICell inventoryCell = row.GetCell(ExcelInventoryColumnIndexEnum.Inventory.ToInt());
                    //確認布種顏色是否有值
                    if (inventoryCell == null) break;
                    ICell inventoryDifferentCell = row.GetCell(ExcelInventoryColumnIndexEnum.DifferentCylinder.ToInt());
                    //確認布種顏色是否為數值，且不同缸的值如為null或是有數值，則可繼續運算
                    try
                    {
                        if (inventoryCell.CellType == CellType.Numeric && (inventoryDifferentCell == null || inventoryDifferentCell.CellType == CellType.Numeric || inventoryDifferentCell.CellType == CellType.Blank))
                        {
                            ICell countInventoryCell = row.GetCell(ExcelInventoryColumnIndexEnum.CountInventory.ToInt());
                            //檢查計算庫存量是否和原庫存量是否相同
                            double inventory = inventoryCell.NumericCellValue;
                            double inventoryDifferent = inventoryDifferentCell != null ? inventoryDifferentCell.NumericCellValue : 0;
                            double countInventory = countInventoryCell.NumericCellValue;
                            double totalInventory = inventory + inventoryDifferent;
                            if (totalInventory != countInventory)
                            {
                                double shippingDate1 = GetExcelNumericCellValue(row, ExcelInventoryColumnIndexEnum.ShippingDate1);
                                double shippingDate2 = GetExcelNumericCellValue(row, ExcelInventoryColumnIndexEnum.ShippingDate2);
                                double shippingDate3 = GetExcelNumericCellValue(row, ExcelInventoryColumnIndexEnum.ShippingDate3);
                                double shippingDate4 = GetExcelNumericCellValue(row, ExcelInventoryColumnIndexEnum.ShippingDate4);
                                double shippingDate5 = GetExcelNumericCellValue(row, ExcelInventoryColumnIndexEnum.ShippingDate5);
                                double shippingDate6 = GetExcelNumericCellValue(row, ExcelInventoryColumnIndexEnum.ShippingDate6);
                                double shippingDate7 = GetExcelNumericCellValue(row, ExcelInventoryColumnIndexEnum.ShippingDate7);
                                double shippingDate8 = GetExcelNumericCellValue(row, ExcelInventoryColumnIndexEnum.ShippingDate8);
                                double shippingDate9 = GetExcelNumericCellValue(row, ExcelInventoryColumnIndexEnum.ShippingDate9);
                                double totalShipping = shippingDate1 + shippingDate2 + shippingDate3 + shippingDate4 + shippingDate5 + shippingDate6
                                                       + shippingDate7 + shippingDate8 + shippingDate9;
                                double finalCountInventory = totalInventory - totalShipping;


                                //如果不同缸為0，則直接修改庫存量
                                if (inventoryDifferent == 0)
                                {
                                    inventoryCell.SetCellValue(finalCountInventory);
                                }
                                //否則判斷數量是否有出完一缸
                                else
                                {
                                    if (inventory > totalShipping)
                                    {
                                        inventoryCell.SetCellValue(inventory - totalShipping);
                                    }
                                    else
                                    {
                                        inventoryDifferentCell.SetCellType(CellType.Blank);
                                        inventoryCell.SetCellValue(finalCountInventory);
                                    }
                                }
                                if (shippingDate1 != 0) row.GetCell(ExcelInventoryColumnIndexEnum.ShippingDate1.ToInt()).SetCellType(CellType.Blank);
                                if (shippingDate2 != 0) row.GetCell(ExcelInventoryColumnIndexEnum.ShippingDate2.ToInt()).SetCellType(CellType.Blank);
                                if (shippingDate3 != 0) row.GetCell(ExcelInventoryColumnIndexEnum.ShippingDate3.ToInt()).SetCellType(CellType.Blank);
                                if (shippingDate4 != 0) row.GetCell(ExcelInventoryColumnIndexEnum.ShippingDate4.ToInt()).SetCellType(CellType.Blank);
                                if (shippingDate5 != 0) row.GetCell(ExcelInventoryColumnIndexEnum.ShippingDate5.ToInt()).SetCellType(CellType.Blank);
                                if (shippingDate6 != 0) row.GetCell(ExcelInventoryColumnIndexEnum.ShippingDate6.ToInt()).SetCellType(CellType.Blank);
                                if (shippingDate7 != 0) row.GetCell(ExcelInventoryColumnIndexEnum.ShippingDate7.ToInt()).SetCellType(CellType.Blank);
                                if (shippingDate8 != 0) row.GetCell(ExcelInventoryColumnIndexEnum.ShippingDate8.ToInt()).SetCellType(CellType.Blank);
                                if (shippingDate9 != 0) row.GetCell(ExcelInventoryColumnIndexEnum.ShippingDate9.ToInt()).SetCellType(CellType.Blank);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string errorMessage = string.Concat("----錯誤來源----\n", "布種名稱：" + sheet.SheetName, "\n資料行數：", rowCount, "\n" + ex.StackTrace);
                        MessageBox.Show(errorMessage);
                        throw ex;
                    }
                }
            }

            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(fs);
            }
        }

        public double GetExcelNumericCellValue(IRow row, ExcelInventoryColumnIndexEnum excelInventoryColumnIndexEnum)
        {
            ICell cell = row.GetCell(excelInventoryColumnIndexEnum.ToInt());
            return cell != null ? cell.NumericCellValue : 0;
        }
    }


}
