using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp1.Utility.ControllerBehavior
{
    public class DataGridBehavior
    {
        public static DependencyProperty OnGotFocusProperty = DependencyProperty.RegisterAttached(
           "OnGotFocus",
           typeof(ICommand),
           typeof(DataGridBehavior),
           new UIPropertyMetadata(OnGotFocus));

        public static void SetOnGotFocus(DependencyObject target, ICommand value)
        {
            target.SetValue(OnGotFocusProperty, value);
        }

        /// <summary>
        /// PropertyChanged callback for OnDoubleClickProperty.  Hooks up an event handler with the 
        /// actual target.
        /// </summary>
        private static void OnGotFocus(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            var element = target as DataGrid;
            if (element == null)
            {
                throw new InvalidOperationException("This behavior can be attached to a TextBox item only.");
            }

            if ((e.NewValue != null) && (e.OldValue == null))
            {
                element.GotFocus += OnPreviewGotFocus;
            }
            else if ((e.NewValue == null) && (e.OldValue != null))
            {
                element.GotFocus -= OnPreviewGotFocus;
            }
        }

        private static void OnPreviewGotFocus(object sender, RoutedEventArgs e)
        {
            UIElement element = (UIElement)sender;
            ICommand command = (ICommand)element.GetValue(DataGridBehavior.OnGotFocusProperty);
            if (command != null)
            {
                command.Execute(e);
            }
        }

        public static DependencyProperty OnSelectionChangedProperty = DependencyProperty.RegisterAttached(
          "OnSelectionChanged",
          typeof(ICommand),
          typeof(DataGridBehavior),
          new UIPropertyMetadata(OnSelectionChanged));

        public static void SetOnSelectionChanged(DependencyObject target, ICommand value)
        {
            target.SetValue(OnSelectionChangedProperty, value);
        }

        /// <summary>
        /// PropertyChanged callback for OnDoubleClickProperty.  Hooks up an event handler with the 
        /// actual target.
        /// </summary>
        private static void OnSelectionChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            var element = target as DataGrid;
            if (element == null)
            {
                throw new InvalidOperationException("This behavior can be attached to a TextBox item only.");
            }

            if ((e.NewValue != null) && (e.OldValue == null))
            {
                element.SelectionChanged += OnPreviewSelectionChanged;
            }
            else if ((e.NewValue == null) && (e.OldValue != null))
            {
                element.SelectionChanged -= OnPreviewSelectionChanged;
            }
        }

        private static void OnPreviewSelectionChanged(object sender, RoutedEventArgs e)
        {
            UIElement element = (UIElement)sender;
            ICommand command = (ICommand)element.GetValue(DataGridBehavior.OnSelectionChangedProperty);
            if (command != null)
            {
                command.Execute(e);
            }
        }
    }
}
