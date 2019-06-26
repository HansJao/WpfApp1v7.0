using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfApp1.Command;
using WpfApp1.DataClass.Enumeration;
using WpfApp1.DataClass.ExcelDataClass;
using WpfApp1.DataClass.StoreSearch;
using WpfApp1.Modules.ExcelModule;
using WpfApp1.Modules.ExcelModule.Implement;
using WpfApp1.Utility;

namespace WpfApp1.ViewModel.InventoryViewModel
{
    public class StoreSearchViewModel : ViewModelBase
    {
        //定義一個ICommand型別的參數，他會回傳實作ICommand介面的RelayCommand類別。
        public ICommand InventoryNumberRangeClick { get { return new RelayCommand(InventoryNumberRangeSearchExecute, CanExecute); } }
        public ICommand ExportToExcelClick { get { return new RelayCommand(ExportToExcel, CanExecute); } }
        public ICommand ExportCheckDateToExcelClick { get { return new RelayCommand(ExportCheckDateToExcel, CanExecute); } }
        public ICommand TestInteractivity { get { return new RelayCommand(TestInteractivityExecute, CanExecute); } }

        private void TestInteractivityExecute()
        {
            throw new NotImplementedException();
        }

        protected IExcelModule ExcelModule { get; } = new ExcelModule();

        public StoreSearchViewModel()
        {
            StoreDataList = new ObservableCollection<StoreData>();
            ShippingHistoryStoreDataList = new ObservableCollection<StoreData>();
            MaxNumber = 3;
            MinNumber = 0;
            DateRange = 10;
        }

        public StoreData StoreData { get; set; }

        public ObservableCollection<StoreData> StoreDataList { get; set; }

        public int DateRange { get; set; }

        public int MaxNumber { get; set; }
        public int MinNumber { get; set; }
        public ObservableCollection<StoreData> ShippingHistoryStoreDataList { get; set; }

        private DateTime _shippingHistoryDate { get; set; } = DateTime.Now;
        public DateTime ShippingHistoryDate
        {
            get { return _shippingHistoryDate; }
            set
            {
                if (_shippingHistoryDate != value)
                {
                    _shippingHistoryDate = value;
                    ShippingHistoryStoreDataList.Clear();
                    CreateStoreSearchListByShipped();
                    RaisePropertyChanged("ShippingHistoryDate");
                }
            }
        }

