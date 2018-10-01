using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.DataClass.Enumeration
{
    public class FactoryEnum
    {

      
    }
    /// <summary>
    /// 加工項目
    /// </summary>
    public enum ProcessItem
    {
        [Description("織")]
        Fabric = 1,
        [Description("染")]
        Dye = 2,
        [Description("定")]
        Clear = 3,
        [Description("染定")]
        DyeClear = 4,
        [Description("刷磨毛")]
        Brushed = 5,
        [Description("印花")]
        Printing = 6,
        [Description("紗")]
        Yarn = 7,

    }

}
