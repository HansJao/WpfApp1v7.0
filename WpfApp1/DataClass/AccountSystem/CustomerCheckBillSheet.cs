using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.TrashSystem;

namespace WpfApp1.DataClass.AccountSystem
{
    public class CustomerCheckBillSheet : TrashCustomerShipped
    {
        public int AccountTextileID { get; set; }
        public int DefaultPrice { get; set; }
        public int CustomerPrice { get; set; }
    }
}
