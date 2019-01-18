using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class TestDataGrid :DataGrid
    {
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
        }
    }
    /// <summary>
    /// ProcessOrderPage.xaml 的互動邏輯
    /// </summary>
    public partial class ProcessOrderPage : Page
    {

        protected IProcessModule ProcessModule { get; } = new ProcessModule();
        protected ICustomerModule CustomerModule { get; } = new CustomerModule();
        protected IFactoryModule FactoryModule { get; } = new FactoryModule();

        public ObservableCollection<ProcessOrder> DataGridProcessOrderCollection { get; set; }
        public ProcessOrderPage()
        {
            InitializeComponent();

            ComboBoxStatus.ItemsSource = Enum.GetValues(typeof(ProcessOrderColorStatus)).Cast<ProcessOrderColorStatus>();
            DataGridProcessOrderCollection = new ObservableCollection<ProcessOrder>(ProcessModule.GetProcessOrder());
            DataGridProcessOrder.ItemsSource = DataGridProcessOrderCollection;
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

            DataGridFactoryShipping.ItemsSource = null;
            //ComboBoxCustomer.SelectedIndex = -1;
            //TextBoxQuantity.Text = string.Empty;
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
                //ProcessOrderColorStatus status;
                //Enum.TryParse(ComboBoxStatus.SelectedIndex == -1 ? "" : ComboBoxStatus.SelectedItem.ToString(), out status);
                ProcessModule.DeleteProcessOrder(processOrder);
                int selectedIndex = DataGridProcessOrder.SelectedIndex - 1;
                DataGridProcessOrderCollection.Remove(processOrder);
                DataGridProcessOrder.SelectedIndex = selectedIndex;
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
        private void TextBoxColorSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            ButtonDisplayAllOrder_Click(sender, e);
        }

        private void TextBoxColorSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox textBox = (TextBox)sender;
                string color = textBox.Text;
                DataGridProcessOrderCollection = new ObservableCollection<ProcessOrder>(ProcessModule.GetProcessOrderByColor(color));
                DataGridProcessOrder.ItemsSource = DataGridProcessOrderCollection;
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
            var processOrderColorDetail = (ProcessOrderColorFactoryShippingDetail)DataGridOrderColorFactoryShippingDetail.SelectedItem;
            var factoryShipping = new FactoryShippingName
            {
                OrderColorDetailNo = processOrderColorDetail.OrderColorDetailNo,
                Name = ComboBoxCustomer.SelectedItem.ToString(),
                Quantity = TextBoxQuantity.Text.ToInt(),
                CreateDate = DateTime.Now,
                ShippingDate = DatePickerShippingDate.SelectedDate
            };

            var count = ProcessModule.InsertFactoryShipping(factoryShipping);
            if (processOrderColorDetail.ShippingQuantity + factoryShipping.Quantity == processOrderColorDetail.Quantity)
            {
                ProcessModule.UpdateProcessOrderColorStatus(processOrderColorDetail.OrderColorDetailNo, ProcessOrderColorStatus.已出完);
            }

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

        public void UpdateDataGridOrderColorFactoryShippingDetail(int processOrderNo)
        {
            var processOrderColorFactoryShippingDetail = ProcessModule.GetProcessOrderColorFactoryShippingDetail(processOrderNo);
            DataGridOrderColorFactoryShippingDetail.ItemsSource = processOrderColorFactoryShippingDetail;

            DataGridOrderColorFactoryShippingDetail.UpdateLayout();

            foreach (ProcessOrderColorFactoryShippingDetail item in DataGridOrderColorFactoryShippingDetail.ItemsSource)
            {
                var row = DataGridOrderColorFactoryShippingDetail.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                
                if (item.Status == ProcessOrderColorStatus.已出完)
                {
                    row.Background = Brushes.Gray;
                }
                else if (item.Status == ProcessOrderColorStatus.已完成)
                {
                    row.Background = Brushes.Pink;
                }
            }
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
            if (DataGridFactoryList.SelectedIndex == -1)
            {
                ProcessOrderColorStatus status;
                Enum.TryParse(ComboBoxStatus.SelectedIndex == -1 ? "" : ComboBoxStatus.SelectedItem.ToString(), out status);
                DataGridProcessOrderCollection = new ObservableCollection<ProcessOrder>(ProcessModule.GetProcessOrderByStatus(status));
                DataGridProcessOrder.ItemsSource = DataGridProcessOrderCollection;
            }
            else
            {
                var selectedItems = DataGridFactoryList.SelectedItems;
                if (selectedItems.Count == 0)
                {
                    return;
                }
                ProcessOrderColorStatus status;
                Enum.TryParse(ComboBoxStatus.SelectedIndex != -1 ? ComboBoxStatus.SelectedItem.ToString() : "", out status);
                List<ProcessOrderColorStatus> statusList = status == 0
                    ? new List<ProcessOrderColorStatus>
                        {
                        ProcessOrderColorStatus.修訂,
                        ProcessOrderColorStatus.已出完,
                        ProcessOrderColorStatus.已完成,
                        ProcessOrderColorStatus.未完成,
                        ProcessOrderColorStatus.緊急
                        }
                    : new List<ProcessOrderColorStatus> { status };
                List<Factory> factoryList = new List<Factory>();
                factoryList.AddRange(selectedItems.Cast<Factory>());
                IEnumerable<ProcessOrder> processOrderList = ProcessModule.GetProcessOrderFilter(factoryList, statusList);
                DataGridProcessOrderCollection = new ObservableCollection<ProcessOrder>(processOrderList);
                DataGridProcessOrder.ItemsSource = DataGridProcessOrderCollection;
            }
            //DataGridFactoryList.SelectedIndex = -1;
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
            if (dataGridFactoryShippingDetailItems.Count == 0)
            {
                return;
            }
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
            ProcessOrderColorStatus status;
            Enum.TryParse(ComboBoxStatus.SelectedIndex != -1 ? ComboBoxStatus.SelectedItem.ToString() : "", out status);
            List<ProcessOrderColorStatus> statusList = status == 0
                ? new List<ProcessOrderColorStatus>
                    {
                        ProcessOrderColorStatus.修訂,
                        ProcessOrderColorStatus.已出完,
                        ProcessOrderColorStatus.已完成,
                        ProcessOrderColorStatus.未完成,
                        ProcessOrderColorStatus.緊急
                    }
                : new List<ProcessOrderColorStatus> { status };
            List<Factory> factoryList = new List<Factory>();
            factoryList.AddRange(x.Cast<Factory>());
            IEnumerable<ProcessOrder> processOrderList = ProcessModule.GetProcessOrderFilter(factoryList, statusList);
            DataGridProcessOrderCollection = new ObservableCollection<ProcessOrder>(processOrderList);
            DataGridProcessOrder.ItemsSource = DataGridProcessOrderCollection;
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
            if (e.EditAction == DataGridEditAction.Cancel)
            {
                return;
            }
            if (e.Column.SortMemberPath == "Quantity")
            {
                var editTextBox = e.EditingElement as TextBox;
                var quantity = editTextBox.Text.ToInt();
                var processOrderColorFactoryShippingDetail = e.Row.Item as ProcessOrderColorFactoryShippingDetail;
                var orderColorDetailNo = processOrderColorFactoryShippingDetail.OrderColorDetailNo;
                MessageBoxResult mr = MessageBox.Show(string.Concat("是否將疋數'", processOrderColorFactoryShippingDetail.Quantity, "'改為'", quantity, "'?"), "修改確認", MessageBoxButton.YesNo);
                if (mr == MessageBoxResult.Yes)
                {
                    int count = ProcessModule.UpdateProcessOrderColorDetailQuantity(orderColorDetailNo, quantity);
                }
                else
                {
                    DataGridOrderColorFactoryShippingDetail.CancelEdit();
                }
            }
            else if (e.Column.SortMemberPath == "Color")
            {
                var editTextBox = e.EditingElement as TextBox;
                var color = editTextBox.Text;
                var processOrderColorFactoryShippingDetail = e.Row.Item as ProcessOrderColorFactoryShippingDetail;
                var orderColorDetailNo = processOrderColorFactoryShippingDetail.OrderColorDetailNo;
                MessageBoxResult mr = MessageBox.Show(string.Concat("是否將顏色'", processOrderColorFactoryShippingDetail.Color, "'改為'", color, "'?"), "修改確認", MessageBoxButton.YesNo);
                if (mr == MessageBoxResult.Yes)
                {
                    int count = ProcessModule.UpdateProcessOrderColorDetailColor(orderColorDetailNo, color);
                }
                else
                {
                    DataGridOrderColorFactoryShippingDetail.CancelEdit();
                }
            }
            else if (e.Column.SortMemberPath == "ColorNumber")
            {
                var editTextBox = e.EditingElement as TextBox;
                var colorNumber = editTextBox.Text;
                var processOrderColorFactoryShippingDetail = e.Row.Item as ProcessOrderColorFactoryShippingDetail;
                var orderColorDetailNo = processOrderColorFactoryShippingDetail.OrderColorDetailNo;
                MessageBoxResult mr = MessageBox.Show(string.Concat("是否將色號'", processOrderColorFactoryShippingDetail.ColorNumber, "'改為'", colorNumber, "'?"), "修改確認", MessageBoxButton.YesNo);
                if (mr == MessageBoxResult.Yes)
                {
                    int count = ProcessModule.UpdateProcessOrderColorDetailColorNumber(orderColorDetailNo, colorNumber);
                }
                else
                {
                    DataGridOrderColorFactoryShippingDetail.CancelEdit();
                }
            }
        }

        private void ButtonDeleteFactoryShippingDetail_Click(object sender, RoutedEventArgs e)
        {
            ProcessOrderColorFactoryShippingDetail processOrderColorFactoryShippingDetail = DataGridOrderColorFactoryShippingDetail.SelectedItem as ProcessOrderColorFactoryShippingDetail;
            if (MessageBox.Show(string.Concat("是否刪除'", processOrderColorFactoryShippingDetail.Color, "'??"), "刪除", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                int count = ProcessModule.DeleteFactoryShippingDetail(processOrderColorFactoryShippingDetail.OrderColorDetailNo);
                UpdateDataGridOrderColorFactoryShippingDetail(processOrderColorFactoryShippingDetail.OrderNo);
            }
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
            editProcessOrderFlowFactoryNameDialog.EditProcessOrderFlowFactoryExecute += EditProcessOrderFlowFactoryExecute;
            DataGridProcessOrderFlowDateDetail.CancelEdit();
            editProcessOrderFlowFactoryNameDialog.Show();
        }
        private void EditProcessOrderFlowFactoryExecute(Factory selectedFactory)
        {
            bool success = ProcessModule.EditProcessOrderFlowFactory(selectedFactory.FactoryID, (DataGridProcessOrderFlowDateDetail.SelectedItem as ProcessOrderFlowDateDetail).OrderFlowNo);
            if (success)
            {
                var processOrderFlowDateDetail = DataGridProcessOrderFlowDateDetail.SelectedItem as ProcessOrderFlowDateDetail;
                processOrderFlowDateDetail.Name = selectedFactory.Name;
                DataGridProcessOrderFlowDateDetail.CommitEdit();
                DataGridProcessOrderFlowDateDetail.Items.Refresh();
                //暫時以此方式解決，避免更新工廠名稱時會同時更新顏色狀態
                DataGridProcessOrderFlowDateDetail.SelectedIndex = -1;
            }
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

        private void ButtonDisplayAllOrder_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxStatus.SelectedIndex = -1;
            DataGridFactoryList.SelectedIndex = -1;
            DataGridProcessOrderCollection = new ObservableCollection<ProcessOrder>(ProcessModule.GetProcessOrder());
            DataGridProcessOrder.ItemsSource = DataGridProcessOrderCollection;
        }

        private void ButtonUpdateDateOrder_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<ProcessOrder> processOrderList = ProcessModule.GetProcessOrderByFactoryUpdateDate(DateTime.Now.ToShortDateString());
            DataGridProcessOrderCollection = new ObservableCollection<ProcessOrder>(processOrderList);
            DataGridProcessOrder.ItemsSource = DataGridProcessOrderCollection;
        }


    }
}
