using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.DataClass.ProcessOrder
{
    public class FactoryShippingName
    {
        public int ShippingNo { get; set; }
        public int OrderColorDetailNo { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
