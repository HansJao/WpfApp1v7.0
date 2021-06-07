using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.DataClass.ProcessOrder
{
    public class ProcessOrderDelivery
    {
        public string FactoryName { get; set; }
        public string StorageSpace { get; set; }
        public int StorageNumber { get; set; }
        public string OrderString { get; set; }
        public string Fabric { get; set; }
        public string Color { get; set; }
        public int Number { get; set; }
    }
}
