using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using WpfApp1.DataClass.Entity;
using WpfApp1.DataClass.Enumeration;
using WpfApp1.DataClass.ProcessOrder;
using WpfApp1.Modules.CustomerModule;
using WpfApp1.Modules.CustomerModule.Implement;
using WpfApp1.Modules.FactoryModule;
using WpfApp1.Modules.FactoryModule.Implement;
using WpfApp1.Modules.Process;
using WpfApp1.Modules.Process.Implement;
using WpfApp1.Utility;
using WpfApp1.Windows;
using WpfApp1.Windows.ProcessWindows;

namespace WpfApp1.Pages.ProcessOrderPages
{
    /// <summary>
    /// ProcessOrderPage.xaml 的互動邏輯
    /// </summary>
    public partial class ProcessOrderPage : Page
    {

        protected IProcessModule ProcessModule { get; } = new ProcessModule();
        protected ICustomerModule CustomerModule { get; } = new CustomerModule();
        protected IFactoryModule FactoryModule { get; } = new FactoryModule();
        private ProcessOrderColorStatus _processOrderColorStatus = ProcessOrderColorStatus.未完成;
        public ProcessOrderPage()
        {
            InitializeComponent();

            ComboBoxStatus.ItemsSource = Enum.GetValues(typeof(ProcessOrderColorStatus)).Cast<ProcessOrderColorStatus>();
            ComboBoxStatus.SelectedIndex = 2;

            ComboBoxCustomer.ItemsSource = CustomerModule.GetCustomerNameList();
            ComboBoxCustomer.Loaded += (ls, le) =>
            {
                var targetTextBox = ComboBoxCustomer?.Template.FindName("PART_EditableTextBox", ComboBoxCustomer) as TextBox;

                if (targetTextBox == null) return;

                ComboBoxCustomer.Tag = "TextInput";
                ComboBoxCustomer.StaysOpenOnEdit = true;
                ComboBoxCustomer.IsEditable = true;
                ComboBoxCustomer.IsTextSearchEnabled = false;

                targetTextBox.TextChanged += (o, args) =>
                {
                    var textBox = (TextBox)o;

                    var searchText = textBox.Text;

                    if (ComboBoxCustomer.Tag.ToString() == "Selection")
                    {
                        ComboBoxCustomer.Tag = "TextInput";
                        ComboBoxCustomer.IsDropDownOpen = true;
                    }
                    else
                    {
                        if (ComboBoxCustomer.SelectionBoxItem != null)
                        {
                            ComboBoxCustomer.SelectedItem = null;
                            targetTextBox.Text = searchText;
                            ComboBoxCustomer.IsDropDownOpen = true;
                            targetTextBox.SelectionStart = targetTextBox.Text.Length;
                        }

                        if (string.IsNullOrEmpty(searchText))
                        {
                            ComboBoxCustomer.Items.Filter = item => true;
                            ComboBoxCustomer.SelectedItem = default(object);
                        }
                        else
                            ComboBoxCustomer.Items.Filter = item =>
                                    item.ToString().StartsWith(searchText);

                        //Keyboard.ClearFocus();
                        //Keyboard.Focus(targetTextBox);
                        ComboBoxCustomer.IsDropDownOpen = true;
                        targetTextBox.SelectionStart = targetTextBox.Text.Length;
                    }
                };

                ComboBoxCustomer.SelectionChanged += (o, args) =>
                {
                    var comboBox = o as ComboBox;
                    if (comboBox?.SelectedItem == null) return;
                    comboBox.Tag = "Selection";
                };
            };

            DataGridFactoryList.ItemsSource = FactoryModule.GetFactoryList();
        }

        private void DataGridProcessOrder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dataGrid = (DataGrid)sender;
            if (dataGrid.SelectedIndex == -1)
            {
                return;
            }
            var processOrder = dataGrid.SelectedItem as ProcessOrder;

