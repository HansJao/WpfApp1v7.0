using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.DataClass.Entity.ProcessOrderFile
{
    public class CustomerOrderRelate
    {
        public int CustomerOrderID { get; set; }
        public int ProcessOrderID { get; set; }
        public int CustomerID { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
