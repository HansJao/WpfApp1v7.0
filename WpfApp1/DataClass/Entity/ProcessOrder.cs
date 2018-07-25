using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.DataClass.Entity
{
    public class ProcessOrder
    {
        public int OrderNo { get; set; }
        public string OrderString { get; set; }
        public string Fabric { get; set; }
        public string Specification { get; set; }
        public string ProcessItem { get; set; }
        public string Precautions { get; set; }
        public string Memo { get; set; }
        public string HandFeel { get; set; }
        public DateTime CreateDate { get; set; }

    }
}
