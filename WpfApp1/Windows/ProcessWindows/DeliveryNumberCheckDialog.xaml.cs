using System.Windows;
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
        public TextileColorInventory InventoryListSelectedTextileColor;
        public DeliveryNumberCheckDialog(string orderString, string fabric, ProcessOrderColorDetail processOrderQuantity)
        {
            InitializeComponent();
            ProcessOrderDelivery = new ProcessOrderDelivery
            {
                //StorageNumber = textileColorInventory?.CountInventory ?? 0,
                //StorageSpace = textileColorInventory?.StorageSpaces ?? string.Empty,
                OrderString = orderString,
                Fabric = fabric,
                Color = processOrderQuantity.Color,
                Number = processOrderQuantity.Quantity
            };
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
    }
}
