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
using WpfApp1.Modules.CustomerModule;
using WpfApp1.Modules.CustomerModule.Implement;
using WpfApp1.Windows;

namespace WpfApp1.Pages.CustomerPages
{
    /// <summary>
    /// CustomerListPage.xaml 的互動邏輯
    /// </summary>
    public partial class CustomerListPage : Page
    {
        protected ICustomerModule CustomerModule { get; } = new CustomerModule();
        public CustomerListPage()
        {
            InitializeComponent();
            IEnumerable<Customer> customerList = CustomerModule.GetCustomerList();
            DataGridCustomerList.ItemsSource = customerList;
        }

        private void ButtonEditCustomer_Click(object sender, RoutedEventArgs e)
        {
            var customer = DataGridCustomerList.SelectedItem as Customer ?? new Customer();
            CustomerEditDialog dialog = new CustomerEditDialog(customer);
            dialog.Show();
        }
    }
}
