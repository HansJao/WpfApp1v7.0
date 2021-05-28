using System.Windows;
using WpfApp1.DataClass.Entity;
using WpfApp1.DataClass.ProcessOrder;
using WpfApp1.Utility;

namespace WpfApp1.Windows.ProcessWindows
{
    /// <summary>
    /// DeliveryNumberChackDialog.xaml 的互動邏輯
    /// </summary>
    public partial class DeliveryNumberCheckDialog : Window
    {
        public ProcessOrderDelivery processOrderDelivery;
        public DeliveryNumberCheckDialog(string orderString, string fabric, ProcessOrderColorDetail processOrderQuantity)
        {
            InitializeComponent();
            processOrderDelivery = new ProcessOrderDelivery
            {
                OrderString = orderString,
                Fabric = fabric,
                Color = processOrderQuantity.Color,
                Number = processOrderQuantity.Quantity
            };
            TextBoxDeliveryNumber.Text = processOrderQuantity.Quantity.ToString();
        }

        public int DeliveryNumberReturn()
        {
            int deliveryNumber = TextBoxDeliveryNumber.Text.ToInt();
            processOrderDelivery.Number = deliveryNumber;
            return deliveryNumber;
        }

        private void ButtonCheck_Click(object sender, RoutedEventArgs e)
        {
            int deliveryNumber = TextBoxDeliveryNumber.Text.ToInt();
            processOrderDelivery.Number = deliveryNumber;
            Close();
        }
    }
}
