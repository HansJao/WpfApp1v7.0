using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace WpfApp1.Utility.XamlHelper
{
    public class XamlCommand
    {

    }
    public class EnterKeyHelper
    {
        public static ICommand GetEnterKeyCommand(DependencyObject target)
        {
            return (ICommand)target.GetValue(EnterKeyCommandProperty);
        }

        public static void SetEnterKeyCommand(DependencyObject target, ICommand value)
        {
            target.SetValue(EnterKeyCommandProperty, value);
        }

        public static readonly DependencyProperty EnterKeyCommandProperty =
            DependencyProperty.RegisterAttached(
                "EnterKeyCommand",
                typeof(ICommand),
                typeof(EnterKeyHelper),
                new PropertyMetadata(null, OnEnterKeyCommandChanged));

        public static void OnEnterKeyCommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            ICommand command = (ICommand)e.NewValue;
            FrameworkElement fe = (FrameworkElement)target;
            Control control = (Control)target;
            control.KeyDown += (s, args) =>
            {
                if (args.Key == Key.Enter)
                {
                    // make sure the textbox binding updates its source first
                    BindingExpression b = control.GetBindingExpression(TextBox.TextProperty);
                    if (b != null)
                    {
                        b.UpdateSource();
                    }
                    command.Execute(null);
                }
            };
        }
    }
}
