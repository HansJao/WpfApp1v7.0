using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.DataClass.Enumeration
{
    public class ProcessEnum
    {

    }
    public enum ProcessOrderColorStatus
    {
        [Description("已出完")]
        已出完 = 1,
        [Description("緊急")]
        緊急 = 2,
        [Description("未出完")]
        未出完 = 3,
        [Description("修訂")]
        修訂 = 4
    }
}
