using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.DataClass.ExcelDataClass
{
   public class ExcelSheetContent<T>
    {
        public string SheetName { get; set; }
        public List<ExcelCellContent>  ExcelColumnContents { get; set; }
        public List<T> ExcelRowContents { get; set; }

    }
}
