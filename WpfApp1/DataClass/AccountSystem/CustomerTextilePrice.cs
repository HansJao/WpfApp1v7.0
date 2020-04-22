using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.DataClass.AccountSystem
{
    public class CustomerTextilePrice
    {
        public int CustomerTextilePriceID { get; set; }
        public string AccountCustomerID { get; set; }
        public int AccountTextileID { get; set; }
        public int Price { get; set; }
    }
}
