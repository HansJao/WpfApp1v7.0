﻿using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.Enumeration;
using WpfApp1.DataClass.ExcelDataClass;
using WpfApp1.DataClass.StoreSearch;
using WpfApp1.Utility;

namespace WpfApp1.Modules.ExcelModule.Implement
{
    public class ExcelModule : IExcelModule
    {

        /// <summary>
        /// 取得Excel每日出貨清單
        /// </summary>
        /// <param name="shippedDate"></param>
        /// <returns></returns>
        public List<StoreSearchData<StoreSearchColorDetail>> GetExcelDailyShippedList(DateTime? shippedDate)
        {
            IWorkbook workbook = null;  //新建IWorkbook對象  
            string fileName = string.Concat(AppSettingConfig.FilePath(), "/", AppSettingConfig.StoreManageFileName());
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            workbook = new XSSFWorkbook(fileStream);  //xlsx數據讀入workbook
            var list = new List<StoreSearchData<StoreSearchColorDetail>>();
            var selectedDateTime = shippedDate == null ? DateTime.Now.ToString("M/d") : shippedDate?.ToString("M/d");
            var currentDateValue = "出貨" + selectedDateTime;
            for (int sheetCount = 1; sheetCount < workbook.NumberOfSheets; sheetCount++)
            {
                ISheet sheet = workbook.GetSheetAt(sheetCount);  //獲取第i個工作表  
                IRow row;
                var currentDateCellIndex = -1;
                var firstRow = sheet.GetRow(0);
                for (int columnIndex = 5; columnIndex < 14; columnIndex++)
                {
                    var cell = firstRow.GetCell(columnIndex);
                    var cellValue = cell.StringCellValue;
                    if (cellValue == currentDateValue)
                    {
                        currentDateCellIndex = columnIndex;
                        break;
                    }
                }

                var colorList = new List<StoreData>();
                for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++)  //對工作表每一行  
                {
                    if (rowIndex > 120)
                        break;
                    row = sheet.GetRow(rowIndex);   //row讀入第i行數據  

                    if (row != null)
                    {
                        if (row.GetCell(ExcelEnum.ExcelInventoryColumnIndexEnum.ColorName.ToInt()) == null)
                        {
                            break;
                        }
                        if (row.GetCell(currentDateCellIndex) != null && row.GetCell(currentDateCellIndex).CellType != CellType.String && row.GetCell(currentDateCellIndex).NumericCellValue != 0)
                        {
                            var countInventory = row.GetCell(ExcelEnum.ExcelInventoryColumnIndexEnum.CountInventory.ToInt());
                            if (countInventory == null || string.IsNullOrEmpty(countInventory.ToString()) || (countInventory.CellType == CellType.Formula && countInventory.CachedFormulaResultType == CellType.Error))
                            {
                                continue;
                            }
                            var cellValue = countInventory.NumericCellValue; //獲取i行j列數據
                            //清單裡若沒有該布種則加入
                            if (list.Where(w => w.TextileName == sheet.SheetName).Count() == 0)
                            {
                                list.Add(new StoreSearchData<StoreSearchColorDetail>
                                {
                                    TextileName = sheet.SheetName,
                                    StoreSearchColorDetails = new List<StoreSearchColorDetail>()
                                });
                            }

                            var currentTextile = list.Where(w => w.TextileName == sheet.SheetName).First();
                            currentTextile.StoreSearchColorDetails.Add(new StoreSearchColorDetail
                            {
                                ColorName = row.GetCell(ExcelEnum.ExcelInventoryColumnIndexEnum.ColorName.ToInt()) == null ? "" : row.GetCell(ExcelEnum.ExcelInventoryColumnIndexEnum.ColorName.ToInt()).ToString(),
                                FabricFactory = row.GetCell(ExcelEnum.ExcelInventoryColumnIndexEnum.FabricFactory.ToInt()) == null ? "" : row.GetCell(ExcelEnum.ExcelInventoryColumnIndexEnum.FabricFactory.ToInt()).ToString(),
                                ClearFactory = row.GetCell(ExcelEnum.ExcelInventoryColumnIndexEnum.ClearFactory.ToInt()) == null ? "" : row.GetCell(ExcelEnum.ExcelInventoryColumnIndexEnum.ClearFactory.ToInt()).ToString(),
                                ShippedCount = Convert.ToInt32(row.GetCell(currentDateCellIndex).NumericCellValue),
                                CountInventory = cellValue.ToString(),
                                CheckDate = row.GetCell(ExcelEnum.ExcelInventoryColumnIndexEnum.CheckDate.ToInt()) == null ? "" : row.GetCell(ExcelEnum.ExcelInventoryColumnIndexEnum.CheckDate.ToInt()).ToString()
                            });
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return list;
        }

        public TextileInventoryHeader GetShippingDate(ISheet sheet)
        {
            TextileInventoryHeader TextileInventoryHeader = new TextileInventoryHeader
            {
                ShippingDate1 = ExcelHelper.CheckExcelCellType<string>(CellType.String, sheet.GetRow(0).GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate1)),
                ShippingDate2 = ExcelHelper.CheckExcelCellType<string>(CellType.String, sheet.GetRow(0).GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate2)),
                ShippingDate3 = ExcelHelper.CheckExcelCellType<string>(CellType.String, sheet.GetRow(0).GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate3)),
                ShippingDate4 = ExcelHelper.CheckExcelCellType<string>(CellType.String, sheet.GetRow(0).GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate4)),
                ShippingDate5 = ExcelHelper.CheckExcelCellType<string>(CellType.String, sheet.GetRow(0).GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate5)),
                ShippingDate6 = ExcelHelper.CheckExcelCellType<string>(CellType.String, sheet.GetRow(0).GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate6)),
                ShippingDate7 = ExcelHelper.CheckExcelCellType<string>(CellType.String, sheet.GetRow(0).GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate7)),
                ShippingDate8 = ExcelHelper.CheckExcelCellType<string>(CellType.String, sheet.GetRow(0).GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate8)),
                ShippingDate9 = ExcelHelper.CheckExcelCellType<string>(CellType.String, sheet.GetRow(0).GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate9))
            };
            return TextileInventoryHeader;
        }

        public Tuple<List<string>, IWorkbook> GetExcelWorkbook(string fileNamePath)
        {
            //string fileNamePath = string.Concat(AppSettingConfig.InventoryHistoryRecordFilePath(), "/", fileName);
            FileStream fileStream = new FileStream(fileNamePath, FileMode.Open, FileAccess.Read);
            IWorkbook Workbook = new XSSFWorkbook(fileStream);  //xlsx數據讀入workbook
            List<string> textileList = new List<string>();
            for (int sheetCount = 1; sheetCount < Workbook.NumberOfSheets; sheetCount++)
            {
                ISheet sheet = Workbook.GetSheetAt(sheetCount);  //獲取第i個工作表  
                textileList.Add(sheet.SheetName);
            }
            GetShippingDate(Workbook.GetSheetAt(1));
            return Tuple.Create(textileList, Workbook);
        }
    }
}
