using Microsoft.Office.Interop.Excel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        }

        public static ICellStyle GetColorByStorageSpaces(IWorkbook wb, string storageSpaces)
        {
            switch (GetStoregeSpacesWord(storageSpaces))
            {
                case "1A":
                    ICellStyle style = wb.CreateCellStyle();
                    style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Tan.Index;
                    style.FillPattern = FillPattern.LessDots;
                    return style;
                case "1B":
                    ICellStyle bstyle = wb.CreateCellStyle();
                    bstyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
                    bstyle.FillPattern = FillPattern.SolidForeground;
                    return bstyle;
                case "1C":
                    ICellStyle cstyle = wb.CreateCellStyle();
                    cstyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Pink.Index;
                    cstyle.FillPattern = FillPattern.LeastDots;
                    return cstyle;
                case "1D":
                    ICellStyle dstyle = wb.CreateCellStyle();
                    dstyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Gold.Index;
                    dstyle.FillPattern = FillPattern.SolidForeground;
                    return dstyle;
                case "E":
                    ICellStyle estyle = wb.CreateCellStyle();
                    estyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Yellow.Index;
                    estyle.FillPattern = FillPattern.SolidForeground;
                    return estyle;
                case "F":
                    ICellStyle fstyle = wb.CreateCellStyle();
                    fstyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.BrightGreen.Index;
                    fstyle.FillPattern = FillPattern.ThinForwardDiagonals;
                    return fstyle;
                case "G":
                    ICellStyle gstyle = wb.CreateCellStyle();
                    gstyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Turquoise.Index;
                    gstyle.FillPattern = FillPattern.ThinBackwardDiagonals;
                    return gstyle;
                case "H":
                    ICellStyle hstyle = wb.CreateCellStyle();
                    hstyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.SkyBlue.Index;
                    hstyle.FillPattern = FillPattern.LessDots;
                    return hstyle;
                case "I":
                    ICellStyle istyle = wb.CreateCellStyle();
                    istyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Rose.Index;
                    istyle.FillPattern = FillPattern.SolidForeground;
                    return istyle;
                case "J":
                    ICellStyle jstyle = wb.CreateCellStyle();
                    jstyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Tan.Index;
                    jstyle.FillPattern = FillPattern.SolidForeground;
                    return jstyle;
                case "K":
                    ICellStyle kstyle = wb.CreateCellStyle();
                    kstyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightYellow.Index;
                    kstyle.FillPattern = FillPattern.SolidForeground;
                    return kstyle;
                case "L":
                    ICellStyle lstyle = wb.CreateCellStyle();
                    lstyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightGreen.Index;
                    lstyle.FillPattern = FillPattern.SolidForeground;
                    return lstyle;
                case "M":
                    ICellStyle mstyle = wb.CreateCellStyle();
                    mstyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightTurquoise.Index;
                    mstyle.FillPattern = FillPattern.SolidForeground;
                    return mstyle;
                case "N":
                    ICellStyle nstyle = wb.CreateCellStyle();
                    nstyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.PaleBlue.Index;
                    nstyle.FillPattern = FillPattern.SolidForeground;
                    return nstyle;
                case "O":
                    ICellStyle ostyle = wb.CreateCellStyle();
                    ostyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Lavender.Index;
                    ostyle.FillPattern = FillPattern.SolidForeground;
                    return ostyle;
                case "P":
                    ICellStyle pstyle = wb.CreateCellStyle();
                    pstyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.CornflowerBlue.Index;
                    pstyle.FillPattern = FillPattern.SolidForeground;
                    return pstyle;
                case "Q":
                    ICellStyle qstyle = wb.CreateCellStyle();
                    qstyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LemonChiffon.Index;
                    qstyle.FillPattern = FillPattern.SolidForeground;
                    return qstyle;
                case "2A":
                    ICellStyle a2style = wb.CreateCellStyle();
                    a2style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Coral.Index;
                    a2style.FillPattern = FillPattern.SolidForeground;
                    return a2style;
                case "2B":
                    ICellStyle b2style = wb.CreateCellStyle();
                    b2style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightCornflowerBlue.Index;
                    b2style.FillPattern = FillPattern.SolidForeground;
                    return b2style;
                case "2C":
                    ICellStyle c2style = wb.CreateCellStyle();
                    c2style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.SeaGreen.Index;
                    c2style.FillPattern = FillPattern.ThinBackwardDiagonals;
                    return c2style;
                case "2D":
                    ICellStyle d2style = wb.CreateCellStyle();
                    d2style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightOrange.Index;
                    d2style.FillPattern = FillPattern.SolidForeground;
                    return d2style;
                default:
                    ICellStyle defaultstyle = wb.CreateCellStyle();
                    return defaultstyle;
            }
        }

        public static string GetStoregeSpacesWord(string storageSpaces)
        {
            storageSpaces = storageSpaces ?? "";
            if (storageSpaces.ToUpper().Contains("1A"))
            {
                return "1A";
            }
            else if (storageSpaces.ToUpper().Contains("1B"))
            {
                return "1B";
            }
            else if (storageSpaces.ToUpper().Contains("1C"))
            {
                return "1C";
            }
            else if (storageSpaces.ToUpper().Contains("1D"))
            {
                return "1D";
            }
            else if (storageSpaces.ToUpper().Contains("E"))
            {
                return "E";
            }
            else if (storageSpaces.ToUpper().Contains("F"))
            {
                return "F";
            }
            else if (storageSpaces.ToUpper().Contains("G"))
            {
                return "G";
            }
            else if (storageSpaces.ToUpper().Contains("H"))
            {
                return "H";
            }
            else if (storageSpaces.ToUpper().Contains("I"))
            {
                return "I";
            }
            else if (storageSpaces.ToUpper().Contains("J"))
            {
                return "J";
            }
            else if (storageSpaces.ToUpper().Contains("K"))
            {
                return "K";
            }
            else if (storageSpaces.ToUpper().Contains("L"))
            {
                return "L";
            }
            else if (storageSpaces.ToUpper().Contains("M"))
            {
                return "M";
            }
            else if (storageSpaces.ToUpper().Contains("N"))
            {
                return "N";
            }
            else if (storageSpaces.ToUpper().Contains("O"))
            {
                return "O";
            }
            else if (storageSpaces.ToUpper().Contains("P"))
            {
                return "P";
            }
            else if (storageSpaces.ToUpper().Contains("Q"))
            {
                return "Q";
            }
            else if (storageSpaces.ToUpper().Contains("2A"))
            {
                return "2A";
            }
            else if (storageSpaces.ToUpper().Contains("2B"))
            {
                return "2B";
            }
            else if (storageSpaces.ToUpper().Contains("2C"))
            {
                return "2C";
            }
            else if (storageSpaces.ToUpper().Contains("2D"))
            {
                return "2D";
            }

            return "";
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

            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                row = sheet.GetRow(i);
                if (row == null)
                {
                    break;
                }
                var differentCylinder = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.DifferentCylinder) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.DifferentCylinder).CellType == CellType.Blank ? "" : "有不同缸應注意";
                var cellValue = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.CountInventory) == null || (row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.CountInventory).CellType == CellType.Formula ? row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.CountInventory).CachedFormulaResultType == CellType.Error : false)
                    ? ""
                    : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.CountInventory).NumericCellValue.ToString();

                double inventory = ExcelModule.CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.Inventory));
                selectedTextiles.Add(new TextileColorInventory
                {
                    Index = ExcelModule.CheckExcelCellType<string>(CellType.String, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.Index)),
                    ColorName = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ColorName) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ColorName).ToString(),
                    StorageSpaces = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.StorageSpaces) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.StorageSpaces).ToString(),
                    Inventory = inventory,
                    DifferentCylinder = ExcelModule.CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.DifferentCylinder)),
                    ShippingDate1 = ExcelModule.CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate1)),
                    ShippingDate2 = ExcelModule.CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate2)),
                    ShippingDate3 = ExcelModule.CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate3)),
                    ShippingDate4 = ExcelModule.CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate4)),
                    ShippingDate5 = ExcelModule.CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate5)),
                    ShippingDate6 = ExcelModule.CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate6)),
                    ShippingDate7 = ExcelModule.CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate7)),
                    ShippingDate8 = ExcelModule.CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate8)),
                    ShippingDate9 = ExcelModule.CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate9)),
                    CountInventory = cellValue,
                    IsChecked = ExcelModule.CheckExcelCellType<string>(CellType.String, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.IsChecked)),
                    CheckDate = ExcelModule.CheckExcelCellType<string>(CellType.String, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.CheckDate)),
                    TextileFactory = new ExcelCell
                    {
                        CellValue = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.FabricFactory) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.FabricFactory).ToString(),
                        FontColor = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.FabricFactory) == null ? Brushes.Black : ExcelHelper.GetFontColor(row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.FabricFactory).CellStyle.FontIndex),
                    },
                    ClearFactory = new ExcelCell
                    {
                        CellValue = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ClearFactory) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ClearFactory).ToString(),
                        FontColor = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ClearFactory) == null ? Brushes.Black : ExcelHelper.GetFontColor(row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ClearFactory).CellStyle.FontIndex),
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
