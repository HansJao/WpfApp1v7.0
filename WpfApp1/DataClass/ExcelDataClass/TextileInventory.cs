using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WpfApp1.DataClass.ExcelDataClass
{
    public class TextileColorInventory
    {
        public string Index { get; set; }
        public string ColorName { get; set; }
        public string StorageSpaces { get; set; }
        public double Inventory { get; set; }
        public double DifferentCylinder { get; set; }
        public double ShippingDate1 { get; set; }
        public double ShippingDate2 { get; set; }
        public double ShippingDate3 { get; set; }
        public double ShippingDate4 { get; set; }
        public double ShippingDate5 { get; set; }
        public double ShippingDate6 { get; set; }
        public double ShippingDate7 { get; set; }
        public double ShippingDate8 { get; set; }
        public double ShippingDate9 { get; set; }
        public ExcelCell TextileFactory { get; set; }
        public ExcelCell ClearFactory { get; set; }
        public string CountInventory { get; set; }
        public string IsChecked { get; set; }
        public string CheckDate { get; set; }
        public string Memo { get; set; }
    }
    public class ExcelCell
    {
        public string CellValue { get; set; }
        public SolidColorBrush FontColor { get; set; }
    }
}
