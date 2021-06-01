using Newtonsoft.Json;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using WpfApp1.DataClass.Entity;
using WpfApp1.DataClass.Entity.ProcessOrderFile;
using WpfApp1.DataClass.Enumeration;
using WpfApp1.DataClass.ExcelDataClass;
using WpfApp1.DataClass.Fabric;
using WpfApp1.DataClass.ProcessOrder;
using WpfApp1.Modules.CustomerModule;
using WpfApp1.Modules.CustomerModule.Implement;
using WpfApp1.Modules.ExcelModule;
using WpfApp1.Modules.ExcelModule.Implement;
using WpfApp1.Modules.FactoryModule;
using WpfApp1.Modules.FactoryModule.Implement;
using WpfApp1.Modules.Process;
using WpfApp1.Modules.Process.Implement;
using WpfApp1.Utility;
using WpfApp1.Windows;
using WpfApp1.Windows.InventoryWindows;
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

        public ObservableCollection<ProcessOrder> DataGridProcessOrderCollection { get; set; }
        public ProcessOrderPage()
        {
            InitializeComponent();

            ComboBoxStatus.ItemsSource = Enum.GetValues(typeof(ProcessOrderColorStatus)).Cast<ProcessOrderColorStatus>();
            DisplayAllOrder();
            DataGridProcessOrder.ItemsSource = DataGridProcessOrderCollection;
            ComboBoxCustomer.ItemsSource = CustomerModule.GetCustomerList();
            ComboBoxCustomerNameSearch.ItemsSource = CustomerModule.GetCustomerList();
            DataGridFactoryList.ItemsSource = FactoryModule.GetFactoryList();
        }

        #region 舊的combobox filter 邏輯
        //private void OnComboBoxCustomerLoad()
        //{
        //    ComboBoxCustomer.Loaded += (ls, le) =>
        //    {
        //        TextBox targetTextBox = ComboBoxCustomer?.Template.FindName("PART_EditableTextBox", ComboBoxCustomer) as TextBox;

        //        if (targetTextBox == null) return;

        //        ComboBoxCustomer.Tag = "TextInput";
        //        ComboBoxCustomer.StaysOpenOnEdit = true;
        //        ComboBoxCustomer.IsEditable = true;
        //        ComboBoxCustomer.IsTextSearchEnabled = false;

        //        targetTextBox.TextChanged += (o, args) =>
        //        {
        //            if (ComboBoxCustomer.Tag.ToString() == "Selection")
        //            {
        //                ComboBoxCustomer.Tag = "TextInput";
        //                ComboBoxCustomer.IsDropDownOpen = true;
        //            }
        //            else
        //            {
        //                TextBox textBox = (TextBox)o;
        //                string searchText = textBox.Text;

        //                if (ComboBoxCustomer.SelectionBoxItem != null)
        //                {
        //                    //ComboBoxCustomer.SelectedItem = null;
        //                    targetTextBox.Text = searchText;
        //                    ComboBoxCustomer.IsDropDownOpen = true;
        //                    targetTextBox.SelectionStart = targetTextBox.Text.Length;
        //                }

        //                if (string.IsNullOrEmpty(searchText))
        //                {
        //                    ComboBoxCustomer.Items.Filter = item => true;
        //                    ComboBoxCustomer.SelectedItem = default(object);
        //                }
        //                else
        //                {
        //                    ComboBoxCustomer.Items.Filter = item =>
        //                            ((Customer)item).Name.Contains(searchText);
        //                }
        //                ComboBoxCustomer.IsDropDownOpen = true;
        //                targetTextBox.SelectionStart = targetTextBox.Text.Length;
        //            }
        //        };

        //        ComboBoxCustomer.SelectionChanged += (o, args) =>
        //        {
        //            ComboBox comboBox = o as ComboBox;
        //            if (comboBox?.SelectedItem == null) return;
        //            comboBox.Tag = "Selection";
        //        };
        //    };
        //}
        #endregion

        private void DataGridProcessOrder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGridProcessOrder = (DataGrid)sender;
            if (dataGridProcessOrder.SelectedIndex == -1)
            {
                return;
            }

            ProcessOrder processOrder = dataGridProcessOrder.SelectedItem as ProcessOrder;

            IEnumerable<ProcessOrderCustomerRelate> customerOrderRelate = ProcessModule.GetCustomerByOrderNo(processOrder.OrderNo);
            DataGridCustomerOrder.ItemsSource = customerOrderRelate;

            TextRange remark = new TextRange(RichTextBoxProcessOrderRemark.Document.ContentStart, RichTextBoxProcessOrderRemark.Document.ContentEnd)
            {
                Text = processOrder.Remark ?? ""
            };

            UpdateDataGridOrderColorFactoryShippingDetail(processOrder.OrderNo);

            DataGridFactoryShipping.ItemsSource = null;
            DataGridProcessOrderFlowDateDetail.ItemsSource = null;

            if (CheckboxDisplayInventory.IsChecked ?? false)
            {
                var textileNameMapping = TextileNameMappings.ToList().Find(f => f.ProcessOrder.Contains(processOrder.Fabric));
                if (textileNameMapping == null)
                {
                    InventoryListDialog.ChangeDataContext(AppSettingConfig.StoreManageFileName(), null, null);
                    return;
                }
                ExcelHelper excelHelper = new ExcelHelper();
                List<TextileColorInventory> selectedTextiles = new List<TextileColorInventory>();
                foreach (var item in textileNameMapping.Inventory)
                {
                    selectedTextiles.AddRange(excelHelper.GetInventoryData(Workbook, item));
                }
                TextileInventoryHeader textileInventoryHeader = ExcelModule.GetShippingDate(Workbook.GetSheetAt(1));
                textileInventoryHeader.Textile = processOrder.Fabric;
                InventoryListDialog.ChangeDataContext(AppSettingConfig.StoreManageFileName(), textileInventoryHeader, selectedTextiles);
            }
        }



        private void ButtonDeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            ProcessOrder processOrder = (ProcessOrder)DataGridProcessOrder.SelectedItem;
            if (processOrder == null)
            {
                MessageBox.Show("未選取訂單!!");
                return;
            }
            MessageBoxResult result = MessageBox.Show(string.Concat("請確認是否要刪除訂單編號:", processOrder.OrderString, ",布種:", processOrder.Fabric), "刪除", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
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
                DataGridProcessOrderCollection = new ObservableCollection<ProcessOrder>(ProcessModule.GetProcessOrderFilter(null, null, CheckBoxContainFinish.IsChecked ?? false, color));
                DataGridProcessOrder.ItemsSource = DataGridProcessOrderCollection;
            }
        }

        private void DataGridProcessOrderColorDetail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dataGrid = (DataGrid)sender;

            var processOrderColorDetail = (ProcessOrderColorDetail)dataGrid.SelectedItem;
            if (processOrderColorDetail == null)
                return;
            if (InventoryListDialog != null)
            {
                InventoryListDialog.InventoryListViewModel.FilterColorName(processOrderColorDetail.Color);
            }
            var factoryShippingList = ProcessModule.GetFactoryShipping(processOrderColorDetail.OrderColorDetailNo);
            DataGridFactoryShipping.ItemsSource = factoryShippingList;

            List<int> orderColorDetailNos = new List<int> { processOrderColorDetail.OrderColorDetailNo };
            RefrashDataGridProcessOrderFlowDateDetail(orderColorDetailNos);
        }

        public void RefrashDataGridProcessOrderFlowDateDetail(List<int> orderColorDetailNos)
        {
            IEnumerable<ProcessOrderFlowDateDetail> processOrderFlowDateDetail = ProcessModule.GetProcessOrderFlowDateDetail(orderColorDetailNos);
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
            ProcessOrderColorFactoryShippingDetail processOrderColorDetail = (ProcessOrderColorFactoryShippingDetail)DataGridOrderColorFactoryShippingDetail.SelectedItem;
            Customer customer = ComboBoxCustomer.SelectedItem as Customer;
            FactoryShippingName factoryShipping = new FactoryShippingName
            {
                OrderColorDetailNo = processOrderColorDetail.OrderColorDetailNo,
                CustomerID = customer.CustomerID,
                Quantity = TextBoxQuantity.Text.ToInt(),
                CreateDate = DateTime.Now,
                ShippingDate = DatePickerShippingDate.SelectedDate
            };

            int count = ProcessModule.InsertFactoryShipping(factoryShipping);
            if (processOrderColorDetail.ShippingQuantity + factoryShipping.Quantity == processOrderColorDetail.Quantity)
            {
                ProcessModule.UpdateProcessOrderColorStatus(processOrderColorDetail.OrderColorDetailNo, ProcessOrderColorStatus.已出完);
            }

            IEnumerable<FactoryShippingName> factoryShippingList = ProcessModule.GetFactoryShipping(processOrderColorDetail.OrderColorDetailNo);
            DataGridFactoryShipping.ItemsSource = factoryShippingList;

            UpdateDataGridOrderColorFactoryShippingDetail(processOrderColorDetail.OrderNo);
        }

        private void ButtonDeleteFactoryShipping_Click(object sender, RoutedEventArgs e)
        {
            FactoryShippingName factoryShipping = (FactoryShippingName)(DataGridFactoryShipping).SelectedItem;
            int count = ProcessModule.DeleteFactoryShipping(factoryShipping.ShippingNo);

            IEnumerable<FactoryShippingName> factoryShippingList = ProcessModule.GetFactoryShipping(factoryShipping.OrderColorDetailNo);
            DataGridFactoryShipping.ItemsSource = factoryShippingList;

            ProcessOrder processOrder = (ProcessOrder)DataGridProcessOrder.SelectedItem;
            UpdateDataGridOrderColorFactoryShippingDetail(processOrder.OrderNo);
        }

        public void UpdateDataGridOrderColorFactoryShippingDetail(int processOrderNo)
        {
            ObservableCollection<ProcessOrderColorFactoryShippingDetail> processOrderColorFactoryShippingDetail = new ObservableCollection<ProcessOrderColorFactoryShippingDetail>(ProcessModule.GetProcessOrderColorFactoryShippingDetail(processOrderNo));
            DataGridOrderColorFactoryShippingDetail.ItemsSource = processOrderColorFactoryShippingDetail;
        }

        private void SomeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProcessOrderColorDetail selectedItem = this.DataGridOrderColorFactoryShippingDetail.CurrentItem as ProcessOrderColorDetail;
            if (selectedItem == null)
                return;
            ComboBox comboBox = sender as ComboBox;
            string x = comboBox.SelectedItem.ToString();
            ProcessOrderColorStatus status;
            Enum.TryParse(x, out status);

            int orderColorDetailNo = selectedItem.OrderColorDetailNo;
            int successCount = ProcessModule.UpdateProcessOrderColorStatus(orderColorDetailNo, status);
        }

        private void ComboBoxStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetProcessOrderByFactoryAndStatus();
        }

        private void GetProcessOrderByFactoryAndStatus()
        {
            ProcessOrderColorStatus status;
            Enum.TryParse(ComboBoxStatus.SelectedIndex != -1 ? ComboBoxStatus.SelectedItem.ToString() : "", out status);
            List<ProcessOrderColorStatus> statusList = status == 0
                ? new List<ProcessOrderColorStatus>
                    {
                        ProcessOrderColorStatus.修訂,
                        ProcessOrderColorStatus.已出完,
                        ProcessOrderColorStatus.已完成,
                        ProcessOrderColorStatus.未完成,
                        ProcessOrderColorStatus.緊急,
                        ProcessOrderColorStatus.在染缸,
                        ProcessOrderColorStatus.定型中,
                        ProcessOrderColorStatus.已排染,
                        ProcessOrderColorStatus.待定型
                    }
                : new List<ProcessOrderColorStatus> { status };
            List<Factory> factoryList = new List<Factory>();

            var selectedItems = DataGridFactoryList.SelectedItems;
            if (selectedItems.Count != 0 && selectedItems != null)
                factoryList.AddRange(selectedItems.Cast<Factory>());

            IEnumerable<ProcessOrder> processOrderList = ProcessModule.GetProcessOrderFilter(factoryList, statusList, CheckBoxContainFinish.IsChecked ?? false, string.Empty);
            DataGridProcessOrderCollection = new ObservableCollection<ProcessOrder>(processOrderList);
            DataGridProcessOrder.ItemsSource = DataGridProcessOrderCollection;
        }

        private void DatePickerInputDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ProcessOrderFlowDateDetail processOrderFlowDateDetail = DataGridProcessOrderFlowDateDetail.SelectedItem as ProcessOrderFlowDateDetail;
            if (processOrderFlowDateDetail == null)
            {
                return;
            }
            var dataGridFactoryShippingDetailItems = DataGridOrderColorFactoryShippingDetail.SelectedItems;
            List<ProcessOrderColorFactoryShippingDetail> factoryShippingDetails = new List<ProcessOrderColorFactoryShippingDetail>();
            factoryShippingDetails.AddRange(dataGridFactoryShippingDetailItems.Cast<ProcessOrderColorFactoryShippingDetail>());

            int orderFlowNo = processOrderFlowDateDetail.OrderFlowNo;
            IEnumerable<int> orderColorDetailNoList = factoryShippingDetails.Select(s => s.OrderColorDetailNo);

            DatePicker datePicker = (DatePicker)sender;
            DateTime? date = datePicker.SelectedDate;
            int count = ProcessModule.UpdateProcessOrderFlowInputDate(orderFlowNo, orderColorDetailNoList, date);
        }

        private void DatePickerCompleteDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ProcessOrderFlowDateDetail processOrderFlowDateDetail = DataGridProcessOrderFlowDateDetail.SelectedItem as ProcessOrderFlowDateDetail;
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

            DatePicker datePicker = (DatePicker)sender;
            DateTime? date = datePicker.SelectedDate;
            bool success = ProcessModule.UpdateProcessOrderFlowCompleteDate(orderFlowNo, orderColorDetailNoList, date);
            if (success)
            {
                bool updateStatusSuccess = ProcessModule.UpdateProcessOrderColorDetailStatusByLastComplete(orderFlowNo, orderColorDetailNoList);
                UpdateDataGridOrderColorFactoryShippingDetail(factoryShippingDetails.First().OrderNo);
            }
        }

        private void DataGridFactoryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetProcessOrderByFactoryAndStatus();
        }

        private void ButtonNewColor_Click(object sender, RoutedEventArgs e)
        {

            if (DataGridProcessOrder.SelectedIndex == -1)
            {
                MessageBox.Show("請選擇一筆訂單!!");
                return;
            }
            ProcessOrderColorFactoryShippingDetail processOrderColorFactoryShippingDetail = DataGridOrderColorFactoryShippingDetail.SelectedItem as ProcessOrderColorFactoryShippingDetail ?? new ProcessOrderColorFactoryShippingDetail();
            ProcessOrder processOrder = DataGridProcessOrder.SelectedItem as ProcessOrder;
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
                TextBox editTextBox = e.EditingElement as TextBox;
                int quantity = editTextBox.Text.ToInt();
                ProcessOrderColorFactoryShippingDetail processOrderColorFactoryShippingDetail = e.Row.Item as ProcessOrderColorFactoryShippingDetail;
                int orderColorDetailNo = processOrderColorFactoryShippingDetail.OrderColorDetailNo;
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
                TextBox editTextBox = e.EditingElement as TextBox;
                string color = editTextBox.Text;
                ProcessOrderColorFactoryShippingDetail processOrderColorFactoryShippingDetail = e.Row.Item as ProcessOrderColorFactoryShippingDetail;
                int orderColorDetailNo = processOrderColorFactoryShippingDetail.OrderColorDetailNo;
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
                TextBox editTextBox = e.EditingElement as TextBox;
                string colorNumber = editTextBox.Text;
                ProcessOrderColorFactoryShippingDetail processOrderColorFactoryShippingDetail = e.Row.Item as ProcessOrderColorFactoryShippingDetail;
                int orderColorDetailNo = processOrderColorFactoryShippingDetail.OrderColorDetailNo;
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

        private void ButtonDelivery_Click(object sender, RoutedEventArgs e)
        {
            var textileColorInventory = InventoryListDialog?.InventoryListViewModel.TextileColor ?? null;
            ProcessOrderColorDetail processOrderColorDetail = (ProcessOrderColorDetail)(DataGridOrderColorFactoryShippingDetail.SelectedItem);
            ProcessOrder processOrder = (ProcessOrder)(DataGridProcessOrder.SelectedItem);
            DeliveryNumberCheckDialog deliveryNumberCheckDialog = new DeliveryNumberCheckDialog(processOrder.OrderString, processOrder.Fabric, processOrderColorDetail, textileColorInventory);
            deliveryNumberCheckDialog.Show();
            deliveryNumberCheckDialog.Closed += DeliveryListDialogExecute;
        }
        public DeliveryListDialog DeliveryListDialog;
        private void DeliveryListDialogExecute(object sender, EventArgs e)
        {
            DeliveryNumberCheckDialog deliveryNumberCheckDialog = (DeliveryNumberCheckDialog)sender;
            if (DeliveryListDialog == null)
            {
                DeliveryListDialog = new DeliveryListDialog(deliveryNumberCheckDialog.IsCheck == true ? deliveryNumberCheckDialog.processOrderDelivery : null);
                DeliveryListDialog.Show();
                DeliveryListDialog.Closed += DeliveryListDialogClosed;
            }
            else
            {
                if (deliveryNumberCheckDialog.IsCheck == true)
                    DeliveryListDialog.ProcessOrderColorDetailChanged(deliveryNumberCheckDialog.processOrderDelivery);
            }
        }
        private void DeliveryListDialogClosed(object sender, EventArgs e)
        {
            DeliveryListDialog = null;
        }
        private void DataGridProcessOrderFlowDateDetail_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataGridProcessOrderFlowDateDetail.SelectedIndex == -1)
            {
                MessageBox.Show("未選擇一筆資料！！");
                return;
            }
            ProcessOrderFlowDateDetail selected = DataGridProcessOrderFlowDateDetail.SelectedItem as ProcessOrderFlowDateDetail;
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
                ProcessOrderFlowDateDetail processOrderFlowDateDetail = DataGridProcessOrderFlowDateDetail.SelectedItem as ProcessOrderFlowDateDetail;
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
            if (success)
                DataGridProcessOrderCollection.Where(w => w.OrderNo == processOrderNo).ToList().ForEach(f => f.Remark = remark.Text);
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
            DisplayAllOrder();
        }

        private void DisplayAllOrder()
        {
            ComboBoxStatus.SelectedIndex = -1;
            DataGridFactoryList.SelectedIndex = -1;
            ComboBoxCustomerNameSearch.SelectedIndex = -1;

            List<ProcessOrderColorStatus> statusList =
                new List<ProcessOrderColorStatus>
                    {
                        ProcessOrderColorStatus.修訂,
                        ProcessOrderColorStatus.已出完,
                        ProcessOrderColorStatus.已完成,
                        ProcessOrderColorStatus.未完成,
                        ProcessOrderColorStatus.緊急,
                        ProcessOrderColorStatus.在染缸,
                        ProcessOrderColorStatus.定型中,
                        ProcessOrderColorStatus.已排染,
                        ProcessOrderColorStatus.待定型
                    };
            List<Factory> factoryList = new List<Factory>();
            IEnumerable<ProcessOrder> processOrderList = ProcessModule.GetProcessOrderFilter(factoryList, statusList, CheckBoxContainFinish.IsChecked ?? false, string.Empty);
            DataGridProcessOrderCollection = new ObservableCollection<ProcessOrder>(processOrderList);
            DataGridProcessOrder.ItemsSource = DataGridProcessOrderCollection;
        }

        private void ButtonUpdateDateOrder_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<ProcessOrder> processOrderList = ProcessModule.GetProcessOrderByFactoryUpdateDate(DateTime.Now.ToShortDateString());
            DataGridProcessOrderCollection = new ObservableCollection<ProcessOrder>(processOrderList);
            DataGridProcessOrder.ItemsSource = DataGridProcessOrderCollection;
        }

        private void CustomerRelate_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataGridProcessOrder.SelectedItem is ProcessOrder processOrder))
            {
                MessageBox.Show("尚未選擇一筆訂單！！");
                return;
            }
            if (!(ComboBoxCustomer.SelectedItem is Customer customer))
            {
                MessageBox.Show("尚未選擇客戶！！");
                return;
            }
            bool inCustomerOrderRelate = ProcessModule.CheckInCustomerOrderRelate(processOrder.OrderNo, customer.CustomerID);
            if (inCustomerOrderRelate)
            {
                MessageBox.Show(string.Format("{0}已關連至{1},{2}！！", customer.Name, processOrder.OrderString, processOrder.Fabric));
                return;
            }
            CustomerOrderRelate customerOrderRelate = new CustomerOrderRelate
            {
                CustomerID = customer.CustomerID,
                ProcessOrderID = processOrder.OrderNo
            };
            bool success = ProcessModule.InsertCustomerOrderRelate(customerOrderRelate);

            if (success)
                MessageBox.Show(string.Format("成功將{0}關連至{1},{2}！！", customer.Name, processOrder.OrderString, processOrder.Fabric));
            else
                MessageBox.Show("新增錯誤！！");
        }

        private void ButtonDeleteCustomerOrder_Click(object sender, RoutedEventArgs e)
        {
            ProcessOrderCustomerRelate processOrderCustomerRelate = DataGridCustomerOrder.SelectedItem as ProcessOrderCustomerRelate;
            bool success = ProcessModule.DeleteCustomerOrderRelate(processOrderCustomerRelate.CustomerOrderID);
            if (success)
                MessageBox.Show(string.Concat("已刪除！！"));
            else
                MessageBox.Show("刪除失敗！！");
        }

        private void CheckBoxContainFinish_Click(object sender, RoutedEventArgs e)
        {
            GetProcessOrderByFactoryAndStatus();
        }

        private void ComboBoxCustomer_KeyUp(object sender, KeyEventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            CollectionView itemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView(cmb.ItemsSource);

            itemsViewOriginal.Filter = ((o) =>
            {
                if (string.IsNullOrEmpty(cmb.Text)) return false;
                else
                {
                    if (((Customer)o).Name.Contains(cmb.Text)) return true;
                    else return false;
                }
            });

            cmb.IsDropDownOpen = true;
            itemsViewOriginal.Refresh();
        }

        private void ComboBoxCustomerNameSearch_KeyUp(object sender, KeyEventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            CollectionView itemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView(cmb.ItemsSource);

            itemsViewOriginal.Filter = ((o) =>
            {
                if (string.IsNullOrEmpty(cmb.Text)) return false;
                else
                {
                    if (((Customer)o).Name.Contains(cmb.Text)) return true;
                    else return false;
                }
            });

            cmb.IsDropDownOpen = true;
            itemsViewOriginal.Refresh();
        }

        private void ComboBoxCustomerNameSearch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox?.SelectedItem == null) return;
            comboBox.Tag = "Selection";
            DataGridProcessOrderCollection = new ObservableCollection<ProcessOrder>(ProcessModule.GetProcessOrderByCustomer(((Customer)comboBox.SelectedItem).CustomerID));
            DataGridProcessOrder.ItemsSource = DataGridProcessOrderCollection;
        }

        private void DataGridOrderColorFactoryShippingDetail_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGridRow row = e.Row;
            ProcessOrderColorFactoryShippingDetail item = (ProcessOrderColorFactoryShippingDetail)row.Item;
            if (item.Status == ProcessOrderColorStatus.已出完)
            {
                row.Background = Brushes.Gray;
            }
            else if (item.Status == ProcessOrderColorStatus.已完成)
            {
                row.Background = Brushes.Pink;
            }
            else
            {
                row.Background = Brushes.White;
            }
        }

        protected IExcelModule ExcelModule { get; } = new ExcelModule();

        private IWorkbook Workbook;
        public InventoryListDialog InventoryListDialog;
        private IEnumerable<TextileNameMapping> TextileNameMappings { get; set; }
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox.IsChecked ?? false)
            {
                string fileNamePath = string.Concat(AppSettingConfig.FilePath(), "/", AppSettingConfig.StoreManageFileName());
                Tuple<List<string>, IWorkbook> tuple = ExcelModule.GetExcelWorkbook(fileNamePath);
                TextileInventoryHeader textileInventoryHeader = ExcelModule.GetShippingDate(tuple.Item2.GetSheetAt(1));
                Workbook = tuple.Item2;
                Window parentWindow = Window.GetWindow(this);
                InventoryListDialog = new InventoryListDialog(AppSettingConfig.StoreManageFileName(), textileInventoryHeader, null)
                {
                    Owner = Window.GetWindow(this),
                    Top = parentWindow.Top + parentWindow.Height,
                    Left = parentWindow.Left,
                    Height = 300
                };
                InventoryListDialog.Show();
                InventoryListDialog.Closed += InventoryListDialog_Closed;

                ExternalDataHelper externalDataHelper = new ExternalDataHelper();
                TextileNameMappings = externalDataHelper.GetTextileNameMappings();

                InventoryUpdateTime.Content = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            }
            else
            {
                InventoryListDialog.Close();
            }
        }

        private void InventoryListDialog_Closed(object sender, EventArgs e)
        {
            CheckboxDisplayInventory.IsChecked = false;
            Workbook = null;
            InventoryUpdateTime.Content = string.Empty;
            InventoryListDialog = null;
        }
    }
}
