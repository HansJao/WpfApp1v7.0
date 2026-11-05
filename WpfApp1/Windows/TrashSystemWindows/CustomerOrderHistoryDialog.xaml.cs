using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using WpfApp1.DataClass.TrashSystem;
using WpfApp1.ViewModel.TrashSystemViewModel.WindowViewModel;

namespace WpfApp1.Windows.TrashSystemWindows
{
    /// <summary>
    /// CustomerOrderHistoryDialog.xaml 的互動邏輯
    /// </summary>
    public partial class CustomerOrderHistoryDialog : Window
    {
        public CustomerOrderHistoryDialog(TrashCustomer trashCustomer, DateTime begin, DateTime end)
        {
            InitializeComponent();
            this.DataContext = new CustomerOrderHistoryViewModel(trashCustomer, begin, end);
        }

        // 按下「搜尋」按鈕
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilter();
        }

        // 按下「清除」按鈕
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
            ApplyFilter();
        }

        // 在輸入框按下 Enter 鍵也能觸發搜尋
        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ApplyFilter();
            }
        }

        // 執行過濾邏輯
        private void ApplyFilter()
        {
            if (ShippedDataGrid.ItemsSource == null) return;

            // 取得 DataGrid 綁定集合的預設 View
            ICollectionView view = CollectionViewSource.GetDefaultView(ShippedDataGrid.ItemsSource);
            if (view == null) return;

            string keyword = SearchTextBox.Text.Trim();

            if (string.IsNullOrEmpty(keyword))
            {
                // 若搜尋框是空的，移除過濾條件（顯示全部）
                view.Filter = null;
            }
            else
            {
                // 設定過濾條件
                view.Filter = item =>
                {
                    // 方法 A：如果你知道集合裡元素的型別（例如叫 TrashCustomerShipped），可以直接轉型（效能較好）：
                    // var data = item as TrashCustomerShipped;
                    // return data != null && data.I_03 != null && data.I_03.Contains(keyword);

                    // 方法 B：萬用寫法（不用管型別名稱叫什麼，直接抓 I_03 屬性比對）：
                    var prop = item.GetType().GetProperty("I_03");
                    if (prop != null)
                    {
                        string fabricName = prop.GetValue(item) as string;
                        return !string.IsNullOrEmpty(fabricName) && fabricName.Contains(keyword);
                    }
                    return false;
                };
            }
        }
    }
}
