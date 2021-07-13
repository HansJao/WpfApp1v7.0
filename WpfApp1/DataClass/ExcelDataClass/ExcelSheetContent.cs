using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.DataClass.ExcelDataClass
{
    public class ExcelSheetContent
    {
        public string SheetName { get; set; }
        public double LeftMargin { get; set; }
        public double RightMargin { get; set; }
        public double TopMargin { get; set; }
        public double BottomMargin { get; set; }
        public short ColumnHeight { get; set; } = 440;
        public List<ExcelColumnContent> ExcelColumnContents { get; set; }
        public List<ExcelRowContent> ExcelRowContents { get; set; }

    }
}
