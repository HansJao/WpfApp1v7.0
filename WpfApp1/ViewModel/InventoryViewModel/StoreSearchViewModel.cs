﻿using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfApp1.Command;
using WpfApp1.DataClass.Enumeration;
using WpfApp1.DataClass.ExcelDataClass;
using WpfApp1.DataClass.StoreSearch;
using WpfApp1.Utility;
using static WpfApp1.Pages.StoreSearchPage;

namespace WpfApp1.ViewModel.InventoryViewModel
{
    public class StoreSearchViewModel : ViewModelBase
    {
        //定義一個ICommand型別的參數，他會回傳實作ICommand介面的RelayCommand類別。
        public ICommand InventoryNumberRangeClick { get { return new RelayCommand(InventoryNumberRangeSearchExecute, CanExecute); } }
        public ICommand ExportToExcelClick { get { return new RelayCommand(ExportToExcel, CanExecute); } }
        public ICommand ExportCheckDateToExcelClick { get { return new RelayCommand(ExportCheckDateToExcel, CanExecute); } }


        public StoreSearchViewModel()
        {
            _storeDataList = new ObservableCollection<StoreData>();
            _shippingHistoryStoreDataList = new ObservableCollection<StoreData>();
            MaxNumber = 3;
            MinNumber = 0;
            DateRange = 10;
        }

        private ObservableCollection<StoreData> _storeDataList { get; set; }
        private int _maxNumber { get; set; }
        private int _minNumber { get; set; }
        private int _dateRange { get; set; }

        public ObservableCollection<StoreData> StoreDataList
        {
            get { return _storeDataList; }
            set { _storeDataList = value; }
        }


        public int DateRange
        {
            get { return _dateRange; }
            set
            {
                if (_dateRange != value)
                {
                    _dateRange = value;
                    RaisePropertyChanged("DateRange");
                }
            }
        }

        public int MaxNumber
        {
            get { return _maxNumber; }
            set
            {
                if (_maxNumber != value)
                {
                    _maxNumber = value;
                    RaisePropertyChanged("MaxNumber");
                }
            }
        }
        public int MinNumber
        {
            get { return _minNumber; }
            set
            {
                if (_minNumber != value)
                {
                    _minNumber = value;
                    RaisePropertyChanged("MinNumber");
                }
            }
        }
        private ObservableCollection<StoreData> _shippingHistoryStoreDataList { get; set; }
        public ObservableCollection<StoreData> ShippingHistoryStoreDataList
        {
            get { return _shippingHistoryStoreDataList; }
            set { _shippingHistoryStoreDataList = value; }
        }

        private DateTime _shippingHistoryDate { get; set; } = DateTime.Now;
        public DateTime ShippingHistoryDate
        {
            get { return _shippingHistoryDate; }
            set
            {
                if (_shippingHistoryDate != value)
                {
                    _shippingHistoryDate = value;
                    _shippingHistoryStoreDataList.Clear();
                    CreateStoreSearchListByShipped();
                    RaisePropertyChanged("ShippingHistoryDate");
                }
            }

        }

