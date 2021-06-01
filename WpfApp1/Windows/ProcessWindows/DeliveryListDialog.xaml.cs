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

namespace WpfApp1.Windows.ProcessWindows
{
    /// <summary>
    /// DeliveryListDialog.xaml 的互動邏輯
    /// </summary>
    public partial class DeliveryListDialog : Window
    {
        ObservableCollection<ProcessOrderDelivery> processOrderColorDetails { get; set; }
        public DeliveryListDialog(ProcessOrderDelivery processOrderDelivery)
        {
            InitializeComponent();
            processOrderColorDetails = new ObservableCollection<ProcessOrderDelivery>();
            if (processOrderDelivery != null)
            {
                processOrderColorDetails.Add(processOrderDelivery);
                //processOrderColorDetails.Add(processOrderDelivery);
                DataGridProcessOrderDelivery.ItemsSource = processOrderColorDetails;
            }
        }

        public void ProcessOrderColorDetailChanged(ProcessOrderDelivery processOrderDelivery)
        {
            processOrderColorDetails.Add(processOrderDelivery);
            //DataGridProcessOrderDelivery.ItemsSource = processOrderColorDetails;

        }
    }
}
