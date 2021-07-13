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
        public string CellValue { get; set; }
        public int Width { get; set; }
        public ICellStyle CellStyle { get; set; }
    }
}
