using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.Enumeration;

namespace WpfApp1.DataClass.Entity
{
    public class ProcessOrderStatus
    {
        public int OrderNo { get; set; }
        public string OrderString { get; set; }
        public string Fabric { get; set; }
        public string Color { get; set; }
        public int Quantity { get; set; }
        public ProcessOrderColorStatus Status { get; set; }
        public DateTime UpdateDate { get; set; }
        public string FactoryName { get; set; }
        public int OrderDetailNo { get; set; }
    }
}
