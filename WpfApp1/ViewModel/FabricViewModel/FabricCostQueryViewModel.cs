using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using WpfApp1.Command;
using WpfApp1.DataClass.Entity;
using WpfApp1.DataClass.Fabric;
using WpfApp1.Modules.FabricModule;
using WpfApp1.Modules.FabricModule.Implement;
using WpfApp1.Utility;
using WpfApp1.Windows.FabricWindows;

namespace WpfApp1.ViewModel.FabricViewModel
{
    public class FabricCostQueryViewModel : ViewModelBase
    {
        protected IFabricModule FabricModule { get; } = new FabricModule();

        public ICommand AddFabricColorClick { get { return new RelayCommand(AddFabricColorExecute, CanExecute); } }
        public ICommand AddProcessSequenceClick { get { return new RelayCommand(AddProcessSequenceExecute, CanExecute); } }

        private void AddProcessSequenceExecute()
        {
            if (Fabric == null)
            {
                MessageBox.Show("請選擇一個布種!!");
                return;
            }
            if (FabricColor == null)
            {
                MessageBox.Show("請選擇一個顏色!!");
                return;
            }
            AddProcessSequenceDialog addProcessSequenceDialog = new AddProcessSequenceDialog(Fabric, FabricColor, ProcessSequenceListGroup);
            addProcessSequenceDialog.Show();
        }

        private void AddFabricColorExecute()
        {
            if (Fabric == null)
            {
                MessageBox.Show("未選取布種!!");
                return;
            }
            EditProportionGroupDialog editProportionGroupDialog = new EditProportionGroupDialog(Fabric, FabricColor, FabricIngredientProportionGroup, FabricColorList)
            {
                Left = 500,
                Top = 550
            };
            editProportionGroupDialog.Show();
        }

        public ObservableCollection<Fabric> FabricList { get; set; }
        private Fabric _fabric { get; set; }
        public Fabric Fabric
        {
            get
            {
                return _fabric;
            }
            set
            {
                _fabric = value;
                FabricColorList.Clear();
                _stackPanel.Children.Clear();
                _stackPanelProcessSequence.Children.Clear();
                if (value == null) return;

                var fabricColorList = FabricModule.GetFabricColorListByFabricID(new List<int> { _fabric.FabricID });

                FabricColorList.AddRange<FabricColor>(fabricColorList);
                //foreach (var item in fabricColorList)
                //{
                //    FabricColorList.Add(item);
                //}
            }
        }
        private decimal _yarnCost { get; set; }
        public decimal YarnCost
        {
            get { return _yarnCost; }
            set
            {
                _yarnCost = value;
                RaisePropertyChanged("YarnCost");
            }
        }
        public ObservableCollection<FabricColor> FabricColorList { get; set; }
        private FabricColor _fabricColor { get; set; }
        public FabricColor FabricColor
        {
            get
            {
                return _fabricColor ?? new FabricColor();
            }
            set
            {
                _fabricColor = value;
                _stackPanelProcessSequence.Children.Clear();
                if (value == null) return;
                IEnumerable<FabricIngredientProportion> fabricIngredientProportions = FabricModule.GetFabricIngredientProportionByColorNo(new List<int> { value.ColorNo });

                FabricIngredientProportionGroup = fabricIngredientProportions.Count() == 0
                                                  ? FabricIngredientProportionGroup
                                                  : fabricIngredientProportions.GroupBy(g => g.Group).ToDictionary(g => g.Key, g => new ObservableCollection<FabricIngredientProportion>(g.ToList()));
                _stackPanel.Children.Clear();

                IEnumerable<ProcessSequenceDetail> processSequences = FabricModule.GetProcessSequences(Fabric.FabricID, FabricColor.ColorNo)
                                                                  .Select(s => new ProcessSequenceDetail
                                                                  {
                                                                      ColorNo = s.ColorNo,
                                                                      Name = s.Name,
                                                                      SequenceNo = s.SequenceNo,
                                                                      FabricID = s.FabricID,
                                                                      ProcessItem = s.ProcessItem,
                                                                      Loss = s.Loss,
                                                                      WorkPay = s.WorkPay,
                                                                      Order = s.Order,
                                                                      Group = s.Group,
                                                                      CreateDate = s.CreateDate,
                                                                      UpdateDate = s.UpdateDate,
                                                                      Cost = 0
                                                                  });
                ProcessSequenceListGroup = processSequences.GroupBy(g => g.Group).ToDictionary(g => g.Key, g => new ObservableCollection<ProcessSequenceDetail>(g.ToList()));

                foreach (var fabricIngredientPropertionItem in FabricIngredientProportionGroup)
                {
                    decimal fabricIngredientProportionYarnCost = CreateFabricIngredientProportion(fabricIngredientPropertionItem);
                    _stackPanelProcessSequence.Children.Add(new Label() { Content = string.Concat("紗價成本:", fabricIngredientProportionYarnCost) });

                    foreach (var processSequenceList in ProcessSequenceListGroup)
                    {
                        decimal processSequenceCost = fabricIngredientProportionYarnCost;
                        List<ProcessSequenceDetail> dataGridProcessSequenceList = new List<ProcessSequenceDetail>();
                        foreach (var item in processSequenceList.Value)
                        {
                            processSequenceCost = (processSequenceCost + item.WorkPay) * (1 + item.Loss / 100);
                            item.Cost = processSequenceCost;
                            dataGridProcessSequenceList.Add(new ProcessSequenceDetail
                            {
                                ColorNo = item.ColorNo,
                                SequenceNo = item.SequenceNo,
                                Cost = item.Cost,
                                Group = item.Group,
                                Loss = item.Loss,
                                Name = item.Name,
                                Order = item.Order,
                                WorkPay = item.WorkPay,
                                ProcessItem = item.ProcessItem
                            });
                        }
                        DataGrid dataGrid = new DataGrid
                        {
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Top,
                            AutoGenerateColumns = false,
                            CanUserAddRows = false,
                            Margin = new Thickness(0, 3, 0, 0),
                            Name = string.Concat("Group", processSequenceList.Key)
                        };
                        CreateDataGridTextColumn(dataGrid, "工廠名稱", "Name", null);

                        DataGridTextColumn processItem = new DataGridTextColumn
                        {
                            Header = "加工項目",
                            Binding = new Binding("ProcessItem")
                            {
                                Converter = new EnumConverter()
                            }
                        };
                        dataGrid.Columns.Add(processItem);

                        CreateDataGridTextColumn(dataGrid, "損耗", "Loss", "{0}%");
                        CreateDataGridTextColumn(dataGrid, "順序", "Order", null);
                        CreateDataGridTextColumn(dataGrid, "工繳", "WorkPay", "{0:C}");
                        CreateDataGridTextColumn(dataGrid, "群組", "Group", null);
                        CreateDataGridTextColumn(dataGrid, "成本", "Cost", null);
                        dataGrid.ItemsSource = dataGridProcessSequenceList;

                        _stackPanelProcessSequence.Children.Add(dataGrid);
                    }
                }
            }
        }

