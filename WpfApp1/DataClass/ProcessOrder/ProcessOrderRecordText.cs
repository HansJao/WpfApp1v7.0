using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.DataClass.ProcessOrder
{
    public class ProcessOrderRecordText
    {
        public DateTime DateTime { get; set; }
        public IEnumerable<int> OrderNos { get; set; }
    }
}
