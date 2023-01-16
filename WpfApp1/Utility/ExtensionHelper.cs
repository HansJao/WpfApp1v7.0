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
        public static decimal ToDecimal(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return 0;
            }
            return Convert.ToDecimal(s);
        }
        public static int ToInt(this Enum s)
        {
            return Convert.ToInt32(s);
        }
        public static int ToInt(this double s)
        {
            if (s == 0)
            {
                return 0;
            }
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

        public static IEnumerable<TSource> Distinct<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                var elementValue = keySelector(element);
                if (seenKeys.Add(elementValue))
                {
                    yield return element;
                }
            }
        }

        public static IEnumerable<TSource> CheckFilter<TSource>(this IEnumerable<TSource> source, bool checkSkip, int start, int end, Func<TSource, bool> predicate)
        {
            if (checkSkip)
            {
                return source.ExecuteWhere(predicate).Skip(start)
                 .Take(end);
            }
            else
            {
                return source;
            }
        }

        public static IEnumerable<TSource> ExecuteWhere<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    yield return item;
                }
            }
        }
    }
}