            //var processOrderFlow = ProcessModule.GetProcessOrderFlow(processOrder.OrderNo);
            //DataGridProcessOrderFlow.ItemsSource = processOrderFlow;

            string processOrderRemark = ProcessModule.GetProcessOrderRemark(processOrder.OrderNo);
            TextRange remark = new TextRange(RichTextBoxProcessOrderRemark.Document.ContentStart, RichTextBoxProcessOrderRemark.Document.ContentEnd);
            remark.Text = processOrderRemark ?? "";

            UpdateDataGridOrderColorFactoryShippingDetail(processOrder.OrderNo);
            DataGridOrderColorFactoryShippingDetail.UpdateLayout();


            IEnumerable<int> isCompleteColor = ProcessModule.GetIsCompleteColor(processOrder.OrderNo);
            foreach (ProcessOrderColorFactoryShippingDetail item in DataGridOrderColorFactoryShippingDetail.ItemsSource)
            {
                var row = DataGridOrderColorFactoryShippingDetail.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                if(item.Status == ProcessOrderColorStatus.已出完)
                {
                    row.Background = Brushes.Gray;
                }
                else if (isCompleteColor.Contains(item.OrderColorDetailNo))
                {
                    row.Background = Brushes.Pink;
                }
            }

            DataGridFactoryShipping.ItemsSource = null;
            ComboBoxCustomer.SelectedIndex = -1;
            TextBoxQuantity.Text = string.Empty;
            DataGridProcessOrderFlowDateDetail.ItemsSource = null;


        }

