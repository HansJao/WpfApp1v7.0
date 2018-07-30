using NPOI.SS.UserModel;
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
        public StoreSearchViewModel()
        {
            _storeDataList = new ObservableCollection<StoreData>();
            _shippingHistoryStoreDataList = new ObservableCollection<StoreData>();
            MaxNumber = 3;
            MinNumber = 0;
        }

        private ObservableCollection<StoreData> _storeDataList { get; set; }
        private int _maxNumber { get; set; }
        private int _minNumber { get; set; }

        public ObservableCollection<StoreData> StoreDataList
        {
            get { return _storeDataList; }
            set { _storeDataList = value; }
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
                for (int i = 5; i < 14; i++)
                {
                    var cell = firstRow.GetCell(i);
                    var cellValue = cell.StringCellValue;
                    if (cellValue == currentDateValue)
                    {
                        currentDateCellIndex = i;
                        break;
                    }
                }
                list.Add(new StoreSearchData<StoreSearchColorDetail>
                {
                    TextileName = sheet.SheetName,
                    StoreSearchColorDetails = new List<StoreSearchColorDetail>()
                });
                var colorList = new List<StoreData>();
                for (int i = 1; i < sheet.LastRowNum; i++)  //對工作表每一行  
                {
                    if (i > 100)
                        break;
                    row = sheet.GetRow(i);   //row讀入第i行數據  

                    if (row != null)
                    {
                        if (row.GetCell(1) == null)
                        {
                            break;
                        }
                        if (row.GetCell(currentDateCellIndex) != null && row.GetCell(currentDateCellIndex).CellType != CellType.String && row.GetCell(currentDateCellIndex).NumericCellValue != 0)
                        {
                            var countInventory = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.CountInventory);
                            if (countInventory == null || string.IsNullOrEmpty(countInventory.ToString()) || (countInventory.CellType == CellType.Formula && countInventory.CachedFormulaResultType == CellType.Error))
                            {
                                continue;
                            }
                            var cellValue = countInventory.NumericCellValue; //獲取i行j列數據
                            list.Last().StoreSearchColorDetails.Add(new StoreSearchColorDetail
                            {
                                ColorName = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ColorName) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ColorName).ToString(),
                                FabricFactory = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.FabricFactory) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.FabricFactory).ToString(),
                                ClearFactory = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ClearFactory) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ClearFactory).ToString(),
                                CountInventory = cellValue.ToString(),
                                CheckDate = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.CheckDate) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.CheckDate).ToString()
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
            ws.SetColumnWidth(4, 1700);
            ws.SetColumnWidth(2, 1700);

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
        public void ButtonInventoryCheckSheet_Click()
        {
            IWorkbook workbook = null;  //新建IWorkbook對象  
            string fileName = string.Concat(AppSettingConfig.FilePath(), "/", AppSettingConfig.StoreManageFileName());
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            workbook = new XSSFWorkbook(fileStream);  //xlsx數據讀入workbook
            var list = new List<StoreSearchData<InventoryCheck>>();
            for (int sheetCount = 1; sheetCount < workbook.NumberOfSheets; sheetCount++)
            {
                ISheet sheet = workbook.GetSheetAt(sheetCount);  //獲取第i個工作表  
                IRow row;
                var firstRow = sheet.GetRow(0);

                list.Add(new StoreSearchData<InventoryCheck>
                {
                    TextileName = sheet.SheetName,
                    StoreSearchColorDetails = new List<InventoryCheck>()
                });
                var colorList = new List<StoreData>();
                for (int rowIndex = 1; rowIndex < sheet.LastRowNum; rowIndex++)  //對工作表每一行  
                {
                    if (rowIndex > 70)
                        break;
                    row = sheet.GetRow(rowIndex);   //row讀入第i行數據  

                    if (row != null)
                    {
                        if (row.GetCell(1) == null)
                        {
                            break;
                        }
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
                    }
                    else
                    {
                        break;
                    }
                }
            }

            CreateInventoryCheckExcel(list);
        }
        public void CreateInventoryCheckExcel(List<StoreSearchData<InventoryCheck>> list)
        {
            //建立Excel 2003檔案
            IWorkbook wb = new XSSFWorkbook();
            ISheet ws = wb.CreateSheet("Class");
            XSSFRow row = (XSSFRow)ws.CreateRow(0);
            row.Height = 440;

            ws.SetColumnWidth(0, 3000);
            ws.SetColumnWidth(1, 2800);
            ws.SetColumnWidth(3, 3000);
            ws.SetColumnWidth(4, 3000);
            ws.SetColumnWidth(5, 2800);
            ws.SetColumnWidth(7, 3000);
            ICellStyle positionStyle = wb.CreateCellStyle();
            positionStyle.WrapText = true;
            positionStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            positionStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;

            ICellStyle aquaStyle = wb.CreateCellStyle();
            aquaStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Aqua.Index;
            aquaStyle.FillPattern = FillPattern.SolidForeground;

            ExcelHelper.CreateCell(row, 0, "顏色", positionStyle);
            ExcelHelper.CreateCell(row, 1, "儲位", positionStyle);
            ExcelHelper.CreateCell(row, 2, "數量", positionStyle);
            ExcelHelper.CreateCell(row, 3, "清點資訊", positionStyle);

            ExcelHelper.CreateCell(row, 4, "顏色", positionStyle);
            ExcelHelper.CreateCell(row, 5, "儲位", positionStyle);
            ExcelHelper.CreateCell(row, 6, "數量", positionStyle);
            ExcelHelper.CreateCell(row, 7, "清點資訊", positionStyle);
            int rowIndex = 1;
            foreach (var storeData in list)
            {
                if (storeData.StoreSearchColorDetails.Count() == 0)
                {
                    continue;
                }
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
            FileStream file = new FileStream(string.Concat(AppSettingConfig.FilePath(), @"\", "庫存盤點清單", DateTime.Now.ToString("yyyyMMdd"), ".xlsx"), FileMode.Create);//產生檔案
            wb.Write(file);
            file.Close();
        }
    }


}
