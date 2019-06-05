using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.DataClass.TrashSystem
{
    public class ShippingRecord
    {
        public string TextileName { get; set; }
        public List<ShippingRecordDetail> ShippingRecordDetails { get; set; }
        public double MaxQuantity { get; set; }
    }
    public class ShippingRecordDetail
    {
        public DateTime ShippedDate { get; set; }
        public double Quantity { get; set; }
    }
}
