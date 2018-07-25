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
using WpfApp1.DataClass.Entity;
using WpfApp1.Modules.FactoryModule;
using WpfApp1.Modules.FactoryModule.Implement;
using WpfApp1.Windows;

namespace WpfApp1.Pages.FactoryPages
{

    
    /// <summary>
    /// FactoryListPage.xaml 的互動邏輯
    /// </summary>
    public partial class FactoryListPage : Page
    {
        protected IFactoryModule FactoryModule { get; } = new FactoryModule();

        public FactoryListPage()
        {
            InitializeComponent();
            IEnumerable<Factory> factoryList = FactoryModule.GetFactoryList();
            DataGridFactoryList.ItemsSource = factoryList;
        }

        private void ButtonEditFactory_Click(object sender, RoutedEventArgs e)
        {
            var factory = DataGridFactoryList.SelectedItem as Factory ?? new Factory();
            FactoryEditDialog factoryEditDialog = new FactoryEditDialog(factory);
            factoryEditDialog.Show();
        }
    }
}
