using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1.Windows
{
    /// <summary>
    /// FactoryAddDialog.xaml 的互動邏輯
    /// </summary>
    public partial class FactoryAddDialog : Window
    {
        public FactoryAddDialog(string FactoryName)
        {
            InitializeComponent();
            TextBox1.Text = FactoryName;
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //Closing -= Window_Closing;
            //e.Cancel = true;
            //var anim = new DoubleAnimation(0, (Duration)TimeSpan.FromSeconds(1));
            //anim.Completed += (s, _) => this.Close();
            //this.BeginAnimation(UIElement.OpacityProperty, anim);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Closing -= Window_Closing;
            e.Cancel = true;
            var anim = new DoubleAnimation(0, (Duration)TimeSpan.FromSeconds(1));
            anim.Completed += (s, _) => this.Close();
            this.BeginAnimation(UIElement.OpacityProperty, anim);
        }
    }
}
