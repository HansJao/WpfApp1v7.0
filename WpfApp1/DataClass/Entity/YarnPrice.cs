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
        public int YarnSpecificationNo { get; set; }
        public int YarnMerchant { get; set; }
        public int Brand { get; set; }
        public int Price { get; set; }
        public int PiecePrice { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
