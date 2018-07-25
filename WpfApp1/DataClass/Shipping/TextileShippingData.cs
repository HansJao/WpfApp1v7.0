using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.DataClass.Shipping
{
    public class TextileShippingData
    {
        public string TextileName { get; set; }
        public List<ShippingSheetData> ShippingSheetDatas { get; set; }
    }
}
