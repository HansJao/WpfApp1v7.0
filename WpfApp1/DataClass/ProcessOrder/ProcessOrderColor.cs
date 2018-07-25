using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.Enumeration;

namespace WpfApp1.DataClass.ProcessOrder
{
    public class ProcessOrderColor
    {
        public string Color { get; set; }
        public string ColorNumber { get; set; }
        public int Quantity { get; set; }
        public ProcessOrderColorStatus Status { get; set; }
    }
}
