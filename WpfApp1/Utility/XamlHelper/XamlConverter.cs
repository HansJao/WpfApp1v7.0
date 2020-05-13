using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfApp1.Utility.XamlHelper
{
    public class XamlConverter
    {

    }
    public class BindingProxy : Freezable
    {
        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }


        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new UIPropertyMetadata(null));
    }
    public class ZeroToEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                return (int)value == 0 ? "" : value;
            }
            if (value is double)
            {
                return (double)value == 0 ? "" : value;
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                int intValue;
                if (!int.TryParse((string)value, out intValue))
                {
                    intValue = 0;
                }
                return intValue;
            }
            return 0;
        }
    }

    public class EnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;
            return GetDescription((Enum)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        public static string GetDescription(Enum en)
        {
            Type type = en.GetType();
            MemberInfo[] memInfo = type.GetMember(en.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            return en.ToString();
        }
    }

    public class HeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (double.TryParse(value.ToString(), out double number) && double.TryParse(parameter.ToString(), out double coefficient))
            {
                return number / coefficient;
            }

            return 100;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class ColorMultiConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values != null)
            {
                int price = Int32.Parse(values[0].ToString());
                int defaultPrice = Int32.Parse(values[1].ToString());
                var customerPrice = Int32.Parse(values[2].ToString());
                if (price == 0)
                    return new SolidColorBrush(Colors.Red);
                if (customerPrice != 0)
                    if (price < customerPrice)
                    {
                        return new SolidColorBrush(Colors.Pink);
                    }
                    else if (price > customerPrice)
                    {
                        return new SolidColorBrush(Colors.LightGreen);
                    }
                    else
                    {
                        return new SolidColorBrush(Colors.LightBlue);
                    }
                else
                {
                    if (price < defaultPrice)
                    {
                        return new SolidColorBrush(Colors.Pink);
                    }
                    else if (price > defaultPrice)
                    {
                        return new SolidColorBrush(Colors.LightGreen);
                    }
                    else
                    {
                        return new SolidColorBrush(Colors.LightBlue);
                    }
                }

            }
            return new SolidColorBrush(Colors.White);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
