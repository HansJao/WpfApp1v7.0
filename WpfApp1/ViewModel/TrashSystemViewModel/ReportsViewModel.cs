﻿using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfApp1.Command;
using WpfApp1.DataClass.ExcelDataClass;
using WpfApp1.DataClass.Reports;
using WpfApp1.DataClass.StoreSearch;
using WpfApp1.DataClass.TrashSystem;
using WpfApp1.Modules.ExcelModule;
using WpfApp1.Modules.ExcelModule.Implement;
using WpfApp1.Modules.TrashModule;
using WpfApp1.Modules.TrashModule.Implement;
using WpfApp1.Utility;

namespace WpfApp1.ViewModel.TrashSystemViewModel
{
    public class ReportsViewModel : ViewModelBase
    {
        protected IExcelModule ExcelModule { get; } = new ExcelModule();
        protected ITrashModule TrashModule { get; } = new TrashModule();
        public ICommand ExportShippingCheckExecuteClick { get { return new RelayCommand(ExportShippingCheckExecute, CanExecute); } }
        public ICommand ButtonExportExecuteClick { get { return new RelayCommand(ButtonExportExecute, CanExecute); } }
        public ICommand ButtonExportCustomerClick { get { return new RelayCommand(ButtonExportCustomerExecute, CanExecute); } }

        public string CustomerName { get; set; }
        public string TextileName { get; set; }
        public DateTime CustomerDatePickerBegin { get; set; } = DateTime.Now.AddYears(-1);
        public DateTime CustomerDatePickerEnd { get; set; } = DateTime.Now;
        private void ButtonExportCustomerExecute()
        {
            IEnumerable<TrashCustomerShipped> trashCustomerShippedList = TrashModule.GetCustomerShippedList(CustomerName, CustomerDatePickerBegin, CustomerDatePickerEnd)
                .GroupBy(g => new { g.C_Name, g.I_03 })
                .Select(s => new TrashCustomerShipped { C_Name = s.Key.C_Name, I_03 = s.Key.I_03, Quantity = s.Sum(item => item.Quantity) });
            IEnumerable<TrashCustomerShipped> trashCustomerShippeds = string.IsNullOrEmpty(TextileName) ? trashCustomerShippedList.OrderBy(o => o.I_03) : trashCustomerShippedList.Where(w => w.I_03.Contains(TextileName)).OrderBy(o => o.I_03);

            ExcelFormat excelFormat = new ExcelFormat()
            {
                FileName = string.Concat(CustomerName, TextileName + "出貨紀錄"),
                ColumnFormats = new List<ColumnFormat>
                {
                     new ColumnFormat
                    {
                        CoiumnWidth = 3000,
                        ColumnTitle = "客戶名稱"
                    },
                    new ColumnFormat
                    {
                        CoiumnWidth = 5000,
                        ColumnTitle = "布種名稱"
                    },
                     new ColumnFormat
                    {
                        CoiumnWidth = 3000,
                        ColumnTitle = "重量"
                    }
                 }
            };

            var excelHelper = new ExcelHelper();
            excelHelper.CreateExcelFile<TrashCustomerShipped>(CreateCustomerShippedExcelAction, trashCustomerShippeds.ToList(), excelFormat);
        }

        private void CreateCustomerShippedExcelAction(IWorkbook wb, ISheet ws, ICellStyle positionStyle, ref int rowIndex, TrashCustomerShipped storeData)
        {
            ICellStyle estyle = wb.CreateCellStyle();
            estyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Yellow.Index;
            estyle.FillPattern = FillPattern.SolidForeground;

            ICellStyle a2style = wb.CreateCellStyle();
            a2style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Coral.Index;
            a2style.FillPattern = FillPattern.SolidForeground;

            XSSFRow rowTextile = (XSSFRow)ws.CreateRow(rowIndex);
            ExcelHelper.CreateCell(rowTextile, 0, storeData.C_Name, positionStyle);
            ExcelHelper.CreateCell(rowTextile, 1, storeData.I_03, positionStyle);
            ExcelHelper.CreateCell(rowTextile, 2, storeData.Quantity.ToString(), positionStyle);

            rowIndex++;
        }

        public DateTime ShippingCheckDate { get; set; } = DateTime.Now;

