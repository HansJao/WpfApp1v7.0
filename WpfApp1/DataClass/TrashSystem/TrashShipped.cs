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
        /// <summary>
        /// 工廠號
        /// </summary>
        [Description("工廠號")]
        public string F_01 { get; set; }
        /// <summary>
        /// 貨號
        /// </summary>
        [Description("貨號")]
        public string I_01 { get; set; }
        /// <summary>
        /// 客戶編號
        /// </summary>
        [Description("客戶編號")]
        public string C_01 { get; set; }
        [Description("日期單號")]
        public int IN_NO { get; set; }
        /// <summary>
        /// 布種顏色
        /// </summary>
        [Description("布種顏色")]
        public string I_03 { get; set; }
        [Description("數量")]
        public double Quantity { get; set; }
        [Description("單價")]
        public double Price { get; set; }
        [Description("輸入時間")]
        public DateTime Time { get; set; }
    }
}
