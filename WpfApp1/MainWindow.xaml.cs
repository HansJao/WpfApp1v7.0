﻿using System.Windows;
using WpfApp1.Pages;
using WpfApp1.Pages.CustomerPages;
using WpfApp1.Pages.FabricPages;
using WpfApp1.Pages.FactoryPages;
using WpfApp1.Pages.ProcessOrderPages;
using WpfApp1.Pages.TrashSystemPages;
using WpfApp1.ViewModel.InventoryViewModel;
using WpfApp1.ViewModel.ProcessOrderViewModel;
using WpfApp1.Windows;
using WpfApp1.Pages.InventoryPages;
using WpfApp1.ViewModel.TrashSystemViewModel;

//using Microsoft.Office.Interop.Excel;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Left = 0;
            this.Top = 0;
            this.Height = AppSettingConfig.MainWindowHeigh();
            this.Width = AppSettingConfig.MainWindowWidth();
        }
        ShippingPage ShippingPage { get; set; }
        private void ButtonStoreSearchFunction_Click(object sender, RoutedEventArgs e)
        {
            this.Title = "庫存查詢";
            this.MainFrame.NavigationService.Navigate(new StoreSearchPage());
        }

        private void ButtonShippingFunction_Click(object sender, RoutedEventArgs e)
        {
            this.Title = "出貨";
            if (ShippingPage == null)
            {
                ShippingPage = new ShippingPage();
            }
            else
            {
                ShippingPage.GetStoreMangeWorkbook();
                ShippingPage.GetShippingCacheNameList();
                ShippingPage.DataGridShippingSheet.ItemsSource = null;
                ShippingPage.ShippingSheetStructure = new System.Collections.Generic.List<DataClass.Shipping.ShippingSheetStructure>();
                ShippingPage.ComboBoxShippingCacheName.SelectedIndex = -1;

            }

            this.MainFrame.NavigationService.Navigate(ShippingPage);

        }

        private void ButtonReportsFunction_Click(object sender, RoutedEventArgs e)
        {
            this.Title = "報表";
            this.MainFrame.NavigationService.Navigate(new ReportsPage());
        }

        private void ButtonReportsByCustomerFunction_Click(object sender, RoutedEventArgs e)
        {
            this.Title = "帳務系統表參考";
            this.MainFrame.NavigationService.Navigate(new TrashSystemTablePage());
        }
        ProcessOrderPage processOrderPage;
        private void ButtonProcessOrderRecordFunction_Click(object sender, RoutedEventArgs e)
        {
            this.Title = "訂單紀錄";
            if (processOrderPage != null && processOrderPage.InventoryListDialog != null)
            {
                processOrderPage.InventoryListDialog.Close();
                foreach (var item in processOrderPage.DeliveryListDialog)
                {
                    item.Value.Close();
                };
            }
            processOrderPage = new ProcessOrderPage();

            this.MainFrame.NavigationService.Navigate(processOrderPage);
        }

        private void ButtonNewProcessOrderRecordFunction_Click(object sender, RoutedEventArgs e)
        {
            this.Title = "新增訂單";
            this.MainFrame.NavigationService.Navigate(new NewProcessOrderPage());
        }

        private void ButtonProcessOrderStatusdFunction_Click(object sender, RoutedEventArgs e)
        {
            this.Title = "訂單狀態表";
            this.MainFrame.NavigationService.Navigate(new ProcessOrderStatusPage());
        }

        //private void ButtonExportProcessOrderRecordFunction_Click(object sender, RoutedEventArgs e)
        //{
        //    //this.MainFrame.NavigationService.Navigate(new NewProcessOrderPage());
        //    var x = new ExportProcessOrderRecordViewModel();
        //}

        private void ButtonFactoryManageFunction_Click(object sender, RoutedEventArgs e)
        {
            this.Title = "新增工廠";
            this.MainFrame.NavigationService.Navigate(new AddFactory());
        }

        private void ButtonStorageInventroyExportFunction_Click(object sender, RoutedEventArgs e)
        {
            var storeSearchViewModel = new StoreSearchViewModel();
            storeSearchViewModel.InventoryCheckSheetClick();
        }

        private void ButtonFactoryList_Click(object sender, RoutedEventArgs e)
        {
            this.Title = "新增工廠";
            this.MainFrame.NavigationService.Navigate(new FactoryListPage());
        }

        private void ButtonCustomerAddFunction_Click(object sender, RoutedEventArgs e)
        {
            this.Title = "新增客戶";
            this.MainFrame.NavigationService.Navigate(new AddCustomerPage());
        }
        private void ButtonCustomerList_Click(object sender, RoutedEventArgs e)
        {
            this.Title = "客戶清單";
            this.MainFrame.NavigationService.Navigate(new CustomerListPage());
        }

        private void ButtonFabricManageFunction_Click(object sender, RoutedEventArgs e)
        {
            this.Title = "新增布種";
            this.MainFrame.NavigationService.Navigate(new NewFabricPage());
        }
        private void ButtonFeatureSearchFunction_Click(object sender, RoutedEventArgs e)
        {
            this.Title = "特徵搜尋";
            this.MainFrame.NavigationService.Navigate(new FeatureSearchPage());
        }
        private void ButtonFabricCostQueryFunction_Click(object sender, RoutedEventArgs e)
        {
            this.Title = "布種成本查詢";
            this.MainFrame.NavigationService.Navigate(new FabricCostQueryPage());
        }

        private void ButtonYarnPriceFunction_Click(object sender, RoutedEventArgs e)
        {
            this.Title = "紗價管理";
            this.MainFrame.NavigationService.Navigate(new YarnPricePage());
        }
        private void ButtonNewMalFunction_Click(object sender, RoutedEventArgs e)
        {
            this.Title = "新增故障處理";
            this.MainFrame.NavigationService.Navigate(new MalFunctionPage());
        }
        private void ButtonInventoryRecordCompareFunction_Click(object sender, RoutedEventArgs e)
        {
            this.Title = "庫存歷史紀錄";
            this.MainFrame.NavigationService.Navigate(new InventoryRecordComparePage());
        }
        //private void ButtonShippingHistoryChartFunction_Click(object sender, RoutedEventArgs e)
        //{
        //    this.Title = "出貨紀錄圖表";
        //    this.MainFrame.NavigationService.Navigate(new ShippingHistoryChartPage());

        //}
        private void ButtonInventoryReturnFunction_Click(object sender, RoutedEventArgs e)
        {
            this.Title = "庫存數量整理";
            MessageBoxResult messageBoxResult = MessageBox.Show("請確認是否要執行庫存數量整理？", "確認", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                this.MainFrame.NavigationService.Navigate(new InventoryReturnModel());
            }
        }
        private void ButtonDeductInventoryFunction_Click(object sender, RoutedEventArgs e)
        {
            this.Title = "扣庫存量";
            this.MainFrame.NavigationService.Navigate(new DeductInventoryPage());
        }
        private void ButtonShippingRankChartFunction_Click(object sender, RoutedEventArgs e)
        {
            this.Title = "出貨排行圖表";
            this.MainFrame.NavigationService.Navigate(new ShippingRankChartPage());

        }
        private void ButtonCheckBillFunction_Click(object sender, RoutedEventArgs e)
        {
            this.Title = "對帳表";
            this.MainFrame.NavigationService.Navigate(new CheckBillPage());

        }
        private void ButtonDefaultPriceFunction_Click(object sender, RoutedEventArgs e)
        {
            this.Title = "預設單價設定";
            this.MainFrame.NavigationService.Navigate(new DefaultPricePage());
        }
        private void ButtonRevenueExportFunction_Click(object sender, RoutedEventArgs e)
        {
            string textBoxRevenueDate = TextBoxRevenueDate.Text;
            new RevenueExportViewModel(textBoxRevenueDate);
        }
        private void ButtonShipFeatureFunction_Click(object sender, RoutedEventArgs e)
        {
            string shipFeatureDate = ShipFeatureDate.Text;
            new ShipFeatureViewModel(shipFeatureDate);
        }

    }
}
