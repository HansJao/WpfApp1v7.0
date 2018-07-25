using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.DataClass.Entity
{
    public class ProcessOrderFlow
    {
        public int OrderDetailNo { get; set; }
        public int OrderNo { get; set; }
        public int FactoryID { get; set; }
        public DateTime? InputDate { get; set; }
        public DateTime? CompleteDate { get; set; }
    }
}
