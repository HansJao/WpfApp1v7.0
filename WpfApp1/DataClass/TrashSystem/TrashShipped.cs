using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.DataClass.TrashSystem
{

    /// <summary>
    /// 帳務系統出貨資料
    /// </summary>
    public class TrashShipped
    {
        public DateTime IN_DATE { get; set; }
        public int I_01 { get; set; }
        public string I_03 { get; set; }
        public int Quantity { get; set; }
    }
}
