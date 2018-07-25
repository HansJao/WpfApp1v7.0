using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.DataClass.Entity
{
    public class ProcessOrderFlowDate
    {
        public int OrderFlowDateNo { get; set; }
        public int OrderColorDetailNo { get; set; }
        public int OrderFlowNo { get; set; }
        public DateTime? InputDate { get; set; }
        public DateTime? CompleteDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
