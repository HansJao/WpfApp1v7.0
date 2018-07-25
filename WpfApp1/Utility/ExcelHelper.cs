using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Utility
{
    public class ExcelHelper
    {
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
    }
}
