using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Utility
{
    public static class ExtensionHelper
    {
        public static int ToInt(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return 0;
            }
            return Convert.ToInt32(s);
        }
        public static int ToInt(this Enum s)
        {
            return Convert.ToInt32(s);
        }
    }
}