        private decimal CreateFabricIngredientProportion(KeyValuePair<int, ObservableCollection<FabricIngredientProportion>> fabricIngredientPropertionItem)
        {
            DataGrid dataGrid = new DataGrid
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                AutoGenerateColumns = false,
                CanUserAddRows = false,
                Name = string.Concat("Group", fabricIngredientPropertionItem.Key)
            };
            CreateDataGridTextColumn(dataGrid, "紗商", "Name", null);
            CreateDataGridTextColumn(dataGrid, "成分", "Ingredient", null);
            CreateDataGridTextColumn(dataGrid, "顏色", "Color", null);
            CreateDataGridTextColumn(dataGrid, "紗支數", "YarnCount", null);
            CreateDataGridTextColumn(dataGrid, "單價", "Price", "{0:C}");
            CreateDataGridTextColumn(dataGrid, "比例", "Proportion", "{0}%");
            CreateDataGridTextColumn(dataGrid, "群組", "Group", null);

            dataGrid.ItemsSource = fabricIngredientPropertionItem.Value;
            _stackPanel.Children.Add(dataGrid);

            decimal fabricIngredientPropertionItemYarnCost = 0;
            foreach (var fabricIngredientProportionItem in fabricIngredientPropertionItem.Value)
            {
                fabricIngredientPropertionItemYarnCost = fabricIngredientPropertionItemYarnCost + fabricIngredientProportionItem.Price * (fabricIngredientProportionItem.Proportion / 100);
            }

            Label label = new Label
            {
                Content = string.Concat("紗價成本:", fabricIngredientPropertionItemYarnCost)
            };
            _stackPanel.Children.Add(label);
            return fabricIngredientPropertionItemYarnCost;
        }

        private static void CreateDataGridTextColumn(DataGrid dataGrid, string Header, string BindingName, string stringFormat)
        {
            DataGridTextColumn yarnMerchant = new DataGridTextColumn
            {
                Header = Header,
                Binding = new Binding(BindingName)
                {
                    StringFormat = stringFormat
                }
            };
            dataGrid.Columns.Add(yarnMerchant);
        }

        public Dictionary<int, ObservableCollection<FabricIngredientProportion>> FabricIngredientProportionGroup { get; set; } = new Dictionary<int, ObservableCollection<FabricIngredientProportion>>();
        public Dictionary<int, ObservableCollection<ProcessSequenceDetail>> ProcessSequenceListGroup { get; set; }

        private StackPanel _stackPanel { get; set; }
        private StackPanel _stackPanelProcessSequence { get; set; }

        public FabricCostQueryViewModel(StackPanel stackPanel, StackPanel stackPanelProcessSequence)
        {
            FabricList = new ObservableCollection<Fabric>(FabricModule.GetFabricList());
            FabricColorList = new ObservableCollection<FabricColor>();
            FabricIngredientProportionGroup.Add(1, new ObservableCollection<FabricIngredientProportion>());

            _stackPanel = stackPanel;
            _stackPanelProcessSequence = stackPanelProcessSequence;
        }
    }
}
