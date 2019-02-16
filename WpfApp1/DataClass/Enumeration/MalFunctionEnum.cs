using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.DataClass.Enumeration
{
    public enum MalFunctionEnum
    {
        [Description("待處理")]
        ToBeProcessed = 1,
        [Description("重修中")]
        Repairing = 2,
        [Description("報廢")]
        Scrapped = 3,
        [Description("結案")]
        Complete = 4,
    }
}
