using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;
using WpfApp1.DataClass.Entity;
using WpfApp1.DataClass.ProcessOrder;
using WpfApp1.Modules.FactoryModule;
using WpfApp1.Modules.FactoryModule.Implement;
using WpfApp1.Modules.Process;
using WpfApp1.Modules.Process.Implement;
using WpfApp1.Pages.ProcessOrderPages;

namespace WpfApp1.Windows.ProcessWindows
{
    /// <summary>
    /// AddProcessOrderFlowFactoryDialog.xaml 的互動邏輯
    /// </summary>
    public partial class AddProcessOrderFlowFactoryDialog : Window
    {

        protected IFactoryModule FactoryModule { get; } = new FactoryModule();
        protected IProcessModule ProcessModule { get; } = new ProcessModule();

        public ObservableCollection<Factory> Factories { get; set; }

        public string FactoryName { get; set; }

        public Factory Factory { get; set; }

        public AddProcessOrderFlowFactoryDialog()
        {
            InitializeComponent();
            ComboBoxFactoryNames.ItemsSource = FactoryModule.GetFactoryList();
        }

        private void ButtonAddFactory_Click(object sender, RoutedEventArgs e)
        {
            Factory = ComboBoxFactoryNames.SelectedItem as Factory;
            ProcessOrderPage processOrderPage = (ProcessOrderPage)this.DataContext;

            Factory factory = (Factory)ComboBoxFactoryNames.SelectedItem;
            ProcessOrder processOrder = (ProcessOrder)processOrderPage.DataGridProcessOrder.SelectedItem;
            IEnumerable<ProcessOrderColorFactoryShippingDetail> processOrderColorDetails = processOrderPage.DataGridOrderColorFactoryShippingDetail.SelectedItems.Cast<ProcessOrderColorFactoryShippingDetail>();
            
            int count = ProcessModule.NewProcessOrderFlow(new ProcessOrderFlow { OrderNo = processOrder.OrderNo, FactoryID = factory.FactoryID, }
            , processOrderColorDetails.Select(s => s.OrderColorDetailNo));
        }
    }
}

