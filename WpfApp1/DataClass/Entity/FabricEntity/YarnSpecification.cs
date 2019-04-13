using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.DataClass.Entity.FabricEntity
{
    public class YarnSpecification
    {
        public int YarnSpecificationNo { get; set; }
        public string Ingredient { get; set; }
        public string YarnCount { get; set; }
        public string Color { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
