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
        public List<ExcelCellContent>  ExcelColumnContents { get; set; }
        public List<List<ExcelCellContent>> ExcelRowContents { get; set; }

    }
}
