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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.ViewModel.TrashSystemViewModel;

namespace WpfApp1.Pages.TrashSystemPages
{
    /// <summary>
    /// DefaultPricePage.xaml 的互動邏輯
    /// </summary>
    public partial class DefaultPricePage : Page
    {
        public DefaultPricePage()
        {
            InitializeComponent();
            this.DataContext = new DefaultPriceViewModel();
        }
    }
}
