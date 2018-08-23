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
    /// EditProcessOrderFlowFactoryName.xaml 的互動邏輯
    /// </summary>
    public partial class EditProcessOrderFlowFactoryNameDialog : Window
    {
        protected IFactoryModule FactoryModule { get; } = new FactoryModule();
        protected IProcessModule ProcessModule { get; } = new ProcessModule();
        private IEnumerable<Factory> _factories;
        private ProcessOrderFlowDateDetail _processOrderFlowDateDetail;
        public EditProcessOrderFlowFactoryNameDialog(ProcessOrderFlowDateDetail processOrderFlowDateDetail)
        {

            InitializeComponent();
            _factories = FactoryModule.GetFactoryList();
            ComboBoxFactoryName.ItemsSource = _factories.Select(s => s.Name);
            ComboBoxFactoryName.SelectedItem = processOrderFlowDateDetail.Name;
            _processOrderFlowDateDetail = processOrderFlowDateDetail;
        }

        private void ButtonEditProcessOrderFlowFactory_Click(object sender, RoutedEventArgs e)
        {
            ProcessOrderPage page = (ProcessOrderPage)this.DataContext;
            var selectedFactory = _factories.Where(w => w.Name == ComboBoxFactoryName.SelectedItem.ToString()).First();
            bool success = ProcessModule.EditProcessOrderFlowFactory(selectedFactory.FactoryID, _processOrderFlowDateDetail.OrderFlowNo);
            if (success)
            {
                var processOrderFlowDateDetail = page.DataGridProcessOrderFlowDateDetail.SelectedItem as ProcessOrderFlowDateDetail;
                processOrderFlowDateDetail.Name = selectedFactory.Name;
                page.DataGridProcessOrderFlowDateDetail.CommitEdit();
                page.DataGridProcessOrderFlowDateDetail.Items.Refresh();
            }
            this.Close();
        }
    }
}
