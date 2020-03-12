using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.DataClass.Entity
{
    public class ProcessOrderStatus
    {
        public string OrderString { get; set; }
        public string Fabric { get; set; }
        public string Color { get; set; }
        public int Quantity { get; set; }
        public int Status { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
