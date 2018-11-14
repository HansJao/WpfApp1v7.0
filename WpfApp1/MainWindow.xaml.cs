using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
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
using WpfApp1.Pages;
using WpfApp1.Pages.CustomerPages;
using WpfApp1.Pages.FabricPages;
using WpfApp1.Pages.FactoryPages;
using WpfApp1.Pages.ProcessOrderPages;
using WpfApp1.Pages.TrashSystemPages;
using WpfApp1.ViewModel.InventoryViewModel;
using WpfApp1.ViewModel.ProcessOrderViewModel;
using WpfApp1.Windows;

//using Microsoft.Office.Interop.Excel;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Left = 100;
            this.Top = 100;
        }

        private void ButtonStoreSearchFunction_Click(object sender, RoutedEventArgs e)
        {
            this.MainFrame.NavigationService.Navigate(new StoreSearchPage());

        }

        private void ButtonShippingFunction_Click(object sender, RoutedEventArgs e)
        {
            this.MainFrame.NavigationService.Navigate(new ShippingPage());
        }

        private void ButtonReportsFunction_Click(object sender, RoutedEventArgs e)
        {
            this.MainFrame.NavigationService.Navigate(new ReportsPage());
        }

        private void ButtonReportsByCustomerFunction_Click(object sender, RoutedEventArgs e)
        {
            this.MainFrame.NavigationService.Navigate(new TrashSystemTablePage());
        }

        private void ButtonProcessOrderRecordFunction_Click(object sender, RoutedEventArgs e)
        {
            this.MainFrame.NavigationService.Navigate(new ProcessOrderPage());
        }

        private void ButtonNewProcessOrderRecordFunction_Click(object sender, RoutedEventArgs e)
        {
            this.MainFrame.NavigationService.Navigate(new NewProcessOrderPage());
        }

        private void ButtonExportProcessOrderRecordFunction_Click(object sender, RoutedEventArgs e)
        {
            //this.MainFrame.NavigationService.Navigate(new NewProcessOrderPage());
            var x = new ExportProcessOrderRecordViewModel();
        }

        private void ButtonFactoryManageFunction_Click(object sender, RoutedEventArgs e)
        {
            this.MainFrame.NavigationService.Navigate(new AddFactory());
        }

        private void ButtonStorageInventroyExportFunction_Click(object sender, RoutedEventArgs e)
        {
            var storeSearchViewModel = new StoreSearchViewModel();
            storeSearchViewModel.InventoryCheckSheetClick();
        }

        private void ButtonFactoryList_Click(object sender, RoutedEventArgs e)
        {
            this.MainFrame.NavigationService.Navigate(new FactoryListPage());
        }

        private void ButtonCustomerAddFunction_Click(object sender, RoutedEventArgs e)
        {
            this.MainFrame.NavigationService.Navigate(new AddCustomerPage());
        }
        private void ButtonCustomerList_Click(object sender, RoutedEventArgs e)
        {
            this.MainFrame.NavigationService.Navigate(new CustomerListPage());
        }

        private void ButtonFabricManageFunction_Click(object sender, RoutedEventArgs e)
        {
            this.MainFrame.NavigationService.Navigate(new NewFabricPage());
        }
        private void ButtonFeatureSearchFunction_Click(object sender, RoutedEventArgs e)
        {
            this.MainFrame.NavigationService.Navigate(new FeatureSearchPage());
        }
        private void ButtonFabricCostQueryFunction_Click(object sender, RoutedEventArgs e)
        {
            this.MainFrame.NavigationService.Navigate(new FabricCostQueryPage());
        }

    }
}
