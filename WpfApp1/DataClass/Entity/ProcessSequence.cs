using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.Enumeration;

namespace WpfApp1.DataClass.Entity
{
    public class ProcessSequence
    {
        public int SequenceNo { get; set; }
        public int FabricID { get; set; }
        public string ColorNoString { get; set; }
        public ProcessItem ProcessItem { get; set; }
        public float Loss { get; set; }
        public int WorkPay { get; set; }
        public int Order { get; set; }
        public int Group { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
