using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.DataClass.Entity
{
    public class Fabric
    {
        public int FabricID { get; set; }
        public string FabricName { get; set; }
        public int AverageUnitPrice { get; set; }
        public int AverageCost { get; set; }
        public DateTime UpdateDate { get; set; }

    }
}
