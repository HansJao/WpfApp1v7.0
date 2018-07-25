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
using WpfApp1.Modules.CustomerModule;
using WpfApp1.Modules.CustomerModule.Implement;
using WpfApp1.Utility;

namespace WpfApp1.Windows
{
    /// <summary>
    /// CustomerEditDialog.xaml 的互動邏輯
    /// </summary>
    public partial class CustomerEditDialog : Window
    {
        protected ICustomerModule CustomerModule { get; } = new CustomerModule();
        private int _customerID;
        public CustomerEditDialog(Customer customer)
        {
            InitializeComponent();
            _customerID = customer.CustomerID;
            LabelCustomerName.Content = customer.Name;
            TextBoxPhoneNumber.Text = customer.PhoneNumber;
            TextBoxCellPhone.Text = customer.CellPhone;
            TextBoxFax.Text = customer.Fax;
            TextBoxAddress.Text = customer.Address;
            TextBoxMemo.Text = customer.Memo;
            TextBoxSort.Text = customer.Sort.ToString();
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = new Customer()
            {
                CustomerID = _customerID,
                Name = LabelCustomerName.Content.ToString(),
                PhoneNumber = TextBoxPhoneNumber.Text,
                CellPhone = TextBoxCellPhone.Text,
                Fax = TextBoxFax.Text,
                Address = TextBoxAddress.Text,
                Memo = TextBoxMemo.Text,
                Sort = TextBoxSort.Text.ToInt()
            };

           int count =  CustomerModule.UpdateCustomer(customer);
            Close();
        }
    }
}
