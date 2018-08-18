using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public static void AddRange<T>(this ObservableCollection<T> observableCollection, IEnumerable<T> rangeList)
        {
            foreach (T item in rangeList)
            {
                observableCollection.Add(item);
            }
        }
    }
}
