using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.DataClass.ProcessOrder
{
    public class ProcessOrderFlowDateDetail
    {
        public int OrderFlowDateNo { get; set; }
        public int OrderColorDetailNo { get; set; }
        public string Name { get; set; }
        public string Process { get; set; }
        public DateTime? InputDate { get; set; }
        public DateTime? CompleteDate { get; set; }
    }
}