        public List<StoreSearchData<StoreSearchColorDetail>> CreateStoreSearchListByShipped()
        {
            var list = new List<StoreSearchData<StoreSearchColorDetail>>();
            list = ExcelModule.GetExcelDailyShippedList(ShippingHistoryDate);

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
                        ShippedCount = color.ShippedCount,
                        CountInventory = color.CountInventory,
                        CheckDate = color.CheckDate,
                    });
                }
            }
            return list;
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
            ExcelHelper.CreateCell(row, 4, "出貨量", positionStyle);
            ExcelHelper.CreateCell(row, 5, "計算庫存量", positionStyle);
            ExcelHelper.CreateCell(row, 6, "時間", positionStyle);
            int rowIndex = 1;
            foreach (StoreData storeData in ShippingHistoryStoreDataList)
            {
                XSSFRow rowTextile = (XSSFRow)ws.CreateRow(rowIndex);

                ExcelHelper.CreateCell(rowTextile, 0, storeData.TextileName, positionStyle);
                if (!string.IsNullOrEmpty(storeData.TextileName))
                {
                    rowIndex++;
                    continue;
                }
                ExcelHelper.CreateCell(rowTextile, 1, storeData.ColorName, positionStyle);
                ExcelHelper.CreateCell(rowTextile, 2, storeData.FabricFactory, positionStyle);
                ExcelHelper.CreateCell(rowTextile, 3, storeData.ClearFactory, positionStyle);
                ExcelHelper.CreateCell(rowTextile, 4, storeData.ShippedCount.ToString(), positionStyle);
                ExcelHelper.CreateCell(rowTextile, 5, storeData.CountInventory, positionStyle);
                ExcelHelper.CreateCell(rowTextile, 6, storeData.CheckDate, positionStyle);

                rowIndex++;
            }
            var selectedDateTime = ShippingHistoryDate == null ? DateTime.Now.ToString("yyyyMMdd") : ShippingHistoryDate.ToString("yyyyMMdd");
            FileStream file = new FileStream(string.Concat(AppSettingConfig.StoreSearchFilePath(), "出貨查詢", selectedDateTime, ".xlsx"), FileMode.Create);//產生檔案
            wb.Write(file);
            file.Close();
        }
        public string StoreArea { get; set; } = "1A,1B,1C,1D,1E,1F,1G,1H,1I,1J,1K,1L,1M,1N,1O,1P,1Q,1R,1S,1T,2A,2B,2C,2D";
        void InventoryNumberRangeSearchExecute()
        {
            StoreDataList.Clear();
            IWorkbook workbook = null;  //新建IWorkbook對象
            string fileName = string.Concat(AppSettingConfig.FilePath(), "/", AppSettingConfig.StoreManageFileName());
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            workbook = new XSSFWorkbook(fileStream);  //xlsx數據讀入workbook
            var checkStoreAreaPattern = new Regex(string.Concat("(", StoreArea.Replace(",", ")+|("), ")+"));

            for (int sheetCount = 1; sheetCount < workbook.NumberOfSheets; sheetCount++)
            {
                ISheet sheet = workbook.GetSheetAt(sheetCount);  //獲取第i個工作表  
                IRow row;// = sheet.GetRow(0);            //新建當前工作表行數據  

                var colorList = new List<StoreData>();
                for (int rowNumber = 1; rowNumber < sheet.LastRowNum; rowNumber++)  //對工作表每一行  
                {
                    if (rowNumber > 70)
                        break;
                    row = sheet.GetRow(rowNumber);   //row讀入第i行數據  

                    if (row != null)
                    {
                        if (row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ColorName) == null)
                        {
                            break;
                        }
                        ICell countInventory = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.CountInventory);
                        if (countInventory == null || string.IsNullOrEmpty(countInventory.ToString()) || (countInventory.CellType == CellType.Formula && countInventory.CachedFormulaResultType == CellType.Error))
                        {
                            continue;
                        }
                        double cellValue = countInventory.NumericCellValue; //獲取i行j列數據
                        string storeArea = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.StorageSpaces) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.StorageSpaces).ToString();
                        if (cellValue <= MaxNumber && cellValue >= MinNumber && checkStoreAreaPattern.IsMatch(storeArea))
                        {
                            colorList.Add(new StoreData
                            {
                                ColorName = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ColorName) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ColorName).ToString(),
                                StoreArea = storeArea,
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
                if (soretColorList.Count() > 0)
                {
                    StoreDataList.Add(new StoreData
                    {
                        TextileName = sheet.SheetName,
                        ColorName = "",
                        CountInventory = ""
                    });
                }
                foreach (var item in soretColorList)
                {
                    StoreDataList.Add(item);
                }
            }
            fileStream.Close();
            workbook.Close();
        }

        /// <summary>
        /// 依據是否出貨過濾資料
        /// </summary>
        /// <param name="list"></param>
        /// <param name="row"></param>
        /// <param name="timeRange"></param>
        /// <returns></returns>
        public List<StoreSearchData<InventoryCheck>> IsShippedAction(List<StoreSearchData<InventoryCheck>> list, IRow row, string sheetName, int timeRange)
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
                    //如果沒有這個布種名稱則新增
                    if (list.Where(w => w.TextileName == sheetName).Count() == 0)
                    {
                        list.Add(new StoreSearchData<InventoryCheck>
                        {
                            TextileName = sheetName,
                            StoreSearchColorDetails = new List<InventoryCheck>()
                        });
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
        /// <summary>
        /// 建立已出貨的庫存盤點Excel清單
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="ws"></param>
        /// <param name="positionStyle"></param>
        /// <param name="rowIndex"></param>
        /// <param name="storeData"></param>
        /// <returns></returns>
        private void CreateIsShippedExcelAction(IWorkbook wb, ISheet ws, ICellStyle positionStyle, ref int rowIndex, StoreSearchData<InventoryCheck> storeData)
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
        }

        /// <summary>
        /// 庫存盤點清單
        /// </summary>
        public void InventoryCheckSheetClick()
        {
            ExcelHelper excelHelper = new ExcelHelper();
            ExcelFormat excelFormat = new ExcelFormat()
            {
                FileName = "庫存盤點清單",
                ColumnFormats = new List<ColumnFormat>
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
                }
            };
            excelHelper.InventoryCheckSheet<StoreSearchData<InventoryCheck>>(IsShippedAction, CreateIsShippedExcelAction, 0, excelFormat);
        }

        /// <summary>
        /// 依據清點日期過濾資料
        /// </summary>
        /// <param name="list"></param>
        /// <param name="row"></param>
        /// <param name="timeRange"></param>
        /// <returns></returns>
        public List<StoreSearchData<InventoryCheck>> CheckDateAction(List<StoreSearchData<InventoryCheck>> list, IRow row, string sheetName, int timeRange)
        {
            //如果沒有這個布種名稱則新增
            if (list.Where(w => w.TextileName == sheetName).Count() == 0)
            {
                list.Add(new StoreSearchData<InventoryCheck>
                {
                    TextileName = sheetName,
                    StoreSearchColorDetails = new List<InventoryCheck>()
                });
            }
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

        /// <summary>
        /// 建立檢查時間點盤點清單
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="ws"></param>
        /// <param name="positionStyle"></param>
        /// <param name="rowIndex"></param>
        /// <param name="storeData"></param>
        /// <returns></returns>
        private void CreateCheckDateExcelAction(IWorkbook wb, ISheet ws, ICellStyle positionStyle, ref int rowIndex, StoreSearchData<InventoryCheck> storeData)
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
        }

        /// <summary>
        /// 檢查時間點盤點清單
        /// </summary>
        private void ExportCheckDateToExcel()
        {
            ExcelHelper excelHelper = new ExcelHelper();
            ExcelFormat excelFormat = new ExcelFormat()
            {
                FileName = "檢查時間盤點清單",
                ColumnFormats = new List<ColumnFormat>
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
                }
            };
            excelHelper.InventoryCheckSheet<StoreSearchData<InventoryCheck>>(CheckDateAction, CreateCheckDateExcelAction, DateRange, excelFormat);
        }
    }


}
