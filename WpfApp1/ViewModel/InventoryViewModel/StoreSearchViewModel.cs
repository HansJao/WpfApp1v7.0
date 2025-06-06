﻿using NPOI.SS.UserModel;
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
using WpfApp1.DataClass.Fabric;
using WpfApp1.DataClass.StoreSearch;
using WpfApp1.DataClass.TrashSystem;
using WpfApp1.Modules.ExcelModule;
using WpfApp1.Modules.ExcelModule.Implement;
using WpfApp1.Modules.TrashModule;
using WpfApp1.Modules.TrashModule.Implement;
using WpfApp1.Utility;

namespace WpfApp1.ViewModel.InventoryViewModel
{
    public class StoreSearchViewModel : ViewModelBase
    {
        //定義一個ICommand型別的參數，他會回傳實作ICommand介面的RelayCommand類別。
        public ICommand InventoryNumberRangeClick { get { return new RelayCommand(InventoryNumberRangeSearchExecute, CanExecute); } }
        public ICommand ExportToExcelClick { get { return new RelayCommand(ExportToExcel, CanExecute); } }
        public ICommand ExportAIToExcelClick { get { return new RelayCommand(ExportAIToExcel, CanExecute); } }
        public ICommand ExportCheckDateToExcelClick { get { return new RelayCommand(ExportCheckDateToExcel, CanExecute); } }
        public ICommand TestInteractivity { get { return new RelayCommand(TestInteractivityExecute, CanExecute); } }

        private void TestInteractivityExecute()
        {
            throw new NotImplementedException();
        }

        protected IExcelModule ExcelModule { get; } = new ExcelModule();
        protected ITrashModule TrashModule { get; } = new TrashModule();

