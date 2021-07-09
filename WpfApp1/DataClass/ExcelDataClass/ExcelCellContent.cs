using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.DataClass.ExcelDataClass
{
    public class ExcelCellContent
    {
        public string CellValue { get; set; }
        public ICellStyle CellStyle { get; set; }
        public CellRangeAddress CellRangeAddress { get; set; }
    }
}
