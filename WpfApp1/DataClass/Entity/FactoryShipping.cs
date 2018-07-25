using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.DataClass.Entity
{
    public class FactoryShipping
    {
        public int ShippingNo { get; set; }
        public int OrderColorDetailNo { get; set; }
        public int CustomerID { get; set; }
        public int Quantity { get; set; }
        public DateTime CreateDate { get; set; }

    }
}
