using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

        public static void CheckSuccessMessageBox(this bool success, string successMessage, string failMessage)
        {
            if (success == true)
            {
                MessageBox.Show(successMessage);
            }
            else
            {
                MessageBox.Show(failMessage);
            }

        }
    }
}
