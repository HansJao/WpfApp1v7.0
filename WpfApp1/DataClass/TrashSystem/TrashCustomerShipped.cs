using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.DataClass.TrashSystem
{
    public class TrashCustomerShipped : TrashShipped
    {
        [Description("客戶名稱")]
        public string C_Name { get; set; }
    }
}
