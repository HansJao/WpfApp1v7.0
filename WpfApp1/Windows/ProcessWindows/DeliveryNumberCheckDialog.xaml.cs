using System.Windows;
using System.Windows.Controls;
using WpfApp1.DataClass.Entity;
using WpfApp1.DataClass.ExcelDataClass;
using WpfApp1.DataClass.ProcessOrder;
using WpfApp1.Utility;

namespace WpfApp1.Windows.ProcessWindows
{
    /// <summary>
    /// DeliveryNumberChackDialog.xaml 的互動邏輯
    /// </summary>
    public partial class DeliveryNumberCheckDialog : Window
    {
        public ProcessOrderDelivery ProcessOrderDelivery;
        public bool IsCheck { get; set; } = false;
        public DeliveryNumberCheckDialog(string factoryName, string orderString, string fabric, ProcessOrderColorDetail processOrderQuantity)
        {
            InitializeComponent();
            ProcessOrderDelivery = new ProcessOrderDelivery
            {
                //StorageNumber = textileColorInventory?.CountInventory ?? 0,
                //StorageSpace = textileColorInventory?.StorageSpaces ?? string.Empty,
                FactoryName = factoryName,
                OrderString = orderString,
                Fabric = fabric,
                Color = processOrderQuantity.Color,
                Number = processOrderQuantity.Quantity
            };
            TextBoxFactoryName.Text = factoryName;
            TextBoxDeliveryNumber.Text = processOrderQuantity.Quantity.ToString();
            TextBlockFabric.Text = string.Concat("布種：", fabric);
            TextBlockColor.Text = string.Concat("顏色：", processOrderQuantity.Color);
        }

        public int DeliveryNumberReturn()
        {
            int deliveryNumber = TextBoxDeliveryNumber.Text.ToInt();
            ProcessOrderDelivery.Number = deliveryNumber;
            return deliveryNumber;
        }

        private void ButtonCheck_Click(object sender, RoutedEventArgs e)
        {
            int deliveryNumber = TextBoxDeliveryNumber.Text.ToInt();
            ProcessOrderDelivery.Number = deliveryNumber;
            IsCheck = true;
            Close();
        }
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TextBoxFactoryName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            ProcessOrderDelivery.FactoryName = textBox.Text;
        }
    }
}