        public DateTime DatePickerBegin { get; set; } = DateTime.Now;
        public DateTime DatePickerEnd { get; set; } = DateTime.Now;

        public string FilterText { get; set; }


        private ObservableCollection<TrashShipped> _trashShippeds { get; set; }
        public ObservableCollection<TrashShipped> TrashShippeds
        {
            get { return _trashShippeds; }
            set { _trashShippeds = value; }
        }

        public ReportsViewModel()
        {
            _trashShippeds = new ObservableCollection<TrashShipped>();
        }


        private void ButtonExportExecute()
        {

            _trashShippeds.AddRange(TrashModule.GetTrashShippedList(DatePickerBegin, DatePickerEnd));
            #region OleDB
            //string databaseDirectory = AppSettingConfig.DbfFilePath();
            //string sql4 = "SELECT invosub.IN_DATE,invosub.I_01,item.I_03,SUM(invosub.QUANTITY) FROM INVOSUB.dbf invosub " +
            //                "INNER JOIN ITEM.dbf item ON invosub.I_01 = item.I_01 AND invosub.F_01 = item.F_01 " +
            //                "WHERE invosub.IN_DATE Between cDate('" + DatePickerBegin.ToString() + "') and cDate('" + DatePickerEnd.ToString() + "') " +
            //                "GROUP BY invosub.IN_DATE,invosub.I_01,item.I_03 " +
            //                "ORDER BY invosub.IN_DATE,invosub.I_01";
            //DataTable dt4 = GetOleDbDbfDataTable(databaseDirectory, sql4);
            //DataGridInCash_Copy3.ItemsSource = dt4.DefaultView; 
            #endregion
            var originalSources = new List<OriginalSource>();
            foreach (var shipped in _trashShippeds)
            {
                originalSources.Add(new OriginalSource
                {
                    DateTime = shipped.IN_DATE,
                    TextileNo = shipped.I_01,
                    TextileColorName = shipped.I_03,
                    Weight = shipped.Quantity
                });
            }

            var filterText = FilterText;
            var filterOriginalSources = string.IsNullOrEmpty(filterText) == true ? originalSources.Select(s => s.TextileNo).Distinct() : originalSources.Where(w => w.TextileColorName.Contains(filterText)).Select(s => s.TextileNo).Distinct(); ;
            var everyDate = originalSources.Select(s => s.DateTime).Distinct();
            var everyTextileNo = filterOriginalSources;
            var textileDatas = new List<TextileData>();
            foreach (var date in everyDate)
            {
                textileDatas.Add(new TextileData
                {
                    DateTime = date,
                    TextileColors = originalSources.Where(w => w.DateTime == date).Select(s => new TextileColor { TextileNo = s.TextileNo, Weight = s.Weight, TextileColorName = s.TextileColorName }).ToList()
                });
            }

            IWorkbook wb = new XSSFWorkbook();
            ISheet ws = wb.CreateSheet("Report");
            XSSFRow row = (XSSFRow)ws.CreateRow(0);

            var textileNameNumber = 1;
            ExcelHelper.CreateCell(row, 0, "時間", null);
            var priviousWeightList = new Dictionary<int, double>();
            foreach (var item in everyTextileNo)
            {
                ExcelHelper.CreateCell(row, textileNameNumber, originalSources.Where(w => w.TextileNo == item).First().TextileColorName, null);
                priviousWeightList.Add(item, 0);
                textileNameNumber++;
            }
            var rowNumber = 1;
            var priviousYear = textileDatas.First().DateTime.ToString("yyyy");
            foreach (var textiles in textileDatas)
            {
                var columnNumber = 1;
                XSSFRow rowTextile = (XSSFRow)ws.CreateRow(rowNumber);
                ExcelHelper.CreateCell(rowTextile, 0, textiles.DateTime.ToString("yyyy/MM/dd"), null);

                if (textiles.DateTime.ToString("yyyy") != priviousYear)
                {
                    priviousYear = textiles.DateTime.ToString("yyyy");
                    foreach (var item in everyTextileNo)
                    {
                        priviousWeightList[item] = 0;
                    }
                }
                foreach (var textileNo in everyTextileNo)
                {
                    var textileData = textiles.TextileColors.Where(w => w.TextileNo == textileNo);
                    var weight = textileData.Count() == 0 ? priviousWeightList[textileNo] : textileData.First().Weight + priviousWeightList[textileNo];
                    ExcelHelper.CreateCell(rowTextile, columnNumber, weight.ToString(), null);
                    priviousWeightList[textileNo] = weight;
                    columnNumber++;
                }
                rowNumber++;
            }

            FileStream file = new FileStream(string.Concat(AppSettingConfig.ReportsFilePath(), @"\", filterText, "報表", DatePickerBegin.ToString("yyyyMMdd"), "-", DatePickerEnd.ToString("yyyyMMdd"), ".xlsx"), FileMode.Create);//產生檔案
            wb.Write(file);
            file.Close();
        }

        private void ExportShippingCheckExecute()
        {
            List<StoreSearchData<StoreSearchColorDetail>> excelDailyShippedList = ExcelModule.GetExcelDailyShippedList(ShippingCheckDate);
            var trashShipped = TrashModule.GetTrashShippedList(ShippingCheckDate, ShippingCheckDate);

            var originalSources = new List<OriginalSource>();
            foreach (var shipped in trashShipped)
            {
                originalSources.Add(new OriginalSource
                {
                    DateTime = shipped.IN_DATE,
                    TextileNo = shipped.I_01,
                    TextileColorName = shipped.I_03,
                    Weight = shipped.Quantity
                });
            }
            var newList = new List<Container>();
            foreach (var originalSource in originalSources)
            {
                var priviousDistance = 10;
                var textileName = string.Empty;
                var textileColor = string.Empty;
                var shippedCount = 0;
                foreach (var excelDailyShippedItem in excelDailyShippedList)
                {
                    foreach (var colorDetail in excelDailyShippedItem.StoreSearchColorDetails)
                    {
                        var currentDistance = LevenshteinDistance(originalSource.TextileColorName, excelDailyShippedItem.TextileName, colorDetail.ColorName);
                        if (currentDistance < priviousDistance)
                        {
                            textileColor = colorDetail.ColorName;
                            textileName = excelDailyShippedItem.TextileName;
                            priviousDistance = currentDistance;
                            shippedCount = colorDetail.ShippedCount;
                        }
                    }
                }
                newList.Add(new Container()
                {
                    OriginalSource = originalSource,
                    TextileName = textileName,
                    ColorName = textileColor,
                    ShippedCount = shippedCount,
                    Distance = priviousDistance
                });
            }

            ExcelFormat columnFormats = new ExcelFormat()
            {
                FileName = "庫存對照清單",
                ColumnFormats = new List<ColumnFormat>
                {
                    new ColumnFormat
                    {
                        CoiumnWidth = 3000,
                        ColumnTitle = "布種顏色",
                    },
                    new ColumnFormat
                    {
                        CoiumnWidth = 2800,
                        ColumnTitle = "出貨重量",
                    },
                    new ColumnFormat
                    {
                        CoiumnWidth = 1850,
                        ColumnTitle = "約略出貨數",
                    },
                    new ColumnFormat
                    {
                        CoiumnWidth = 3000,
                        ColumnTitle = "布種名稱",
                    },
                    new ColumnFormat
                    {
                        CoiumnWidth = 1850,
                        ColumnTitle = "顏色",
                    },
                    new ColumnFormat
                    {
                        CoiumnWidth = 1850,
                        ColumnTitle = "出貨數量",
                    }
                }
            };

            var excelHelper = new ExcelHelper();
            excelHelper.CreateExcelFile<Container>(CreateShippingCheckExcelAction, newList.OrderBy(o => excelDailyShippedList.Select(s => s.TextileName).ToList().IndexOf(o.TextileName)).ToList(), columnFormats);
        }

        private void CreateShippingCheckExcelAction(IWorkbook wb, ISheet ws, ICellStyle positionStyle, ref int rowIndex, Container storeData)
        {
            ICellStyle estyle = wb.CreateCellStyle();
            estyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Yellow.Index;
            estyle.FillPattern = FillPattern.SolidForeground;

            ICellStyle a2style = wb.CreateCellStyle();
            a2style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Coral.Index;
            a2style.FillPattern = FillPattern.SolidForeground;

            var approximateNumber = storeData.OriginalSource.Weight / 20;
            var round = Math.Round(approximateNumber, 0, MidpointRounding.AwayFromZero);

            var isEqual = round == storeData.ShippedCount;

            XSSFRow rowTextile = (XSSFRow)ws.CreateRow(rowIndex);
            ExcelHelper.CreateCell(rowTextile, 0, storeData.OriginalSource.TextileColorName, GetStyle(wb, storeData.Distance) ?? positionStyle);
            ExcelHelper.CreateCell(rowTextile, 1, storeData.OriginalSource.Weight.ToString(), positionStyle);
            ExcelHelper.CreateCell(rowTextile, 2, (approximateNumber).ToString(), positionStyle);
            ExcelHelper.CreateCell(rowTextile, 3, storeData.TextileName, positionStyle);
            ExcelHelper.CreateCell(rowTextile, 4, storeData.ColorName, positionStyle);
            ExcelHelper.CreateCell(rowTextile, 5, storeData.ShippedCount.ToString(), isEqual ? positionStyle : estyle);

            rowIndex++;
        }


        private ICellStyle GetStyle(IWorkbook wb, int distence)
        {
            switch (distence)
            {
                case 0:
                    return null;
                case 1:
                    ICellStyle bstyle = wb.CreateCellStyle();
                    bstyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
                    bstyle.FillPattern = FillPattern.SolidForeground;
                    return bstyle;
                case 2:
                    ICellStyle hstyle = wb.CreateCellStyle();
                    hstyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.SkyBlue.Index;
                    hstyle.FillPattern = FillPattern.LessDots;
                    return hstyle;
                default:
                    ICellStyle a2style = wb.CreateCellStyle();
                    a2style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Coral.Index;
                    a2style.FillPattern = FillPattern.SolidForeground;
                    return a2style;
            }
        }
        public class Container
        {
            public OriginalSource OriginalSource { get; set; }
            public string TextileName { get; set; }
            public string ColorName { get; set; }
            public int ShippedCount { get; set; }
            public int Distance { get; set; }
        }
        public int LevenshteinDistance(string accountTextileColorName, string excelTextileName, string excelColorName)
        {

            string replaceTextileName = excelTextileName.Replace("采毓", "").Replace("佳隆", "");

            switch (excelTextileName)
            {
                case "搖粒布碼布":
                case "搖粒布配件":
                case "單刷布碼布":
                case "單刷布配件":
                    replaceTextileName = "SP+T2X2配件";
                    break;
                case "CVC大絨布配件碼布":
                case "CVC大絨布配件":
                case "30CVC細絨布碼布":
                case "30CVC細絨布配件":
                    replaceTextileName = "CVC2X2+OP配件";
                    break;
                case "C大絨碼布":
                case "C大絨配件":
                case "30C細絨布碼布":
                case "30C細絨布配件":
                    replaceTextileName = "20C2X2+OP配件";
                    break;
                default:
                    break;
            }

            string excelTextileColorName = string.Concat(replaceTextileName, excelColorName);
            int n = accountTextileColorName.Length;
            int m = excelTextileColorName.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (excelTextileColorName[j - 1] == accountTextileColorName[i - 1]) ? 0 : 1;

                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            return d[n, m];
        }


        #region OleDB
        //public static OleDbConnection OleDbDbfOpenConn(string DatabaseDirectory)
        //{
        //    string cnstr = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DatabaseDirectory + "; Extended Properties=dBASE IV;");
        //    OleDbConnection icn = new OleDbConnection();
        //    icn.ConnectionString = cnstr;
        //    if (icn.State == ConnectionState.Open) icn.Close();
        //    icn.Open();
        //    return icn;
        //}

        //public static DataTable GetOleDbDbfDataTable(string DatabaseDirectory, string OleDbString)
        //{
        //    DataTable myDataTable = new DataTable();
        //    OleDbConnection icn = OleDbDbfOpenConn(DatabaseDirectory);
        //    OleDbDataAdapter da = new OleDbDataAdapter(OleDbString, icn);
        //    DataSet ds = new DataSet();
        //    ds.Clear();
        //    da.Fill(ds);
        //    myDataTable = ds.Tables[0];
        //    if (icn.State == ConnectionState.Open) icn.Close();
        //    return myDataTable;
        //} 
        #endregion
    }
}
