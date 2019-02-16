using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.DataClass.ExcelDataClass
{
    public class ExcelFormat
    {
        public string FileName { get; set; }
        public List<ColumnFormat> ColumnFormats { get; set; }
    }
}
