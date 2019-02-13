using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        [Description("出貨時間")]
        public DateTime IN_DATE { get; set; }
        [Description("貨號")]
        public int I_01 { get; set; }
        [Description("布種顏色")]
        public string I_03 { get; set; }
        [Description("數量")]
        public int Quantity { get; set; }
    }
}
