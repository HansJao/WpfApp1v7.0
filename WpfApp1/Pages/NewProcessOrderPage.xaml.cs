using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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

namespace WpfApp1.Pages
{
    /// <summary>
    /// NewProcessOrderPage.xaml 的互動邏輯
    /// </summary>
    public partial class NewProcessOrderPage : Page
    {
        protected IProcessModule ProcessModule { get; } = new ProcessModule();
        protected IFactoryModule FactoryModule { get; } = new FactoryModule();
        public NewProcessOrderPage()
        {
            InitializeComponent();
            IWorkbook workbook = null;  //新建IWorkbook對象  
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
            //StackPanelBrushed.Visibility = Visibility.Visible;
            //StackPanelClear.Visibility = Visibility.Visible;
            //StackPanelDye.Visibility = Visibility.Visible;
            //StackPanelDyeClear.Visibility = Visibility.Visible;
            //ComboBoxProcessPlan.Visibility = Visibility.Visible;
            //ComboBoxItemDyeClear2.Visibility = Visibility.Visible;
            //ComboBoxItemFabricClear.Visibility = Visibility.Visible;
            //ComboBoxItemFabricDyeClear.Visibility = Visibility.Visible;
            //ComboBoxItemFabric.Visibility = Visibility.Visible;
            //ComboBoxItemClear.Visibility = Visibility.Visible;
            //ComboBoxItemDyeClear.Visibility = Visibility.Visible;

            TextBoxDyeClearFactory.Text = string.Empty;
            if (((ComboBox)sender).SelectedValue == null)
                return;
            var orderString = ((ComboBox)sender).SelectedValue.ToString();

            IWorkbook workbook = null;  //新建IWorkbook對象  
            string fileName = string.Concat(AppSettingConfig.ProcessOrderFilePath(), AppSettingConfig.ProcessOrderFileName());
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            workbook = new XSSFWorkbook(fileStream);  //xlsx數據讀入workbook

            var processOrderSheet = new List<string>();
            ISheet sheet = workbook.GetSheet(orderString);

            IRow rowFive = sheet.GetRow(5);
            var width = rowFive.GetCell((int)ExcelEnum.ProcessOrderColumnIndexEnum.Width).StringCellValue;
            var clearType = rowFive.GetCell((int)ExcelEnum.ProcessOrderColumnIndexEnum.ClearType).StringCellValue;
            var factoryString = rowFive.GetCell((int)ExcelEnum.ProcessOrderColumnIndexEnum.Factory).StringCellValue.Replace(" ", "");
            var handFeel = rowFive.GetCell((int)ExcelEnum.ProcessOrderColumnIndexEnum.HandFeel).StringCellValue;
            TextBoxHandFeel.Text = handFeel;

            var factoryList = factoryString.Split('一').ToList();
            FactoryList = FactoryModule.GetFactoryIdentiys(factoryList);
            var notInDbFactoryName = CheckFactoryList(factoryList, FactoryList.ToList());
            if (notInDbFactoryName.Count() > 0)
            {
                MessageBox.Show(string.Concat("以下工廠尚未存在於清單:\n", string.Join(",", notInDbFactoryName), "\n點選確認將跳轉至新增工廠頁面!!"));
                this.NavigationService.Navigate(new AddFactory(notInDbFactoryName.First()));
                return;
            }
            var factoryDictionary = GetFactoryName(factoryList);

            IRow rowSix = sheet.GetRow(6);
            var fabric = rowSix.GetCell((int)ExcelEnum.ProcessOrderColumnIndexEnum.Fabric);
            TextBoxFabric.Text = fabric.StringCellValue;
            var weight = rowSix.GetCell((int)ExcelEnum.ProcessOrderColumnIndexEnum.Weight).StringCellValue;

            TextBoxSpecification.Text = string.Concat(clearType, " ", width, "X", weight);

            IRow rowNine = sheet.GetRow(9);
            var memo = rowNine.GetCell((int)ExcelEnum.ProcessOrderColumnIndexEnum.Memo).StringCellValue;
            TextBoxMemo.Text = Regex.Replace(memo, " {2,}", " ");

            IRow rowSeven = sheet.GetRow(7);
            var processItem = rowSeven.GetCell((int)ExcelEnum.ProcessOrderColumnIndexEnum.ProcessItem).StringCellValue;
            TextBoxProcessItem.Text = processItem;
            var precautions = rowSeven.GetCell((int)ExcelEnum.ProcessOrderColumnIndexEnum.Precations).StringCellValue;
            TextBoxPrecautions.Text = precautions;

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
                var colorCell = row.GetCell((int)ExcelEnum.ProcessOrderColumnIndexEnum.Color);
                colorCell.SetCellType(CellType.String);
                var color = colorCell.StringCellValue;
                if (string.IsNullOrEmpty(color) || string.IsNullOrEmpty(color.Trim()))
                    break;
                var colorNumberCell = row.GetCell((int)ExcelEnum.ProcessOrderColumnIndexEnum.ColorNumber);
                colorNumberCell.SetCellType(CellType.String);
                var colorNumber = colorNumberCell.StringCellValue;

                var quantityCell = row.GetCell((int)ExcelEnum.ProcessOrderColumnIndexEnum.ColorQuantity);
                quantityCell.SetCellType(CellType.String);
                Int32.TryParse(quantityCell.StringCellValue.Replace("疋", "").Replace("約", ""), out int quantity);

                processOrderColor.Add(new ProcessOrderColor
                {
                    Color = color,
                    ColorNumber = colorNumber,
                    Quantity = quantity,
                    Status = ProcessOrderColorStatus.未出完
                }
                );
            }
            DataGridProcessOrderColor.ItemsSource = processOrderColor;
        }

