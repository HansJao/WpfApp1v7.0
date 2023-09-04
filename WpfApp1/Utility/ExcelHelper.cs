using Microsoft.Office.Interop.Excel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;
using WpfApp1.DataClass.Enumeration;
using WpfApp1.DataClass.ExcelDataClass;
using WpfApp1.DataClass.Fabric;
using WpfApp1.DataClass.StoreSearch;
using WpfApp1.Modules.ExcelModule;
using WpfApp1.Modules.ExcelModule.Implement;

namespace WpfApp1.Utility
{
    public class ExcelHelper
    {
        protected IExcelModule ExcelModule { get; } = new ExcelModule();
        public static void CreateCell(XSSFRow row, int cellIndex, string cellValue, ICellStyle style)
        {
            var cell = row.CreateCell(cellIndex);
            cell.SetCellValue(cellValue);
            cell.CellStyle = style;
            cell.SetCellType(CellType.String);
        }
        public static void CreateCell(XSSFRow row, int cellIndex, double cellValue, ICellStyle style)
        {
            var cell = row.CreateCell(cellIndex);
            cell.SetCellValue(cellValue);
            cell.CellStyle = style;
        }

        public static ICellStyle GetColorByStorageSpaces(IWorkbook wb, string storageSpaces)
        {
            //NPOI.SS.UserModel.IFont font = wb.CreateFont();
            //font.Color = IndexedColors.White.Index;

            switch (storageSpaces)
            {
                case var someVal when new Regex("(^1A)+").IsMatch(someVal):
                    ICellStyle style = wb.CreateCellStyle();
                    style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Orange.Index;
                    style.FillPattern = FillPattern.SolidForeground;
                    return style;
                case var someVal when new Regex("(^1B)+|(B小)").IsMatch(someVal):
                    ICellStyle bstyle = wb.CreateCellStyle();
                    bstyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.DarkYellow.Index;
                    bstyle.FillPattern = FillPattern.SolidForeground;
                    return bstyle;
                case var someVal when new Regex("(^1C)+").IsMatch(someVal):
                    ICellStyle cstyle = wb.CreateCellStyle();
                    cstyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Green.Index;
                    cstyle.FillPattern = FillPattern.SolidForeground;
                    return cstyle;
                case var someVal when new Regex("(^1D)+").IsMatch(someVal):
                    ICellStyle dstyle = wb.CreateCellStyle();
                    dstyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Teal.Index;
                    dstyle.FillPattern = FillPattern.SolidForeground;
                    return dstyle;
                case var someVal when new Regex("(^1E)+").IsMatch(someVal):
                    ICellStyle estyle = wb.CreateCellStyle();
                    estyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.BlueGrey.Index;
                    estyle.FillPattern = FillPattern.SolidForeground;
                    return estyle;
                case var someVal when new Regex("(^1F)+").IsMatch(someVal):
                    ICellStyle fstyle = wb.CreateCellStyle();
                    fstyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey50Percent.Index;
                    fstyle.FillPattern = FillPattern.SolidForeground;
                    return fstyle;
                case var someVal when new Regex("(^1G)+|(G小)").IsMatch(someVal):
                    ICellStyle gstyle = wb.CreateCellStyle();
                    gstyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Red.Index;
                    gstyle.FillPattern = FillPattern.SolidForeground;
                    return gstyle;
                case var someVal when new Regex("(^2E)+|(H小)").IsMatch(someVal):
                    ICellStyle hstyle = wb.CreateCellStyle();
                    hstyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightOrange.Index;
                    hstyle.FillPattern = FillPattern.SolidForeground;
                    return hstyle;
                case var someVal when new Regex("(^1I)+").IsMatch(someVal):
                    ICellStyle istyle = wb.CreateCellStyle();
                    istyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Lime.Index;
                    istyle.FillPattern = FillPattern.SolidForeground;
                    return istyle;
                case var someVal when new Regex("(^1J)+").IsMatch(someVal):
                    ICellStyle jstyle = wb.CreateCellStyle();
                    jstyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.SeaGreen.Index;
                    jstyle.FillPattern = FillPattern.SolidForeground;
                    return jstyle;
                case var someVal when new Regex("(^1K)+").IsMatch(someVal):
                    ICellStyle kstyle = wb.CreateCellStyle();
                    kstyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Aqua.Index;
                    kstyle.FillPattern = FillPattern.SolidForeground;
                    return kstyle;
                case var someVal when new Regex("(^L)+").IsMatch(someVal):
                    ICellStyle lstyle = wb.CreateCellStyle();
                    lstyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightBlue.Index;
                    lstyle.FillPattern = FillPattern.SolidForeground;
                    return lstyle;
                case var someVal when new Regex("(^1M)+|(M小)").IsMatch(someVal):
                    ICellStyle mstyle = wb.CreateCellStyle();
                    mstyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Pink.Index;
                    mstyle.FillPattern = FillPattern.SolidForeground;
                    return mstyle;
                case var someVal when new Regex("(^1N)+|(N小)").IsMatch(someVal):
                    ICellStyle nstyle = wb.CreateCellStyle();
                    nstyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Gold.Index;
                    nstyle.FillPattern = FillPattern.SolidForeground;
                    return nstyle;
                case var someVal when new Regex("(^1O)+").IsMatch(someVal):
                    ICellStyle ostyle = wb.CreateCellStyle();
                    ostyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Lavender.Index;
                    ostyle.FillPattern = FillPattern.SolidForeground;
                    return ostyle;
                case var someVal when new Regex("(^1P)+").IsMatch(someVal):
                    ICellStyle pstyle = wb.CreateCellStyle();
                    pstyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Yellow.Index;
                    pstyle.FillPattern = FillPattern.SolidForeground;
                    return pstyle;
                case var someVal when new Regex("(^1Q)+|(Q小)").IsMatch(someVal):
                    ICellStyle qstyle = wb.CreateCellStyle();
                    qstyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightYellow.Index;
                    qstyle.FillPattern = FillPattern.SolidForeground;
                    return qstyle;
                case var someVal when new Regex("(^1R)+").IsMatch(someVal):
                    ICellStyle rstyle = wb.CreateCellStyle();
                    rstyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Turquoise.Index;
                    rstyle.FillPattern = FillPattern.SolidForeground;
                    return rstyle;
                case var someVal when new Regex("(^1S)+").IsMatch(someVal):
                    ICellStyle sstyle = wb.CreateCellStyle();
                    sstyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
                    sstyle.FillPattern = FillPattern.SolidForeground;
                    return sstyle;
                case var someVal when new Regex("(^1T)+").IsMatch(someVal):
                    ICellStyle tstyle = wb.CreateCellStyle();
                    tstyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Rose.Index;
                    tstyle.FillPattern = FillPattern.SolidForeground;
                    return tstyle;
                case var someVal when new Regex("(^2A)+").IsMatch(someVal):
                    ICellStyle a2style = wb.CreateCellStyle();
                    a2style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Tan.Index;
                    a2style.FillPattern = FillPattern.SolidForeground;
                    return a2style;
                case var someVal when new Regex("(^2B)+").IsMatch(someVal):
                    ICellStyle b2style = wb.CreateCellStyle();
                    b2style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightGreen.Index;
                    b2style.FillPattern = FillPattern.SolidForeground;
                    return b2style;
                case var someVal when new Regex("(^2C)+").IsMatch(someVal):
                    ICellStyle c2style = wb.CreateCellStyle();
                    c2style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightTurquoise.Index;
                    c2style.FillPattern = FillPattern.SolidForeground;
                    return c2style;
                case var someVal when new Regex("(^2D)+").IsMatch(someVal):
                    ICellStyle d2style = wb.CreateCellStyle();
                    d2style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.PaleBlue.Index;
                    d2style.FillPattern = FillPattern.SolidForeground;
                    return d2style;
                case var someVal when new Regex("(^A)+").IsMatch(someVal):
                    ICellStyle oastyle = wb.CreateCellStyle();
                    oastyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Lavender.Index;
                    oastyle.FillPattern = FillPattern.SolidForeground;
                    oastyle.BorderTop = BorderStyle.MediumDashDot;
                    oastyle.BorderRight = BorderStyle.MediumDashDot;
                    oastyle.BorderLeft = BorderStyle.MediumDashDot;
                    oastyle.BorderBottom = BorderStyle.MediumDashDot;
                    return oastyle;
                case var someVal when new Regex("(^B)+").IsMatch(someVal):
                    ICellStyle obstyle = wb.CreateCellStyle();
                    obstyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.CornflowerBlue.Index;
                    obstyle.FillPattern = FillPattern.SolidForeground;
                    obstyle.BorderTop = BorderStyle.MediumDashDot;
                    obstyle.BorderRight = BorderStyle.MediumDashDot;
                    obstyle.BorderLeft = BorderStyle.MediumDashDot;
                    obstyle.BorderBottom = BorderStyle.MediumDashDot;
                    return obstyle;
                case var someVal when new Regex("(^菜)+").IsMatch(someVal):
                    ICellStyle vestyle = wb.CreateCellStyle();
                    vestyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LemonChiffon.Index;
                    vestyle.FillPattern = FillPattern.SolidForeground;
                    vestyle.BorderTop = BorderStyle.MediumDashDot;
                    vestyle.BorderRight = BorderStyle.MediumDashDot;
                    vestyle.BorderLeft = BorderStyle.MediumDashDot;
                    vestyle.BorderBottom = BorderStyle.MediumDashDot;
                    return vestyle;
                default:
                    ICellStyle defaultstyle = wb.CreateCellStyle();
                    return defaultstyle;
            }
        }
        public static T CheckExcelCellType<T>(CellType cellType, ICell cell)
        {
            switch (cellType)
            {
                case CellType.Unknown:
                    return default(T);
                case CellType.Numeric:
                    if (cell == null)
                    {
                        return (T)Convert.ChangeType(0, typeof(T));
                    }
                    else if (cell.CellType == cellType)
                    {
                        return (T)Convert.ChangeType(cell.NumericCellValue, typeof(T));
                    }
                    else if (cell.CellType == CellType.Blank)
                    {
                        return default(T);
                    }
                    else
                    {
                        return (T)Convert.ChangeType(0, typeof(T));
                    }
                case CellType.String:
                    if (cell == null)
                    {
                        return (T)Convert.ChangeType("null", typeof(T));
                    }
                    else if (cell.CellType == cellType)
                    {
                        return (T)Convert.ChangeType(cell.StringCellValue, typeof(T)); ;
                    }
                    else if (cell.CellType == CellType.Blank)
                    {
                        return default(T);
                    }
                    else if (cell.CellType == CellType.Numeric)
                    {
                        return (T)Convert.ChangeType(cell.NumericCellValue.ToString(), typeof(T));
                    }
                    else
                    {
                        return (T)Convert.ChangeType("Unknown", typeof(T));
                    }
                case CellType.Formula:
                    return default(T);
                case CellType.Blank:
                    return default(T);
                case CellType.Boolean:
                    return default(T);
                case CellType.Error:
                    return default(T);
                default:
                    return default(T);
            }
        }

