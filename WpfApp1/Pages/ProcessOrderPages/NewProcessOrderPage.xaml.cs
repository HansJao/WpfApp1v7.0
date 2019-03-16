using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.DataClass.Entity;
using WpfApp1.DataClass.Enumeration;
using WpfApp1.DataClass.FactoryClass;
using WpfApp1.DataClass.ProcessOrder;
using WpfApp1.Modules.FactoryModule;
using WpfApp1.Modules.FactoryModule.Implement;
using WpfApp1.Modules.Process;
using WpfApp1.Modules.Process.Implement;
using WpfApp1.Pages.FactoryPages;
using WpfApp1.Utility;
using WpfApp1.ViewModel.FactoryViewModel;

namespace WpfApp1.Pages.ProcessOrderPages
{
    /// <summary>
    /// NewProcessOrderPage.xaml 的互動邏輯
    /// </summary>
    public partial class NewProcessOrderPage : Page
    {
        protected IProcessModule ProcessModule { get; } = new ProcessModule();
        protected IFactoryModule FactoryModule { get; } = new FactoryModule();



        private IWorkbook workbook = null;  //新建IWorkbook對象
        public NewProcessOrderPage()
        {
            InitializeComponent();
            string fileName = string.Concat(AppSettingConfig.ProcessOrderFilePath(), AppSettingConfig.ProcessOrderFileName());
            try
            {
                FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                workbook = new XSSFWorkbook(fileStream);  //xlsx數據讀入workbook
                var processOrderSheet = new List<string>();
                for (int sheetCount = 0; sheetCount < workbook.NumberOfSheets; sheetCount++)
                {
                    ISheet sheet = workbook.GetSheetAt(sheetCount);  //獲取第i個工作表
                    if (!sheet.SheetName.Contains("HY"))
                        continue;
                    processOrderSheet.Add(sheet.SheetName);
                }
                ComboBoxProcessOrderSheet.ItemsSource = processOrderSheet;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private IEnumerable<FactoryIdentity> FactoryList;
        private void ComboBoxProcessOrderSheet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedValue = ((ComboBox)sender).SelectedValue;
            if (selectedValue == null)
                return;
            var orderString = selectedValue.ToString();

            ProcessOrder processOrder = GetProcessOrder(orderString);

            TextBoxDyeClearFactory.Text = string.Empty;

            ISheet sheet = workbook.GetSheet(orderString);

            IRow rowFive = sheet.GetRow(5);

            string factoryString = ExcelHelper.GetCellString(rowFive, (int)ExcelEnum.ProcessOrderColumnIndexEnum.Factory).Replace(" ", "");

            TextBoxHandFeel.Text = processOrder.HandFeel;

            var factoryList = factoryString.Split('一').ToList();
            var factoryNameExist = CheckFactoryListExist(factoryList);
            if (!factoryNameExist) return;

            TextBoxFabric.Text = processOrder.Fabric;
            TextBoxSpecification.Text = processOrder.Specification;
            TextBoxMemo.Text = processOrder.Memo;
            TextBoxProcessItem.Text = processOrder.ProcessItem;
            TextBoxPrecautions.Text = processOrder.Precautions;

            var factoryDictionary = GetFactoryName(factoryList);
            factoryDictionary.TryGetValue("FabricFactory", out string fabricFactory);
            TextBoxFabricFactory.Text = fabricFactory;
            factoryDictionary.TryGetValue("DyeFactory", out string dyeFactory);
            TextBoxDyeFactory.Text = dyeFactory;
            factoryDictionary.TryGetValue("BrushedFactory", out string brushedFactory);
            TextBoxBrushedFactory.Text = brushedFactory;
            factoryDictionary.TryGetValue("ClearFactory", out string clearFactory);
            TextBoxClearFactory.Text = clearFactory;

            var processOrderColor = new List<ProcessOrderColor>();
            for (int rowIndex = 9; rowIndex <= 18; rowIndex++)
            {
                IRow row = sheet.GetRow(rowIndex);
                string color = ExcelHelper.GetCellString(row, (int)ExcelEnum.ProcessOrderColumnIndexEnum.Color);

                if (string.IsNullOrEmpty(color) || string.IsNullOrEmpty(color.Trim()))
                    break;
                var colorNumber = ExcelHelper.GetCellString(row, (int)ExcelEnum.ProcessOrderColumnIndexEnum.ColorNumber);

                var quantityCellString = ExcelHelper.GetCellString(row, (int)ExcelEnum.ProcessOrderColumnIndexEnum.ColorQuantity);
                Int32.TryParse(quantityCellString.Replace("疋", "").Replace("約", ""), out int quantity);

                processOrderColor.Add(new ProcessOrderColor
                {
                    Color = color,
                    ColorNumber = colorNumber,
                    Quantity = quantity,
                    Status = ProcessOrderColorStatus.未完成
                });
            }
            DataGridProcessOrderColor.ItemsSource = processOrderColor;
        }

        private ProcessOrder GetProcessOrder(string orderString)
        {
            ISheet sheet = workbook.GetSheet(orderString);
            IRow rowFive = sheet.GetRow(5);
            int widthCellNum = (int)ExcelEnum.ProcessOrderColumnIndexEnum.Width;
            string width = ExcelHelper.GetCellString(rowFive, widthCellNum);
            string clearType = ExcelHelper.GetCellString(rowFive, (int)ExcelEnum.ProcessOrderColumnIndexEnum.ClearType);
            string factoryString = ExcelHelper.GetCellString(rowFive, (int)ExcelEnum.ProcessOrderColumnIndexEnum.Factory).Replace(" ", "");
            string handFeel = ExcelHelper.GetCellString(rowFive, (int)ExcelEnum.ProcessOrderColumnIndexEnum.HandFeel);
            IRow rowSix = sheet.GetRow(6);
            string fabric = ExcelHelper.GetCellString(rowSix, (int)ExcelEnum.ProcessOrderColumnIndexEnum.Fabric);
            var weight = ExcelHelper.GetCellString(rowSix, (int)ExcelEnum.ProcessOrderColumnIndexEnum.Weight);
            IRow rowNine = sheet.GetRow(9);
            string memo = ExcelHelper.GetCellString(rowNine, (int)ExcelEnum.ProcessOrderColumnIndexEnum.Memo);
            IRow rowSeven = sheet.GetRow(7);
            string processItem = ExcelHelper.GetCellString(rowSeven, (int)ExcelEnum.ProcessOrderColumnIndexEnum.ProcessItem);
            string precautions = ExcelHelper.GetCellString(rowSeven, (int)ExcelEnum.ProcessOrderColumnIndexEnum.Precations);
            ProcessOrder processOrder = new ProcessOrder()
            {
                HandFeel = handFeel,
                Fabric = fabric,
                Specification = string.Concat(clearType, " ", width, "X", weight),
                Memo = Regex.Replace(memo, " {2,}", " "),
                ProcessItem = processItem,
                Precautions = precautions,
            };
            return processOrder;
        }

        private bool CheckFactoryListExist(List<string> factoryList)
        {
            FactoryList = FactoryModule.GetFactoryIdentiys(factoryList);
            List<string> remainFactoryList = new List<string>();
            remainFactoryList = factoryList.Where(w => !FactoryList.ToList().Select(s => s.Name).Contains(w)).ToList();
            if (remainFactoryList.Count() > 0)
            {
                MessageBox.Show(string.Concat("以下工廠尚未存在於清單:\n", string.Join(",", remainFactoryList), "\n點選確認將跳轉至新增工廠頁面!!"));
                var addFactory = new AddFactory
                {
                    DataContext = new AddFactoryViewModel { Name = remainFactoryList.First() }
                };
                this.NavigationService.Navigate(addFactory);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 預設加工廠名稱
        /// </summary>
        /// <param name="factoryList"></param>
        /// <returns></returns>
        private Dictionary<string, string> GetFactoryName(List<string> factoryList)
        {
            //預設第一個工廠為針織廠
            var factoryDictionary = new Dictionary<string, string>
            {
                { "FabricFactory", factoryList.First() }
            };

            switch (factoryList.Count())
            {
                case (1):
                    ComboBoxProcessPlan.SelectedIndex = 0;
                    break;

                //兩個加工廠則預設第二個工廠為定型廠
                case (2):
                    factoryDictionary.Add("ClearFactory", factoryList.Last());
                    ComboBoxProcessPlan.SelectedIndex = 3;
                    break;
                //三個加工廠則預設第二個工廠為染廠,第三個為定型廠
                case (3):
                    factoryDictionary.Add("DyeFactory", factoryList.Skip(1).Take(1).First());
                    factoryDictionary.Add("ClearFactory", factoryList.Last());
                    break;
                //四個加工廠則預設第二個工廠為染廠,第三個為刷毛廠,第四個為定型廠
                case (4):
                    factoryDictionary.Add("DyeFactory", factoryList[1]);
                    factoryDictionary.Add("BrushedFactory", factoryList[2]);
                    factoryDictionary.Add("ClearFactory", factoryList.Last());
                    break;
                default:
                    break;
            }

            return factoryDictionary;
        }

        private void ButtonNewProcessOrder_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxProcessOrderSheet.SelectedIndex == -1)
            {
                MessageBox.Show("請選擇一筆訂單");
                return;
            }
            //var oldProcessOrder = ProcessModule.GetProcessOrder();
            ProcessOrder processOrder = CheckNewProcessOrder();
            //{
            //    OrderString = ComboBoxProcessOrderSheet.SelectedValue.ToString(),
            //    Fabric = TextBoxFabric.Text,
            //    Specification = TextBoxSpecification.Text,
            //    ProcessItem = TextBoxProcessItem.Text,
            //    Precautions = TextBoxPrecautions.Text,
            //    Memo = TextBoxMemo.Text,
            //    HandFeel = TextBoxHandFeel.Text,
            //    CreateDate = DateTime.Now
            //};

            //if (oldProcessOrder.Select(s => s.OrderString.Trim()).Contains(processOrder.OrderString))
            //{
            //    MessageBox.Show("此筆訂單已存在於紀錄中!!");
            //    return;
            //}

            OnNewProcessOrder(processOrder);
        }

        private void OnNewProcessOrder(ProcessOrder processOrder)
        {
            if (processOrder == null) return;
            using (var scope = new TransactionScope())
            {
                var processOrderNo = ProcessModule.InsertProcessOrder(processOrder);

                List<ProcessOrderFlow> processOrderFlowList = GetProcessOrderFlowList(processOrderNo);

                var processOrderFlowSuccessCount = ProcessModule.InsertProcessOrderFlow(processOrderFlowList);

                var processOrderColorDetailList = new List<ProcessOrderColorDetail>();

                foreach (ProcessOrderColor item in DataGridProcessOrderColor.Items)
                {
                    processOrderColorDetailList.Add(new ProcessOrderColorDetail
                    {
                        OrderNo = processOrderNo,
                        Color = item.Color,
                        ColorNumber = item.ColorNumber,
                        Quantity = item.Quantity,
                        Status = item.Status
                    });
                }
                ProcessModule.CreateProcessOrderColorFlow(processOrderColorDetailList, processOrderNo);
                scope.Complete();
            }
        }

        private ProcessOrder CheckNewProcessOrder()
        {
            var oldProcessOrder = ProcessModule.GetProcessOrder();
            if (oldProcessOrder.Select(s => s.OrderString.Trim()).Contains(ComboBoxProcessOrderSheet.SelectedValue.ToString()))
            {
                MessageBox.Show(string.Format("此筆訂單{0}已存在於紀錄中!!", ComboBoxProcessOrderSheet.SelectedValue.ToString()));
                return null;
            }

            var processOrder = new ProcessOrder()
            {
                OrderString = ComboBoxProcessOrderSheet.SelectedValue.ToString(),
                Fabric = TextBoxFabric.Text,
                Specification = TextBoxSpecification.Text,
                ProcessItem = TextBoxProcessItem.Text,
                Precautions = TextBoxPrecautions.Text,
                Memo = TextBoxMemo.Text,
                HandFeel = TextBoxHandFeel.Text,
                CreateDate = DateTime.Now
            };

            return processOrder;
        }

        private List<ProcessOrderFlow> GetProcessOrderFlowList(int processOrderNo)
        {
            var processOrderFlowList = new List<ProcessOrderFlow>();
            if (!string.IsNullOrEmpty(TextBoxFabricFactory.Text))
            {
                processOrderFlowList.Add(new ProcessOrderFlow
                {
                    OrderNo = processOrderNo,
                    FactoryID = FactoryList.Where(w => w.Name == TextBoxFabricFactory.Text).First().FactoryID,
                    //InputDate = DatePickerFabricInput.SelectedDate,
                    //CompleteDate = DatePickerFabricComplete.SelectedDate
                });
            }
            if (!string.IsNullOrEmpty(TextBoxDyeFactory.Text))
            {
                processOrderFlowList.Add(new ProcessOrderFlow
                {
                    OrderNo = processOrderNo,
                    FactoryID = FactoryList.Where(w => w.Name == TextBoxDyeFactory.Text).First().FactoryID,
                    //InputDate = DatePickerDyeInput.SelectedDate,
                    //CompleteDate = DatePickerDyeComplete.SelectedDate
                });
            }
            if (!string.IsNullOrEmpty(TextBoxDyeClearFactory.Text))
            {
                processOrderFlowList.Add(new ProcessOrderFlow
                {
                    OrderNo = processOrderNo,
                    FactoryID = FactoryList.Where(w => w.Name == TextBoxDyeClearFactory.Text).First().FactoryID,
                    //InputDate = DatePickerDyeClearInput.SelectedDate,
                    //CompleteDate = DatePickerDyeClearComplete.SelectedDate
                });
            }
            if (!string.IsNullOrEmpty(TextBoxBrushedFactory.Text))
            {
                processOrderFlowList.Add(new ProcessOrderFlow
                {
                    OrderNo = processOrderNo,
                    FactoryID = FactoryList.Where(w => w.Name == TextBoxBrushedFactory.Text).First().FactoryID,
                    //InputDate = DatePickerBrushedInput.SelectedDate,
                    //CompleteDate = DatePickerBrushedComplete.SelectedDate
                });
            }

            if (!string.IsNullOrEmpty(TextBoxClearFactory.Text))
            {
                processOrderFlowList.Add(new ProcessOrderFlow
                {
                    OrderNo = processOrderNo,
                    FactoryID = FactoryList.Where(w => w.Name == TextBoxClearFactory.Text).First().FactoryID,
                    //InputDate = DatePickerClearInput.SelectedDate,
                    //CompleteDate = DatePickerClearComplete.SelectedDate
                });
            }

            return processOrderFlowList;
        }

        private void ComboBoxProcessPlan_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextBoxBrushedFactory.Text = string.Empty;
            TextBoxFabricFactory.Text = string.Empty;
            TextBoxDyeFactory.Text = string.Empty;
            TextBoxClearFactory.Text = string.Empty;
            TextBoxDyeClearFactory.Text = string.Empty;
            var comboBoxProcessPlan = sender as ComboBox;
            switch (ComboBoxProcessPlan.SelectedIndex)
            {
                case 0:
                    TextBoxFabricFactory.Text = FactoryList.First().Name;
                    break;
                case 1:
                    TextBoxDyeClearFactory.Text = FactoryList.First().Name;
                    break;
                case 2:
                    TextBoxClearFactory.Text = FactoryList.First().Name;
                    break;
                case 3:
                    //StackPanelClear.Visibility = Visibility.Visible;
                    //StackPanelDyeClear.Visibility = Visibility.Collapsed;
                    TextBoxFabricFactory.Text = FactoryList.First().Name;
                    TextBoxClearFactory.Text = FactoryList.Last().Name;
                    break;
                case 4:
                    //StackPanelClear.Visibility = Visibility.Collapsed;
                    //StackPanelDyeClear.Visibility = Visibility.Visible;
                    TextBoxFabricFactory.Text = FactoryList.First().Name;
                    TextBoxDyeClearFactory.Text = FactoryList.Last().Name;
                    break;
                case 5:
                    //StackPanelDye.Visibility = Visibility.Visible;
                    //StackPanelClear.Visibility = Visibility.Visible;
                    //StackPanelFabric.Visibility = Visibility.Collapsed;
                    //StackPanelDyeClear.Visibility = Visibility.Collapsed;
                    TextBoxDyeFactory.Text = FactoryList.First().Name;
                    TextBoxClearFactory.Text = FactoryList.Last().Name;
                    break;
                default:
                    break;
            }

        }
        private void ButtonDeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = DataGridProcessOrderColor.SelectedItem;
            var items = (List<ProcessOrderColor>)DataGridProcessOrderColor.ItemsSource;
            items.Remove((ProcessOrderColor)selectedItem);
            DataGridProcessOrderColor.ItemsSource = null;
            DataGridProcessOrderColor.ItemsSource = items;
        }

        private void BatchNewProcessOrder_Click(object sender, RoutedEventArgs e)
        {
            BatchInsertProcessOrder();
        }

        private void BatchInsertProcessOrder()
        {
            foreach (string item in ComboBoxProcessOrderSheet.Items)
            {
                ISheet sheet = workbook.GetSheet(item);
                IRow rowFive = sheet.GetRow(5);
                string factoryString = ExcelHelper.GetCellString(rowFive, (int)ExcelEnum.ProcessOrderColumnIndexEnum.Factory).Replace(" ", "");
                var factoryList = factoryString.Split('一').ToList();
                bool factoryListExist = CheckFactoryListExist(factoryList);
                if (!factoryListExist)
                {
                    return;
                }
            }
            for (int index = 0; index < ComboBoxProcessOrderSheet.Items.Count; index++)
            {
                ComboBoxProcessOrderSheet.SelectedIndex = index;
                ProcessOrder processOrder = CheckNewProcessOrder();
                OnNewProcessOrder(processOrder);
            }
        }
    }
}
