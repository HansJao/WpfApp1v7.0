using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.DataClass.TrashSystem
{
    public class TrashItem
    {
        [Description("工廠代號")]
        public string F_01 { get; set; }
        [Description("貨號")]
        public string I_01 { get; set; }
        [Description("布種顏色")]
        public string I_03 { get; set; }


    }
}
