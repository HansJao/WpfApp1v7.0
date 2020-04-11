using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.DataClass.AccountSystem
{
    public class AccountTextile
    {
        public int AccountTextileID;
        public string FactoryID { get; set; }
        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public int DefaultPrice { get; set; }
    }
}