        private void ButtonDeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            var processOrder = (ProcessOrder)DataGridProcessOrder.SelectedItem;
            if (processOrder == null)
            {
                MessageBox.Show("未選取訂單!!");
                return;
            }
            var result = MessageBox.Show(string.Concat("請確認是否要刪除訂單編號:", processOrder.OrderString, ",布種:", processOrder.Fabric), "刪除", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                ProcessModule.DeleteProcessOrder(processOrder);
                DataGridProcessOrder.ItemsSource = ProcessModule.GetProcessOrderByStatus(_processOrderColorStatus);
                //DataGridProcessOrderFlow.ItemsSource = null;
                DataGridOrderColorFactoryShippingDetail.ItemsSource = null;
                DataGridProcessOrderFlowDateDetail.ItemsSource = null;
                DataGridFactoryShipping.ItemsSource = null;
            }
        }

        private void TextBoxOrderStringSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBoxName = (TextBox)sender;
            string filterText = textBoxName.Text;
            ICollectionView cv = CollectionViewSource.GetDefaultView(DataGridProcessOrder.ItemsSource);
            if (!string.IsNullOrEmpty(filterText))
            {
                cv.Filter = o =>
                {
                    /* change to get data row value */
                    ProcessOrder p = o as ProcessOrder;
                    return (p.OrderString.ToUpper().Contains(filterText.ToUpper()));
                    /* end change to get data row value */
                };
            }
            else
            {
                cv.Filter = o =>
                {
                    return (true);
                };
            }
        }

        private void TextBoxFabricSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBoxName = (TextBox)sender;
            string filterText = textBoxName.Text;
            ICollectionView cv = CollectionViewSource.GetDefaultView(DataGridProcessOrder.ItemsSource);
            if (!string.IsNullOrEmpty(filterText))
            {
                cv.Filter = o =>
                {
                    /* change to get data row value */
                    ProcessOrder p = o as ProcessOrder;
                    return (p.Fabric.ToUpper().Contains(filterText.ToUpper()));
                    /* end change to get data row value */
                };
            }
            else
            {
                cv.Filter = o =>
                {
                    return (true);
                };
            }
        }

        private void DataGridProcessOrderColorDetail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dataGrid = (DataGrid)sender;

            var processOrderColorDetail = (ProcessOrderColorDetail)dataGrid.SelectedItem;
            if (processOrderColorDetail == null)
                return;

            var factoryShippingList = ProcessModule.GetFactoryShipping(processOrderColorDetail.OrderColorDetailNo);
            DataGridFactoryShipping.ItemsSource = factoryShippingList;

            List<int> orderColorDetailNos = new List<int> { processOrderColorDetail.OrderColorDetailNo };
            RefrashDataGridProcessOrderFlowDateDetail(orderColorDetailNos);
        }

        public void RefrashDataGridProcessOrderFlowDateDetail(List<int> orderColorDetailNos)
        {
            var processOrderFlowDateDetail = ProcessModule.GetProcessOrderFlowDateDetail(orderColorDetailNos);
            DataGridProcessOrderFlowDateDetail.ItemsSource = processOrderFlowDateDetail;
        }

        private void ButtonNewFactoryShipping_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridOrderColorFactoryShippingDetail.SelectedIndex == -1)
            {
                MessageBox.Show("未選擇出貨顏色!!");
                return;
            }
            if (string.IsNullOrEmpty(TextBoxQuantity.Text))
            {
                MessageBox.Show("未輸入出貨數量!!");
                return;
            }
            if (ComboBoxCustomer.SelectedIndex == -1)
            {
                MessageBox.Show("未選擇客戶!!");
                return;
            }
            var processOrderColorDetail = (ProcessOrderColorDetail)DataGridOrderColorFactoryShippingDetail.SelectedItem;
            var factoryShipping = new FactoryShippingName
            {
                OrderColorDetailNo = processOrderColorDetail.OrderColorDetailNo,
                Name = ComboBoxCustomer.SelectedItem.ToString(),
                Quantity = TextBoxQuantity.Text.ToInt(),
                CreateDate = DateTime.Now,
                ShippingDate = DatePickerShippingDate.SelectedDate
            };

            var count = ProcessModule.InsertFactoryShipping(factoryShipping);
            var factoryShippingList = ProcessModule.GetFactoryShipping(processOrderColorDetail.OrderColorDetailNo);
            DataGridFactoryShipping.ItemsSource = factoryShippingList;

            UpdateDataGridOrderColorFactoryShippingDetail(processOrderColorDetail.OrderNo);
        }

        private void ButtonDeleteFactoryShipping_Click(object sender, RoutedEventArgs e)
        {
            var factoryShipping = (FactoryShippingName)(DataGridFactoryShipping).SelectedItem;
            var count = ProcessModule.DeleteFactoryShipping(factoryShipping.ShippingNo);


            var factoryShippingList = ProcessModule.GetFactoryShipping(factoryShipping.OrderColorDetailNo);
            DataGridFactoryShipping.ItemsSource = factoryShippingList;

            var processOrder = (ProcessOrder)DataGridProcessOrder.SelectedItem;
            UpdateDataGridOrderColorFactoryShippingDetail(processOrder.OrderNo);
        }

        private void UpdateDataGridOrderColorFactoryShippingDetail(int processOrderNo)
        {
            var processOrderColorFactoryShippingDetail = ProcessModule.GetProcessOrderColorFactoryShippingDetail(processOrderNo);
            DataGridOrderColorFactoryShippingDetail.ItemsSource = processOrderColorFactoryShippingDetail;
        }

        private void SomeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = this.DataGridOrderColorFactoryShippingDetail.CurrentItem as ProcessOrderColorDetail;
            if (selectedItem == null)
                return;
            var comboBox = sender as ComboBox;
            var x = comboBox.SelectedItem.ToString();
            ProcessOrderColorStatus status;
            Enum.TryParse(x, out status);

            var orderColorDetailNo = selectedItem.OrderColorDetailNo;
            int successCount = ProcessModule.UpdateProcessOrderColorStatus(orderColorDetailNo, status);

        }

        private void ComboBoxStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProcessOrderColorStatus status;
            Enum.TryParse(ComboBoxStatus.SelectedItem.ToString(), out status);
            _processOrderColorStatus = status;
            DataGridProcessOrder.ItemsSource = ProcessModule.GetProcessOrderByStatus(status);

            DataGridFactoryList.SelectedIndex = -1;
        }

        private void DatePickerInputDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var processOrderFlowDateDetail = DataGridProcessOrderFlowDateDetail.SelectedItem as ProcessOrderFlowDateDetail;
            if (processOrderFlowDateDetail == null)
            {
                return;
            }
            var dataGridFactoryShippingDetailItems = DataGridOrderColorFactoryShippingDetail.SelectedItems;
            List<ProcessOrderColorFactoryShippingDetail> factoryShippingDetails = new List<ProcessOrderColorFactoryShippingDetail>();
            factoryShippingDetails.AddRange(dataGridFactoryShippingDetailItems.Cast<ProcessOrderColorFactoryShippingDetail>());

            int orderFlowNo = processOrderFlowDateDetail.OrderFlowNo;
            IEnumerable<int> orderColorDetailNoList = factoryShippingDetails.Select(s => s.OrderColorDetailNo);

            var datePicker = (DatePicker)sender;
            var date = datePicker.SelectedDate;
            int count = ProcessModule.UpdateProcessOrderFlowInputDate(orderFlowNo, orderColorDetailNoList, date);
        }

        private void DatePickerCompleteDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var processOrderFlowDateDetail = DataGridProcessOrderFlowDateDetail.SelectedItem as ProcessOrderFlowDateDetail;
            if (processOrderFlowDateDetail == null)
            {
                return;
            }
            var dataGridFactoryShippingDetailItems = DataGridOrderColorFactoryShippingDetail.SelectedItems;
            List<ProcessOrderColorFactoryShippingDetail> factoryShippingDetails = new List<ProcessOrderColorFactoryShippingDetail>();
            factoryShippingDetails.AddRange(dataGridFactoryShippingDetailItems.Cast<ProcessOrderColorFactoryShippingDetail>());

            int orderFlowNo = processOrderFlowDateDetail.OrderFlowNo;
            IEnumerable<int> orderColorDetailNoList = factoryShippingDetails.Select(s => s.OrderColorDetailNo);

            var datePicker = (DatePicker)sender;
            var date = datePicker.SelectedDate;
            bool success = ProcessModule.UpdateProcessOrderFlowCompleteDate(orderFlowNo, orderColorDetailNoList, date);
            if (success)
            {
                bool updateStatusSuccess = ProcessModule.UpdateProcessOrderColorDetailStatusByLastComplete(orderFlowNo, orderColorDetailNoList);
                UpdateDataGridOrderColorFactoryShippingDetail(factoryShippingDetails.First().OrderNo);
            }
        }

        private void DataGridFactoryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dataGrid = (DataGrid)sender;
            var x = dataGrid.SelectedItems;
            if (x.Count == 0)
            {
                return;
            }
            List<ProcessOrderColorStatus> statusList = new List<ProcessOrderColorStatus>
            {
                _processOrderColorStatus
            };
            List<Factory> factoryList = new List<Factory>();
            factoryList.AddRange(x.Cast<Factory>());
            IEnumerable<ProcessOrder> y = ProcessModule.GetProcessOrderFilter(factoryList, statusList);
            DataGridProcessOrder.ItemsSource = y;
        }

        private void ButtonNewColor_Click(object sender, RoutedEventArgs e)
        {

            if (DataGridProcessOrder.SelectedIndex == -1)
            {
                MessageBox.Show("請選擇一筆訂單!!");
                return;
            }
            ProcessOrderColorFactoryShippingDetail processOrderColorFactoryShippingDetail = DataGridOrderColorFactoryShippingDetail.SelectedItem as ProcessOrderColorFactoryShippingDetail ?? new ProcessOrderColorFactoryShippingDetail();
            var processOrder = DataGridProcessOrder.SelectedItem as ProcessOrder;
            NewProcessOrderColorDetaiDialog dialog = new NewProcessOrderColorDetaiDialog(processOrder, processOrderColorFactoryShippingDetail);
            dialog.DataContext = this;
            dialog.Show();
        }

        private void DataGridFactoryShippingDetail_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column.SortMemberPath == "Quantity")
            {
                var editTextBox = e.EditingElement as TextBox;
                var quantity = editTextBox.Text.ToInt();
                var processOrderColorFactoryShippingDetail = e.Row.Item as ProcessOrderColorFactoryShippingDetail;
                var orderColorDetailNo = processOrderColorFactoryShippingDetail.OrderColorDetailNo;

                int count = ProcessModule.UpdateProcessOrderColorDetail(orderColorDetailNo, quantity);
            }
        }

        private void ButtonDeleteFactoryShippingDetail_Click(object sender, RoutedEventArgs e)
        {
            ProcessOrderColorFactoryShippingDetail processOrderColorFactoryShippingDetail = DataGridOrderColorFactoryShippingDetail.SelectedItem as ProcessOrderColorFactoryShippingDetail;
            if (MessageBox.Show(string.Concat("是否刪除'", processOrderColorFactoryShippingDetail.Color, "'??"), "刪除", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                int count = ProcessModule.DeleteFactoryShippingDetail(processOrderColorFactoryShippingDetail.OrderColorDetailNo);
                DataGridRefresh<ProcessOrderColorFactoryShippingDetail>(DataGridOrderColorFactoryShippingDetail, processOrderColorFactoryShippingDetail);
            }
        }

        private void DataGridRefresh<T>(DataGrid dataGrid, T processOrderColorFactoryShippingDetail)
        {
            var items = dataGrid.ItemsSource as List<T>;
            items.Remove(processOrderColorFactoryShippingDetail);
            DataGridOrderColorFactoryShippingDetail.ItemsSource = items;
            DataGridOrderColorFactoryShippingDetail.Items.Refresh();
        }

        public void Test()
        {

        }

        private void DataGridProcessOrderFlowDateDetail_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataGridProcessOrderFlowDateDetail.SelectedIndex == -1)
            {
                MessageBox.Show("未選擇一筆資料！！");
                return;
            }
            var selected = DataGridProcessOrderFlowDateDetail.SelectedItem as ProcessOrderFlowDateDetail;
            EditProcessOrderFlowFactoryNameDialog editProcessOrderFlowFactoryNameDialog = new EditProcessOrderFlowFactoryNameDialog(selected);
            DataGridProcessOrderFlowDateDetail.CancelEdit();
            editProcessOrderFlowFactoryNameDialog.DataContext = this;
            editProcessOrderFlowFactoryNameDialog.Show();
        }

        private void ButtonUpdateProcessOrderRemark_Click(object sender, RoutedEventArgs e)
        {
            TextRange remark = new TextRange(RichTextBoxProcessOrderRemark.Document.ContentStart, RichTextBoxProcessOrderRemark.Document.ContentEnd);
            int processOrderNo = (DataGridProcessOrder.SelectedItem as ProcessOrder).OrderNo;
            bool success = ProcessModule.UpdateProcessOrderRemark(processOrderNo, remark.Text);
        }

        private void ButtonAddFactory_Click(object sender, RoutedEventArgs e)
        {

            if (DataGridProcessOrder.SelectedIndex == -1)
            {
                MessageBox.Show("請選擇一筆訂單!!");
                return;
            }
            if (DataGridOrderColorFactoryShippingDetail.SelectedIndex == -1)
            {
                MessageBox.Show("請選擇一種顏色!!");
                return;
            }

            AddProcessOrderFlowFactoryDialog dialog = new AddProcessOrderFlowFactoryDialog();
            dialog.DataContext = this;
            dialog.Show();
        }

    }
}
