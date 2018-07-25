using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.DataClass.Shipping
{
    public class ShippingSheetStructure
    {
        public string Customer { get; set; }
        public List<TextileShippingData> TextileShippingDatas { get; set; }
    }
}
