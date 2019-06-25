using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.DataClass.TrashSystem
{
    public class ChartData
    {
        public string LegendText { get; set; }
        public List<ChartDetail> ChartDetail { get; set; }
        public double MaxQuantity { get; set; }
    }
    public class ChartDetail
    {
        public double ShippedDate { get; set; }
        public double Quantity { get; set; }
    }
}