        public void CreateStoreSearchListByShipped()
        {
            IWorkbook workbook = null;  //新建IWorkbook對象  
            string fileName = string.Concat(AppSettingConfig.FilePath(), "/", AppSettingConfig.StoreManageFileName());
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            workbook = new XSSFWorkbook(fileStream);  //xlsx數據讀入workbook
            var list = new List<StoreSearchData<StoreSearchColorDetail>>();
            var selectedDateTime = ShippingHistoryDate == null ? DateTime.Now.ToString("M/d") : ShippingHistoryDate.ToString("M/d");
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
                list.Add(new StoreSearchData<StoreSearchColorDetail>
                {
                    TextileName = sheet.SheetName,
                    StoreSearchColorDetails = new List<StoreSearchColorDetail>()
                });
                var colorList = new List<StoreData>();
                for (int rowIndex = 1; rowIndex < sheet.LastRowNum; rowIndex++)  //對工作表每一行  
                {
                    if (rowIndex > 100)
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
                            list.Last().StoreSearchColorDetails.Add(new StoreSearchColorDetail
                            {
                                ColorName = row.GetCell(ExcelEnum.ExcelInventoryColumnIndexEnum.ColorName.ToInt()) == null ? "" : row.GetCell(ExcelEnum.ExcelInventoryColumnIndexEnum.ColorName.ToInt()).ToString(),
                                FabricFactory = row.GetCell(ExcelEnum.ExcelInventoryColumnIndexEnum.FabricFactory.ToInt()) == null ? "" : row.GetCell(ExcelEnum.ExcelInventoryColumnIndexEnum.FabricFactory.ToInt()).ToString(),
                                ClearFactory = row.GetCell(ExcelEnum.ExcelInventoryColumnIndexEnum.ClearFactory.ToInt()) == null ? "" : row.GetCell(ExcelEnum.ExcelInventoryColumnIndexEnum.ClearFactory.ToInt()).ToString(),
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
            foreach (var item in list)
            {
                if (item.StoreSearchColorDetails.Count() == 0)
                {
                    continue;
                }
                ShippingHistoryStoreDataList.Add(new StoreData
                {
                    TextileName = item.TextileName,

                });
                foreach (var color in item.StoreSearchColorDetails)
                {
                    ShippingHistoryStoreDataList.Add(new StoreData
                    {
                        ColorName = color.ColorName,
                        FabricFactory = color.FabricFactory,
                        ClearFactory = color.ClearFactory,
                        CountInventory = color.CountInventory,
                        CheckDate = color.CheckDate,
                    });
                }
            }
        }

        private void ExportToExcel()
        {
            //建立Excel 2003檔案
            IWorkbook wb = new XSSFWorkbook();
            ISheet ws = wb.CreateSheet("Class");
            XSSFRow row = (XSSFRow)ws.CreateRow(0);
            row.Height = 440;

            ws.SetColumnWidth(0, 3000);
            ws.SetColumnWidth(1, 3150);
            ws.SetColumnWidth(2, 1700);
            ws.SetColumnWidth(4, 1700);

            ICellStyle positionStyle = wb.CreateCellStyle();
            positionStyle.WrapText = true;
            positionStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            positionStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;

            ExcelHelper.CreateCell(row, 0, "布種", positionStyle);
            ExcelHelper.CreateCell(row, 1, "顏色", positionStyle);
            ExcelHelper.CreateCell(row, 2, "織廠", positionStyle);
            ExcelHelper.CreateCell(row, 3, "整理", positionStyle);
            ExcelHelper.CreateCell(row, 4, "數量", positionStyle);
            ExcelHelper.CreateCell(row, 5, "時間", positionStyle);
            int rowIndex = 1;
            foreach (StoreData storeData in ShippingHistoryStoreDataList)
            {
                XSSFRow rowTextile = (XSSFRow)ws.CreateRow(rowIndex);

                ExcelHelper.CreateCell(rowTextile, 0, storeData.TextileName, positionStyle);
                ExcelHelper.CreateCell(rowTextile, 1, storeData.ColorName, positionStyle);
                ExcelHelper.CreateCell(rowTextile, 2, storeData.FabricFactory, positionStyle);
                ExcelHelper.CreateCell(rowTextile, 3, storeData.ClearFactory, positionStyle);
                ExcelHelper.CreateCell(rowTextile, 4, storeData.CountInventory, positionStyle);
                ExcelHelper.CreateCell(rowTextile, 5, storeData.CheckDate, positionStyle);

                rowIndex++;
            }
            var selectedDateTime = ShippingHistoryDate == null ? DateTime.Now.ToString("yyyyMMdd") : ShippingHistoryDate.ToString("yyyyMMdd");
            FileStream file = new FileStream(string.Concat(AppSettingConfig.StoreSearchFilePath(), "出貨查詢", selectedDateTime, ".xlsx"), FileMode.Create);//產生檔案
            wb.Write(file);
            file.Close();
        }

        void InventoryNumberRangeSearchExecute()
        {
            StoreDataList.Clear();
            IWorkbook workbook = null;  //新建IWorkbook對象
            string fileName = string.Concat(AppSettingConfig.FilePath(), "/", AppSettingConfig.StoreManageFileName());
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            workbook = new XSSFWorkbook(fileStream);  //xlsx數據讀入workbook

            for (int sheetCount = 1; sheetCount < workbook.NumberOfSheets; sheetCount++)
            {
                ISheet sheet = workbook.GetSheetAt(sheetCount);  //獲取第i個工作表  
                IRow row;// = sheet.GetRow(0);            //新建當前工作表行數據  

                StoreDataList.Add(new StoreData
                {
                    TextileName = sheet.SheetName,
                    ColorName = "",
                    CountInventory = ""
                });
                var colorList = new List<StoreData>();
                for (int rowNumber = 1; rowNumber < sheet.LastRowNum; rowNumber++)  //對工作表每一行  
                {
                    if (rowNumber > 50)
                        break;
                    row = sheet.GetRow(rowNumber);   //row讀入第i行數據  

                    if (row != null)
                    {
                        if (row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ColorName) == null)
                        {
                            break;
                        }
                        var countInventory = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.CountInventory);
                        if (countInventory == null || string.IsNullOrEmpty(countInventory.ToString()) || (countInventory.CellType == CellType.Formula && countInventory.CachedFormulaResultType == CellType.Error))
                        {
                            continue;
                        }
                        var cellValue = countInventory.NumericCellValue; //獲取i行j列數據
                        if (cellValue <= MaxNumber && cellValue >= MinNumber)
                        {
                            colorList.Add(new StoreData
                            {
                                ColorName = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ColorName) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ColorName).ToString(),
                                FabricFactory = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.FabricFactory) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.FabricFactory).ToString(),
                                ClearFactory = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ClearFactory) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ClearFactory).ToString(),
                                CountInventory = cellValue.ToString(),
                                CheckDate = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.CheckDate) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.CheckDate).ToString()
                            });
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                DateTime result;
                var soretColorList = colorList.OrderBy(o => o.FabricFactory).ThenBy(o => o.ClearFactory).ThenBy(o => Convert.ToDouble(o.CountInventory)).ThenByDescending(O => DateTime.TryParse(O.CheckDate, out result) == true ? result : DateTime.Now.AddDays(-360));
                foreach (var item in soretColorList)
                {
                    StoreDataList.Add(item);
                }
            }
            fileStream.Close();
            workbook.Close();
        }

        bool CanExecute()
        {
            return true;
        }


        /// <summary>
        /// 依據是否出貨過濾資料
        /// </summary>
        /// <param name="list"></param>
        /// <param name="row"></param>
        /// <param name="timeRange"></param>
        /// <returns></returns>
        public List<StoreSearchData<InventoryCheck>> IsShippedAction(List<StoreSearchData<InventoryCheck>> list, IRow row, int timeRange)
        {
            for (int columnIndex = 5; columnIndex < 14; columnIndex++)
            {
                if (row.GetCell(columnIndex) != null && row.GetCell(columnIndex).CellType != CellType.String && row.GetCell(columnIndex).NumericCellValue != 0)
                {
                    var countInventory = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.CountInventory);
                    if (countInventory == null || string.IsNullOrEmpty(countInventory.ToString()) || (countInventory.CellType == CellType.Formula && countInventory.CachedFormulaResultType == CellType.Error))
                    {
                        continue;
                    }
                    var cellValue = countInventory.NumericCellValue; //獲取i行j列數據
                    list.Last().StoreSearchColorDetails.Add(new InventoryCheck
                    {
                        ColorName = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ColorName) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ColorName).ToString(),
                        StorageSpaces = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.StorageSpaces) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.StorageSpaces).ToString(),
                        CountInventory = cellValue.ToString(),
                    });
                    break;
                }
            }

            return list;
        }

        private string CreateIsShippedExcelAction(IWorkbook wb, ISheet ws, ICellStyle positionStyle,ref int rowIndex, StoreSearchData<InventoryCheck> storeData)
        {
            XSSFRow rowTextile = (XSSFRow)ws.CreateRow(rowIndex);
            ExcelHelper.CreateCell(rowTextile, 0, storeData.TextileName, positionStyle);
            ws.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
            ws.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 4, 6));
            foreach (var item in storeData.StoreSearchColorDetails)
            {
                rowIndex++;
                XSSFRow rowColor = (XSSFRow)ws.CreateRow(rowIndex);
                ExcelHelper.CreateCell(rowColor, 0, item.ColorName, positionStyle);
                ExcelHelper.CreateCell(rowColor, 1, item.StorageSpaces, ExcelHelper.GetColorByStorageSpaces(wb, item.StorageSpaces));
                ExcelHelper.CreateCell(rowColor, 2, item.CountInventory, positionStyle);
            }

            rowIndex++;
            return "庫存盤點清單";
        }

        public void InventoryCheckSheetClick()
        {
            ExcelHelper excelHelper = new ExcelHelper();
            List<ColumnFormat> columnFormats = new List<ColumnFormat>()
            {
                new ColumnFormat
                {
                    CoiumnWidth = 3000,
                    ColumnTitle = "顏色",
                },
                new ColumnFormat
                {
                    CoiumnWidth = 2800,
                    ColumnTitle = "儲位",
                },
                new ColumnFormat
                {
                    CoiumnWidth = 2600,
                    ColumnTitle = "數量",
                },
                new ColumnFormat
                {
                    CoiumnWidth = 3000,
                    ColumnTitle = "清點資訊",
                },
                new ColumnFormat
                {
                    CoiumnWidth = 3000,
                    ColumnTitle = "顏色",
                },
                new ColumnFormat
                {
                    CoiumnWidth = 2800,
                    ColumnTitle = "儲位",
                },
                new ColumnFormat
                {
                    CoiumnWidth = 2600,
                    ColumnTitle = "數量",
                },
                new ColumnFormat
                {
                    CoiumnWidth = 3000,
                    ColumnTitle = "清點資訊",
                }
            };
            excelHelper.ButtonInventoryCheckSheet_Click<InventoryCheck>(IsShippedAction, CreateIsShippedExcelAction, 0, columnFormats);
        }

        /// <summary>
        /// 依據清點日期過濾資料
        /// </summary>
        /// <param name="list"></param>
        /// <param name="row"></param>
        /// <param name="timeRange"></param>
        /// <returns></returns>
        public List<StoreSearchData<InventoryCheck>> CheckDateAction(List<StoreSearchData<InventoryCheck>> list, IRow row, int timeRange)
        {
            var checkDate = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.CheckDate);
            var countInventory = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.CountInventory);

            if (checkDate == null || string.IsNullOrEmpty(checkDate.ToString()) || (checkDate.CellType == CellType.Formula && checkDate.CachedFormulaResultType == CellType.Error))
            {
                list.Last().StoreSearchColorDetails.Add(new InventoryCheck
                {
                    ColorName = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ColorName) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ColorName).ToString(),
                    StorageSpaces = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.StorageSpaces) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.StorageSpaces).ToString(),
                    CountInventory = countInventory == null || string.IsNullOrEmpty(countInventory.ToString()) || (countInventory.CellType == CellType.Formula && countInventory.CachedFormulaResultType == CellType.Error) ? "-10" : countInventory.NumericCellValue.ToString(),
                    CheckDate = null
                });
            }
            else
            {
                var correctCheckDate = checkDate.CellType == CellType.Numeric ? checkDate.DateCellValue : Convert.ToDateTime(checkDate.StringCellValue.ToString()).Date;
                if (correctCheckDate < DateTime.Now.AddDays(timeRange * -1))
                {
                    list.Last().StoreSearchColorDetails.Add(new InventoryCheck
                    {
                        ColorName = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ColorName) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ColorName).ToString(),
                        StorageSpaces = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.StorageSpaces) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.StorageSpaces).ToString(),
                        CountInventory = countInventory == null || string.IsNullOrEmpty(countInventory.ToString()) || (countInventory.CellType == CellType.Formula && countInventory.CachedFormulaResultType == CellType.Error) ? "-10" : countInventory.NumericCellValue.ToString(),
                        CheckDate = correctCheckDate
                    });
                }
            }
            return list;
        }

        private string CreateCheckDateExcelAction(IWorkbook wb, ISheet ws, ICellStyle positionStyle,ref int rowIndex, StoreSearchData<InventoryCheck> storeData)
        {
            XSSFRow rowTextile = (XSSFRow)ws.CreateRow(rowIndex);
            ExcelHelper.CreateCell(rowTextile, 0, storeData.TextileName, positionStyle);
            ws.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));
            ws.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 5, 7));
            
            foreach (var item in storeData.StoreSearchColorDetails.OrderBy(o => o.CheckDate))
            {
                rowIndex++;
                XSSFRow rowColor = (XSSFRow)ws.CreateRow(rowIndex);
                ExcelHelper.CreateCell(rowColor, 0, item.ColorName, positionStyle);
                ExcelHelper.CreateCell(rowColor, 1, item.StorageSpaces, ExcelHelper.GetColorByStorageSpaces(wb, item.StorageSpaces));
                ExcelHelper.CreateCell(rowColor, 2, item.CountInventory, positionStyle);
                ExcelHelper.CreateCell(rowColor, 3, item.CheckDate?.ToString("MM/dd"), positionStyle);
            }

            rowIndex++;
            return "檢查時間盤點清單";
        }

        private void ExportCheckDateToExcel()
        {
            ExcelHelper excelHelper = new ExcelHelper();
            List<ColumnFormat> columnFormats = new List<ColumnFormat>()
            {
                new ColumnFormat
                {
                    CoiumnWidth = 3000,
                    ColumnTitle = "顏色",
                },
                new ColumnFormat
                {
                    CoiumnWidth = 2800,
                    ColumnTitle = "儲位",
                },
                new ColumnFormat
                {
                    CoiumnWidth = 1850,
                    ColumnTitle = "數量",
                },
                new ColumnFormat
                {
                    CoiumnWidth = 1850,
                    ColumnTitle = "清點日期",
                },
                new ColumnFormat
                {
                    CoiumnWidth = 3000,
                    ColumnTitle = "清點資訊",
                },

            };
            excelHelper.ButtonInventoryCheckSheet_Click<InventoryCheck>(CheckDateAction, CreateCheckDateExcelAction, DateRange, columnFormats);
        }
    }


}
