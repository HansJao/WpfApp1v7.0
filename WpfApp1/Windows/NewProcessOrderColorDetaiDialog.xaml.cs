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
using WpfApp1.DataClass.Enumeration;
using WpfApp1.DataClass.ProcessOrder;
using WpfApp1.Modules.Process;
using WpfApp1.Modules.Process.Implement;
using WpfApp1.Pages;
using WpfApp1.Pages.ProcessOrderPages;
using WpfApp1.Utility;

namespace WpfApp1.Windows
{
    /// <summary>
    /// NewProcessOrderColorDetaiDialog.xaml 的互動邏輯
    /// </summary>
    public partial class NewProcessOrderColorDetaiDialog : Window
    {
        private ProcessOrder _processOrder;
        protected IProcessModule ProcessModule { get; } = new ProcessModule();
        public NewProcessOrderColorDetaiDialog(ProcessOrder processOrder, ProcessFactoryShippingDetail processFactoryShippingDetail)
        {

            InitializeComponent();
            LabelOrderString.Content = processOrder.OrderString;
            LabelFabric.Content = processOrder.Fabric;
            TextBoxColor.Text = processFactoryShippingDetail.Color;
            TextBoxColorNumber.Text = processFactoryShippingDetail.ColorNumber;
            _processOrder = processOrder;
        }

        private void ButtonNew_Click(object sender, RoutedEventArgs e)
        {
            var processOrderColorDetailList = new List<ProcessOrderColorDetail>();
            var selectedItem = ComboBoxStatus.SelectedItem.ToString();
            ProcessOrderColorStatus status = new ProcessOrderColorStatus();
            Enum.TryParse<ProcessOrderColorStatus>(selectedItem, out status);

            processOrderColorDetailList.Add(new ProcessOrderColorDetail
            {
                OrderNo = _processOrder.OrderNo,
                Color = TextBoxColor.Text,
                ColorNumber = TextBoxColorNumber.Text,
                Quantity = TextBoxQuantity.Text.ToInt(),
                Status = status
            }
            );

            ProcessModule.CreateProcessOrderColorFlow(processOrderColorDetailList, _processOrder.OrderNo);

            ProcessOrderPage page = (ProcessOrderPage)this.DataContext;
            page.DataGridFactoryShippingDetail.ItemsSource = ProcessModule.GetProcessFactoryShippingDetail(_processOrder.OrderNo);
            this.Close();
        }


    }
}