        private List<string> CheckFactoryList(List<string> factoryList, List<FactoryIdentity> factoryIdentities)
        {
            List<string> remainFactoryList = new List<string>();
            if (factoryList.Count() != factoryIdentities.Count())
            {
                remainFactoryList = factoryList.Where(w => !factoryIdentities.Select(s => s.Name).Contains(w)).ToList();
            }
            return remainFactoryList;
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
                    //StackPanelDye.Visibility = Visibility.Collapsed;
                    //StackPanelBrushed.Visibility = Visibility.Collapsed;
                    //StackPanelDyeClear.Visibility = Visibility.Collapsed;
                    //StackPanelClear.Visibility = Visibility.Collapsed;
                    //ComboBoxItemDyeClear2.Visibility = Visibility.Collapsed;
                    //ComboBoxItemFabricClear.Visibility = Visibility.Collapsed;
                    //ComboBoxItemFabricDyeClear.Visibility = Visibility.Collapsed;
                    ComboBoxProcessPlan.SelectedIndex = 0;
                    break;

                //兩個加工廠則預設第二個工廠為定型廠
                case (2):
                    //StackPanelDye.Visibility = Visibility.Collapsed;
                    //StackPanelBrushed.Visibility = Visibility.Collapsed;
                    //StackPanelDyeClear.Visibility = Visibility.Collapsed;
                    //ComboBoxItemFabric.Visibility = Visibility.Collapsed;
                    //ComboBoxItemClear.Visibility = Visibility.Collapsed;
                    //ComboBoxItemDyeClear.Visibility = Visibility.Collapsed;
                    factoryDictionary.Add("ClearFactory", factoryList.Last());
                    ComboBoxProcessPlan.SelectedIndex = 3;
                    break;
                //三個加工廠則預設第二個工廠為染廠,第三個為定型廠
                case (3):
                    //StackPanelBrushed.Visibility = Visibility.Collapsed;
                    //StackPanelDyeClear.Visibility = Visibility.Collapsed;
                    //ComboBoxProcessPlan.Visibility = Visibility.Collapsed;
                    factoryDictionary.Add("DyeFactory", factoryList.Skip(1).Take(1).First());
                    factoryDictionary.Add("ClearFactory", factoryList.Last());
                    break;
                //四個加工廠則預設第二個工廠為染廠,第三個為刷毛廠,第四個為定型廠
                case (4):
                    //StackPanelDyeClear.Visibility = Visibility.Collapsed;
                    //ComboBoxProcessPlan.Visibility = Visibility.Collapsed;
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
            var oldProcessOrder = ProcessModule.GetProcessOrder();
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

            if (oldProcessOrder.Select(s => s.OrderString.Trim()).Contains(processOrder.OrderString))
            {
                MessageBox.Show("此筆訂單已存在於紀錄中!!");
                return;
            }


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
                }
                );
            }
            ProcessModule.CreateProcessOrderColorFlow(processOrderColorDetailList, processOrderNo);
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
    }
}
