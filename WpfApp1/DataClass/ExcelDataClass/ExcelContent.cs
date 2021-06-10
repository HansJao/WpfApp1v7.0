using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.DataClass.ExcelDataClass
{
    public class ExcelContent<T>
    {
        public string FileName { get; set; }
        public List<ExcelSheetContent<T>> ExcelSheetContents { get; set; }
    }
}
