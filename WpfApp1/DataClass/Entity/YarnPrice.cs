using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.DataClass.Entity
{
    public class YarnPrice
    {
        public int YarnPriceNo { get; set; }
        public string Ingredient { get; set; }
        public string YarnCount { get; set; }
        public string Color { get; set; }
        public int Price { get; set; }
        public int YarnMerchant { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