        public StoreSearchViewModel()
        {
            StoreDataList = new ObservableCollection<StoreData>();
            ShippingHistoryStoreDataList = new ObservableCollection<StoreData>();
            ShippingHistoryStoreAIDataList = new ObservableCollection<StoreData>();
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

        public ObservableCollection<StoreData> ShippingHistoryStoreAIDataList { get; set; }

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
                    ShippingHistoryStoreAIDataList.Clear();
                    CreateStoreSearchListByShipped();
                    RaisePropertyChanged("ShippingHistoryDate");
                }
            }
        }

        public List<StoreSearchData<StoreSearchColorDetail>> CreateStoreSearchListByShipped()
        {
            List<StoreSearchData<StoreSearchColorDetail>> storeSearchDatas = ExcelModule.GetExcelDailyShippedList(ShippingHistoryDate);
            foreach (StoreSearchData<StoreSearchColorDetail> item in storeSearchDatas)
            {
                if (item.StoreSearchColorDetails.Count() == 0)
                {
                    continue;
                }
                ShippingHistoryStoreDataList.Add(new StoreData
                {
                    TextileName = item.TextileName,

                });
                foreach (StoreSearchColorDetail color in item.StoreSearchColorDetails)
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

                    ShippingHistoryStoreAIDataList.Add(new StoreData
                    {
                        TextileName = item.TextileName,
                        ColorName = color.ColorName,
                        FabricFactory = color.FabricFactory,
                        ClearFactory = color.ClearFactory,
                        ShippedCount = color.ShippedCount,
                        CountInventory = color.CountInventory,
                        CheckDate = color.CheckDate,
                    });
                }
            }
            return storeSearchDatas;
        }

        private void ExportAIToExcel()
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

            ICellStyle greyStyle = wb.CreateCellStyle();
            greyStyle.WrapText = true;
            greyStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            greyStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
            greyStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
            greyStyle.FillPattern = FillPattern.SolidForeground;

            ICellStyle lightGreenStyle = wb.CreateCellStyle();
            lightGreenStyle.WrapText = true;
            lightGreenStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            lightGreenStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
            lightGreenStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightGreen.Index;
            lightGreenStyle.FillPattern = FillPattern.SolidForeground;

            ICellStyle lightTurquoiseStyle = wb.CreateCellStyle();
            lightTurquoiseStyle.WrapText = true;
            lightTurquoiseStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            lightTurquoiseStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
            lightTurquoiseStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightTurquoise.Index;
            lightTurquoiseStyle.FillPattern = FillPattern.SolidForeground;

            ICellStyle coralStyle = wb.CreateCellStyle();
            coralStyle.WrapText = true;
            coralStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            coralStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
            coralStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Coral.Index;
            coralStyle.FillPattern = FillPattern.SolidForeground;

            ExcelHelper.CreateCell(row, 0, "布種", positionStyle);
            ExcelHelper.CreateCell(row, 1, "顏色", positionStyle);
            ExcelHelper.CreateCell(row, 2, "訂單號", positionStyle);
            ExcelHelper.CreateCell(row, 3, "織廠", positionStyle);
            ExcelHelper.CreateCell(row, 4, "整理", positionStyle);
            ExcelHelper.CreateCell(row, 5, "開單日", positionStyle);
            ExcelHelper.CreateCell(row, 6, "入布日", positionStyle);
            ExcelHelper.CreateCell(row, 7, "染定廠", positionStyle);
            ExcelHelper.CreateCell(row, 8, "製作數量", positionStyle);
            ExcelHelper.CreateCell(row, 9, "出貨量", positionStyle);
            ExcelHelper.CreateCell(row, 10, "計算庫存量", positionStyle);
            ExcelHelper.CreateCell(row, 11, "時間", positionStyle);
            ExcelHelper.CreateCell(row, 12, "10天內", positionStyle);
            ExcelHelper.CreateCell(row, 13, "20天內", positionStyle);
            ExcelHelper.CreateCell(row, 14, "30天內", positionStyle);
            ExcelHelper.CreateCell(row, 15, "60天內", positionStyle);


            int rowIndex = 1;
            string textileName = string.Empty;
            ExternalDataHelper externalDataHelper = new ExternalDataHelper();
            IEnumerable<TextileNameMapping> textileNameMappings = externalDataHelper.GetTextileNameMappings();
            IEnumerable<TrashItem> trashItems = TrashModule.GetTrashItems().Where(w => w.I_03 != null).OrderBy(o => o.I_01);
            DateTime tenDays = DateTime.Now.Date.AddDays(-10);
            DateTime twentyDays = DateTime.Now.Date.AddDays(-20);
            DateTime thirtyDays = DateTime.Now.Date.AddDays(-30);
            List<TrashShipped> xtrashCustomerShippeds = TrashModule.GetTrashShippedList(DateTime.Now.AddDays(-60), DateTime.Now).ToList();

            foreach (StoreData storeData in ShippingHistoryStoreAIDataList)
            {
                XSSFRow rowTextile = (XSSFRow)ws.CreateRow(rowIndex);

                ExcelHelper.CreateCell(rowTextile, 0, storeData.TextileName, positionStyle);

                textileName = storeData.TextileName;
                TrashItem trashItem = externalDataHelper.GetTrashItemFromInventoryMapping(trashItems, textileName, storeData.ColorName.Split('-')[0], textileNameMappings);
                List<TrashShipped> trashShippeds = new List<TrashShipped>();
                if (trashItem != null)
                {
                    trashShippeds = xtrashCustomerShippeds.Where(w => w.I_03 == trashItem.I_03).ToList();
                }
                string[] ColorNameParts = (storeData.ColorName ?? "").Split('-');
                string colorName = ColorNameParts.ElementAtOrDefault(0) ?? "";
                string order = ColorNameParts.ElementAtOrDefault(1) ?? "";
                ExcelHelper.CreateCell(rowTextile, 1, colorName, positionStyle);
                ExcelHelper.CreateCell(rowTextile, 2, order, positionStyle);
                ExcelHelper.CreateCell(rowTextile, 3, storeData.FabricFactory, positionStyle);

                string[] ClearFactoryParts = (storeData.ClearFactory ?? "").Split('-');
                string startDate = ClearFactoryParts.ElementAtOrDefault(0) ?? "";
                string inputDate = ClearFactoryParts.ElementAtOrDefault(1) ?? "";
                string factory = ClearFactoryParts.ElementAtOrDefault(2) ?? "";
                string amount = ClearFactoryParts.ElementAtOrDefault(3) ?? "";

                ExcelHelper.CreateCell(rowTextile, 4, storeData.ClearFactory, positionStyle);
                ExcelHelper.CreateCell(rowTextile, 5, startDate, positionStyle);
                ExcelHelper.CreateCell(rowTextile, 6, inputDate, positionStyle);
                ExcelHelper.CreateCell(rowTextile, 7, factory, positionStyle);
                ExcelHelper.CreateCell(rowTextile, 8, amount, positionStyle);
                ExcelHelper.CreateCell(rowTextile, 9, storeData.ShippedCount.ToString(), positionStyle);
                ExcelHelper.CreateCell(rowTextile, 10, storeData.CountInventory, positionStyle);
                ExcelHelper.CreateCell(rowTextile, 11, storeData.CheckDate, positionStyle);
                ExcelHelper.CreateCell(rowTextile, 12, Math.Round(trashShippeds.Where(w => w.IN_DATE.Date >= tenDays).Sum(s => s.Quantity) / 22, 0), greyStyle);
                ExcelHelper.CreateCell(rowTextile, 13, Math.Round(trashShippeds.Where(w => w.IN_DATE.Date >= twentyDays).Sum(s => s.Quantity) / 22, 0), lightGreenStyle);
                ExcelHelper.CreateCell(rowTextile, 14, Math.Round(trashShippeds.Where(w => w.IN_DATE.Date >= thirtyDays).Sum(s => s.Quantity) / 22, 0), lightTurquoiseStyle);
                ExcelHelper.CreateCell(rowTextile, 15, Math.Round(trashShippeds.Sum(s => s.Quantity) / 22, 0), coralStyle);

                rowIndex++;
            }
            var selectedDateTime = ShippingHistoryDate == null ? DateTime.Now.ToString("yyyyMMdd") : ShippingHistoryDate.ToString("yyyyMMdd");
            FileStream file = new FileStream(string.Concat(AppSettingConfig.StoreSearchFilePath(), "AI出貨查詢", selectedDateTime, ".xlsx"), FileMode.Create);//產生檔案
            wb.Write(file);
            file.Close();
        }

        // 輔助方法：創建單元格樣式
        private static ICellStyle CreateCellStyle(IWorkbook wb, HorizontalAlignment hAlign, VerticalAlignment vAlign, bool wrapText = false, short? fillColor = null)
        {
            var style = wb.CreateCellStyle();
            style.WrapText = wrapText;
            style.Alignment = hAlign;
            style.VerticalAlignment = vAlign;

            if (fillColor.HasValue)
            {
                style.FillForegroundColor = fillColor.Value;
                style.FillPattern = FillPattern.SolidForeground;
            }

            return style;
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

            // 創建樣式
            var positionStyle = CreateCellStyle(wb, HorizontalAlignment.Center, VerticalAlignment.Center, wrapText: true);
            var greyStyle = CreateCellStyle(wb, HorizontalAlignment.Center, VerticalAlignment.Center, wrapText: true, fillColor: NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index);
            var lightGreenStyle = CreateCellStyle(wb, HorizontalAlignment.Center, VerticalAlignment.Center, wrapText: true, fillColor: NPOI.HSSF.Util.HSSFColor.LightGreen.Index);
            var lightTurquoiseStyle = CreateCellStyle(wb, HorizontalAlignment.Center, VerticalAlignment.Center, wrapText: true, fillColor: NPOI.HSSF.Util.HSSFColor.LightTurquoise.Index);
            var coralStyle = CreateCellStyle(wb, HorizontalAlignment.Center, VerticalAlignment.Center, wrapText: true, fillColor: NPOI.HSSF.Util.HSSFColor.Coral.Index);

            ExcelHelper.CreateCell(row, 0, "布種", positionStyle);
            ExcelHelper.CreateCell(row, 1, "顏色", positionStyle);
            ExcelHelper.CreateCell(row, 2, "織廠", positionStyle);
            ExcelHelper.CreateCell(row, 3, "整理", positionStyle);
            ExcelHelper.CreateCell(row, 4, "出貨量", positionStyle);
            ExcelHelper.CreateCell(row, 5, "計算庫存量", positionStyle);
            ExcelHelper.CreateCell(row, 6, "剩餘數量", positionStyle);
            ExcelHelper.CreateCell(row, 7, "10天內", positionStyle);
            ExcelHelper.CreateCell(row, 8, "20天內", positionStyle);
            ExcelHelper.CreateCell(row, 9, "30天內", positionStyle);
            ExcelHelper.CreateCell(row, 10, "60天內", positionStyle);


            int rowIndex = 1;
            string textileName = string.Empty;
            ExternalDataHelper externalDataHelper = new ExternalDataHelper();
            IEnumerable<TextileNameMapping> textileNameMappings = externalDataHelper.GetTextileNameMappings();
            IEnumerable<TrashItem> trashItems = TrashModule.GetTrashItems().Where(w => w.I_03 != null).OrderBy(o => o.I_01);
            DateTime tenDays = DateTime.Now.Date.AddDays(-10);
            DateTime twentyDays = DateTime.Now.Date.AddDays(-20);
            DateTime thirtyDays = DateTime.Now.Date.AddDays(-30);
            List<TrashShipped> xtrashCustomerShippeds = TrashModule.GetTrashShippedList(DateTime.Now.AddDays(-60), DateTime.Now).ToList();
            var excelModul = new ExcelModule();
            string filePath = string.Concat(AppSettingConfig.FilePath(), "/", AppSettingConfig.StoreManageFileName());
            IWorkbook inventoryWorkbook = null;
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                inventoryWorkbook = new XSSFWorkbook(fileStream);
                foreach (StoreData storeData in ShippingHistoryStoreDataList)
                {
                    XSSFRow rowTextile = (XSSFRow)ws.CreateRow(rowIndex);

                    ExcelHelper.CreateCell(rowTextile, 0, storeData.TextileName, positionStyle);
                    if (!string.IsNullOrEmpty(storeData.TextileName))
                    {
                        textileName = storeData.TextileName;
                        rowIndex++;
                        continue;
                    }

                    string restAmount = excelModul.GetExcelMatchColorList(inventoryWorkbook, textileName, storeData);
                    TrashItem trashItem = externalDataHelper.GetTrashItemFromInventoryMapping(trashItems, textileName, storeData.ColorName.Split('-')[0], textileNameMappings);
                    List<TrashShipped> trashShippeds = new List<TrashShipped>();
                    if (trashItem != null)
                    {
                        trashShippeds = xtrashCustomerShippeds.Where(w => w.I_03 == trashItem.I_03).ToList();
                    }
                    ExcelHelper.CreateCell(rowTextile, 1, storeData.ColorName, positionStyle);
                    ExcelHelper.CreateCell(rowTextile, 2, storeData.FabricFactory, positionStyle);
                    ExcelHelper.CreateCell(rowTextile, 3, storeData.ClearFactory, positionStyle);
                    ExcelHelper.CreateCell(rowTextile, 4, storeData.ShippedCount.ToString(), positionStyle);
                    ExcelHelper.CreateCell(rowTextile, 5, storeData.CountInventory, positionStyle);
                    ExcelHelper.CreateCell(rowTextile, 6, restAmount, positionStyle);
                    ExcelHelper.CreateCell(rowTextile, 7, Math.Round(trashShippeds.Where(w => w.IN_DATE.Date >= tenDays).Sum(s => s.Quantity) / 22, 0), greyStyle);
                    ExcelHelper.CreateCell(rowTextile, 8, Math.Round(trashShippeds.Where(w => w.IN_DATE.Date >= twentyDays).Sum(s => s.Quantity) / 22, 0), lightGreenStyle);
                    ExcelHelper.CreateCell(rowTextile, 9, Math.Round(trashShippeds.Where(w => w.IN_DATE.Date >= thirtyDays).Sum(s => s.Quantity) / 22, 0), lightTurquoiseStyle);
                    ExcelHelper.CreateCell(rowTextile, 10, Math.Round(trashShippeds.Sum(s => s.Quantity) / 22, 0), coralStyle);

                    rowIndex++;
                }
            }
            var selectedDateTime = ShippingHistoryDate == null ? DateTime.Now.ToString("yyyyMMdd") : ShippingHistoryDate.ToString("yyyyMMdd");
            FileStream file = new FileStream(string.Concat(AppSettingConfig.StoreSearchFilePath(), "出貨查詢", selectedDateTime, ".xlsx"), FileMode.Create);//產生檔案
            wb.Write(file);
            file.Close();
        }
        public string StoreArea { get; set; } = "1B,1C,2A,2B,2C";
        public string ExceptArea { get; set; } = "小,大";
        public string TextileName { get; set; } = "";

        private void InventoryNumberRangeSearchExecute()
        {
            StoreDataList.Clear();
            IWorkbook workbook = null;  //新建IWorkbook對象
            string fileName = string.Concat(AppSettingConfig.FilePath(), "/", AppSettingConfig.StoreManageFileName());
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            workbook = new XSSFWorkbook(fileStream);  //xlsx數據讀入workbook
            Regex checkStoreAreaPattern = new Regex(string.Concat("(^", StoreArea.Replace(",", ")+|(^"), ")+"));
            Regex checkExceptAreaPattern = new Regex(string.Concat("(", ExceptArea.Replace(",", ")+|("), ")+"));
            Regex checkTextileNamePattern = new Regex(string.Concat("(", TextileName.Replace(",", ")+|("), ")+"));

            for (int sheetCount = 1; sheetCount < workbook.NumberOfSheets; sheetCount++)
            {
                ISheet sheet = workbook.GetSheetAt(sheetCount);  //獲取第i個工作表  
                IRow row;// = sheet.GetRow(0);            //新建當前工作表行數據  

                if (!checkTextileNamePattern.IsMatch(sheet.SheetName))
                {
                    continue;
                }
                var colorList = new List<StoreData>();
                for (int rowNumber = 1; rowNumber <= sheet.LastRowNum; rowNumber++)  //對工作表每一行
                {
                    if (rowNumber > 200)
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
                        if (cellValue <= MaxNumber && cellValue >= MinNumber && checkStoreAreaPattern.IsMatch(storeArea) && !checkExceptAreaPattern.IsMatch(storeArea))
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
                        CoiumnWidth = 4580,
                        ColumnTitle = "顏色",
                    },
                    new ColumnFormat
                    {
                        CoiumnWidth = 2810,
                        ColumnTitle = "儲位",
                    },
                    new ColumnFormat
                    {
                        CoiumnWidth = 1540,
                        ColumnTitle = "數量",
                    },
                    new ColumnFormat
                    {
                        CoiumnWidth = 2380,
                        ColumnTitle = "清點資訊",
                    },
                    new ColumnFormat
                    {
                        CoiumnWidth = 4580,
                        ColumnTitle = "顏色",
                    },
                    new ColumnFormat
                    {
                        CoiumnWidth = 2810,
                        ColumnTitle = "儲位",
                    },
                    new ColumnFormat
                    {
                        CoiumnWidth = 1540,
                        ColumnTitle = "數量",
                    },
                    new ColumnFormat
                    {
                        CoiumnWidth = 2380,
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
