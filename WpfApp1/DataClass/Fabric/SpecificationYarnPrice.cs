using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.Entity;

namespace WpfApp1.DataClass.Fabric
{
    public class SpecificationYarnPrice : MerchantYarnPrice
    {
        public string Ingredient { get; set; }
        public string YarnCount { get; set; }
        public string Color { get; set; }
    }
}
