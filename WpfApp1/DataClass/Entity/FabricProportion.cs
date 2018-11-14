using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.DataClass.Entity
{
    public class FabricProportion
    {
        public int ProportionNo { get; set; }
        public int ColorNo { get; set; }
        public int YarnPriceNo { get; set; }
        public decimal Proportion { get; set; }
        public int Group { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
