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
using WpfApp1.DataClass.Enumeration;
using WpfApp1.DataClass.Entity;
using WpfApp1.Modules.FactoryModule;
using WpfApp1.Modules.FactoryModule.Implement;
using WpfApp1.Windows;
using WpfApp1.Utility;

namespace WpfApp1.Pages.FactoryPages
{
    /// <summary>
    /// AddFactory.xaml 的互動邏輯
    /// </summary>
    public partial class AddFactory : Page
    {
        protected IFactoryModule FactoryModule { get; } = new FactoryModule();
        public AddFactory(string factoryName)
        {
            InitializeComponent();
            TextBoxFactoryName.Text = factoryName;
        }

        private void ButtonAddFactory_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxProcessItem.SelectedIndex == -1)
            {
                MessageBox.Show("請選擇加工項目!!");
                return;
            }
            var selectedItem = ComboBoxProcessItem.SelectedItem.ToString();
            ProcessItem process = new ProcessItem();
            Enum.TryParse<ProcessItem>(selectedItem, out process);
            var processNumber = (int)process;
            var factory = new Factory()
            {
                Name = TextBoxFactoryName.Text,
                PhoneNumber = TextBoxPhoneNumber.Text,
                CellPhone = TextBoxCellPhone.Text,
                Fax = TextBoxFax.Text,
                Process = process,
                Address = TextBoxAddress.Text,
                Sort = TextBoxSort.Text.ToInt()
            };

            int count = FactoryModule.InsertFactory(factory);
            if (count == 1)
            {
                MessageBox.Show("新增成功!!");
                TextBoxFactoryName.Text = string.Empty;
                TextBoxPhoneNumber.Text = string.Empty;
                TextBoxCellPhone.Text = string.Empty;
                TextBoxFax.Text = string.Empty;
                TextBoxAddress.Text = string.Empty;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FactoryAddDialog dialog = new FactoryAddDialog("test");
            dialog.Show();
        }
    }
}
