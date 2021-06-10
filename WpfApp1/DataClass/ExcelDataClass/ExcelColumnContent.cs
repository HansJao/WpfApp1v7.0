using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.DataClass.ExcelDataClass
{
    public class ExcelColumnContent
    {
        public string ColumnTitle { get; set; }
        public int ColumnWidth { get; set; }
        public int ColumnHeight { get; set; }
        public ICellStyle CellStyle { get; set; }
    }
}
