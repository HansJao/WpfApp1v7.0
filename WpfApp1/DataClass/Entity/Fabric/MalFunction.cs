using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.Enumeration;

namespace WpfApp1.DataClass.Entity
{
    public class MalFunction
    {
        public int MalFunctionID { get; set; }
        public int CustomerID { get; set; }
        public int FactoryID { get; set; }
        public ProcessItem RepaireItem { get; set; }
        public MalFunctionEnum Status { get; set; }
        public int ColorNo { get; set; }
        public int RepairOrderNo { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; }
        public DateTime MalFunctionTime { get; set; } = DateTime.Now;
        public DateTime CreateDate { get; set; } = DateTime.Now;
    }
}
