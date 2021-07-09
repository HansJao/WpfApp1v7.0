using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.DataClass.ExcelDataClass
{
    public class ExcelRowContent
    {
        public short Height { get; set; }
        public List<ExcelCellContent> ExcelCellContents { get; set; }
    }
}