        public delegate List<T> ReadExcelAction<T>(List<T> list, IRow row, string sheetName, int timeRange);
        public delegate void CreateExcelAction<T>(IWorkbook wb, ISheet ws, ICellStyle positionStyle, ref int rowIndex, T storeData);

        public List<string> GetExcelSheetName()
        {
            IWorkbook workbook = null;  //新建IWorkbook對象  
            string fileName = string.Concat(AppSettingConfig.FilePath(), "/", AppSettingConfig.StoreManageFileName());
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            workbook = new XSSFWorkbook(fileStream);  //xlsx數據讀入workbook
            var sheetListName = new List<string>();
            for (int sheetCount = 1; sheetCount < workbook.NumberOfSheets; sheetCount++)
            {
                var sheetName = workbook.GetSheetName(sheetCount);  //獲取第i個工作表
                sheetListName.Add(sheetName);
            }
            return sheetListName;
        }


        /// <summary>
        /// 讀取Excel後匯出Excel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="readExcelAction"></param>
        /// <param name="createExcelAction"></param>
        /// <param name="timeRange"></param>
        /// <param name="excelFormat"></param>
        public void InventoryCheckSheet<T>(ReadExcelAction<T> readExcelAction, CreateExcelAction<T> createExcelAction, int timeRange, ExcelFormat excelFormat)
        {
            IWorkbook workbook = null;  //新建IWorkbook對象  
            string fileName = string.Concat(AppSettingConfig.FilePath(), "/", AppSettingConfig.StoreManageFileName());
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            workbook = new XSSFWorkbook(fileStream);  //xlsx數據讀入workbook
            var list = new List<T>();
            for (int sheetCount = 1; sheetCount < workbook.NumberOfSheets; sheetCount++)
            {
                ISheet sheet = workbook.GetSheetAt(sheetCount);  //獲取第i個工作表  
                IRow row;
                var firstRow = sheet.GetRow(0);

                for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++)  //對工作表每一行  
                {
                    if (rowIndex > 70)
                        break;
                    row = sheet.GetRow(rowIndex);   //row讀入第i行數據  

                    if (row != null)
                    {
                        //該筆資料沒有顏色則不讀取該Row
                        if (row.GetCell(1) == null)
                        {
                            break;
                        }
                        readExcelAction(list, row, sheet.SheetName, timeRange);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            CreateExcelFile(createExcelAction, list, excelFormat);
        }

        public void CreateExcelFile<T>(CreateExcelAction<T> createExcelAction, List<T> list, ExcelFormat excelFormat)
        {
            //建立Excel 2003檔案
            IWorkbook wb = new XSSFWorkbook();
            ISheet ws = wb.CreateSheet("Class");
            XSSFRow row = (XSSFRow)ws.CreateRow(0);
            row.Height = 440;

            ICellStyle positionStyle = wb.CreateCellStyle();
            positionStyle.WrapText = true;
            positionStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            positionStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;

            foreach (var item in excelFormat.ColumnFormats)
            {
                ws.SetColumnWidth(excelFormat.ColumnFormats.IndexOf(item), item.CoiumnWidth);
                CreateCell(row, excelFormat.ColumnFormats.IndexOf(item), item.ColumnTitle, positionStyle);
            }

            int rowIndex = 1;
            foreach (var storeData in list)
            {
                createExcelAction(wb, ws, positionStyle, ref rowIndex, storeData);
            }
            FileStream file = new FileStream(string.Concat(AppSettingConfig.FilePath(), @"\", excelFormat.FileName, DateTime.Now.ToString("yyyyMMdd"), ".xlsx"), FileMode.Create);//產生檔案
            wb.Write(file);
            file.Close();
        }

        public void CreateExcelFile(IWorkbook wb, ExcelContent excelContent)
        {
            foreach (var excelSheet in excelContent.ExcelSheetContents)
            {
                ISheet ws = wb.CreateSheet(excelSheet.SheetName);
                ws.SetMargin(MarginType.LeftMargin, excelSheet.LeftMargin);
                ws.SetMargin(MarginType.RightMargin, excelSheet.RightMargin);
                ws.SetMargin(MarginType.TopMargin, excelSheet.TopMargin);
                ws.SetMargin(MarginType.BottomMargin, excelSheet.BottomMargin);
                XSSFRow row = (XSSFRow)ws.CreateRow(0);
                row.Height = excelSheet.ColumnHeight;
                foreach (ExcelColumnContent columnContent in excelSheet.ExcelColumnContents)
                {
                    if (columnContent.Width != 0)
                        ws.SetColumnWidth(excelSheet.ExcelColumnContents.ToList().IndexOf(columnContent), columnContent.Width);
                    CreateCell(row, excelSheet.ExcelColumnContents.ToList().IndexOf(columnContent), columnContent.CellValue, columnContent.CellStyle);
                }

                int rowIndex = 1;
                foreach (ExcelRowContent rowContent in excelSheet.ExcelRowContents)
                {
                    XSSFRow rowTextile = (XSSFRow)ws.CreateRow(rowIndex);
                    rowTextile.Height = rowContent.Height;
                    for (int cellIndex = 0; cellIndex < rowContent.ExcelCellContents?.Count; cellIndex++)
                    {
                        var cellValue = rowContent.ExcelCellContents.ElementAt(cellIndex).CellValue;
                        if (int.TryParse(cellValue, out int cellIntValue))
                        {
                            CreateCell(rowTextile, cellIndex, cellIntValue, rowContent.ExcelCellContents.ElementAt(cellIndex).CellStyle);
                        }
                        else if (double.TryParse(cellValue, out double cellDoubleValue) && !cellValue.Contains("E"))
                        {
                            CreateCell(rowTextile, cellIndex, cellDoubleValue, rowContent.ExcelCellContents.ElementAt(cellIndex).CellStyle);
                        }
                        else
                        {
                            CreateCell(rowTextile, cellIndex, cellValue, rowContent.ExcelCellContents.ElementAt(cellIndex).CellStyle);
                        }
                        if (rowContent.ExcelCellContents.ElementAt(cellIndex).CellRangeAddress != null)
                            ws.AddMergedRegion(rowContent.ExcelCellContents.ElementAt(cellIndex).CellRangeAddress);
                    }
                    rowIndex++;
                }
            }

            FileStream file = new FileStream(string.Concat(AppSettingConfig.FilePath(), @"\", excelContent.FileName, ".xlsx"), FileMode.Create);//產生檔案
            wb.Write(file);
            file.Close();
        }

        public static string GetCellString(IRow row, int cellNum)
        {
            row.GetCell(cellNum).SetCellType(CellType.String);
            string result = row.GetCell(cellNum).StringCellValue;
            return result;
        }

        public List<TextileColorInventory> GetInventoryData(IWorkbook workbook, string sheetName)
        {
            List<TextileColorInventory> selectedTextiles = new List<TextileColorInventory>();

            ISheet sheet = workbook.GetSheet(sheetName);  //獲取工作表
            if (sheet == null) return null;
            IRow row;

            for (int sheetRowNum = 1; sheetRowNum <= sheet.LastRowNum; sheetRowNum++)
            {
                row = sheet.GetRow(sheetRowNum);
                if (row == null)
                {
                    break;
                }
                var differentCylinder = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.DifferentCylinder) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.DifferentCylinder).CellType == CellType.Blank ? "" : "有不同缸應注意";
                int cellValue = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.CountInventory) == null || (row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.CountInventory).CellType == CellType.Formula ? row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.CountInventory).CachedFormulaResultType == CellType.Error : false)
                    ? 0
                    : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.CountInventory).NumericCellValue.ToInt();

                selectedTextiles.Add(new TextileColorInventory
                {
                    Index = CheckExcelCellType<string>(CellType.String, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.Index)),
                    ColorName = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ColorName) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ColorName).ToString(),
                    StorageSpaces = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.StorageSpaces) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.StorageSpaces).ToString(),
                    Inventory = CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.Inventory)),
                    DifferentCylinder = CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.DifferentCylinder)),
                    ShippingDate1 = CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate1)),
                    ShippingDate2 = CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate2)),
                    ShippingDate3 = CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate3)),
                    ShippingDate4 = CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate4)),
                    ShippingDate5 = CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate5)),
                    ShippingDate6 = CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate6)),
                    ShippingDate7 = CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate7)),
                    ShippingDate8 = CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate8)),
                    ShippingDate9 = CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate9)),
                    CountInventory = cellValue,
                    IsChecked = CheckExcelCellType<string>(CellType.String, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.IsChecked)),
                    CheckDate = CheckExcelCellType<string>(CellType.String, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.CheckDate)),
                    TextileFactory = new ExcelCell
                    {
                        CellValue = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.FabricFactory) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.FabricFactory).ToString(),
                        FontColor = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.FabricFactory) == null ? Brushes.Black : GetFontColor(row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.FabricFactory).CellStyle.FontIndex),
                    },
                    ClearFactory = new ExcelCell
                    {
                        CellValue = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ClearFactory) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ClearFactory).ToString(),
                        FontColor = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ClearFactory) == null ? Brushes.Black : GetFontColor(row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ClearFactory).CellStyle.FontIndex),
                    },
                    Memo = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.Memo) == null ? differentCylinder : string.Concat(row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.Memo).ToString(), ",", differentCylinder)
                });
            }
            return selectedTextiles;
        }


        public static SolidColorBrush GetFontColor(short fontIndex)
        {
            switch (fontIndex)
            {
                case 2:
                case 32:
                    return Brushes.Red;
                case 3:
                case 8:
                case 35:
                case 36:
                    return Brushes.Blue;
                default:
                    return Brushes.Black;
            }
        }
    }
}
