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
            AddProcessSequenceDialog addProcessSequenceDialog = new AddProcessSequenceDialog();
            addProcessSequenceDialog.Show();
        }

        private void AddFabricColorExecute()
        {
            if (Fabric == null)
            {
                MessageBox.Show("未選取布種!!");
                return;
            }
            AddFabricColorDialog addFabricColorDialog = new AddFabricColorDialog(Fabric, FabricColor, FabricIngredientProportionGroup, FabricColorList);
            addFabricColorDialog.Left = 500;
            addFabricColorDialog.Top = 550;
            addFabricColorDialog.Show();
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
                ProcessSequenceList.Clear();
                if (value == null) return;
                List<int> fabricIDList = new List<int> { _fabric.FabricID };

                var fabricColorList = FabricModule.GetFabricColorListByFabricID(fabricIDList);
                foreach (var item in fabricColorList)
                {
                    FabricColorList.Add(item);
                }

                IEnumerable<ProcessSequenceCost> processSequences = FabricModule.GetProcessSequences(new List<int> { _fabric.FabricID })
                                                                    .Select(s => new ProcessSequenceCost
                                                                    {
                                                                        SequenceNo = s.SequenceNo,
                                                                        FabricID = s.FabricID,
                                                                        ColorNoString = s.ColorNoString,
                                                                        ProcessItem = s.ProcessItem,
                                                                        Loss = s.Loss,
                                                                        WorkPay = s.WorkPay,
                                                                        Order = s.Order,
                                                                        Group = s.Group,
                                                                        CreateDate = s.CreateDate,
                                                                        UpdateDate = s.UpdateDate,
                                                                        Cost = 0
                                                                    });
                foreach (var item in processSequences)
                {
                    ProcessSequenceList.Add(item);
                }
            }
        }
        private float _yarnCost { get; set; }
        public float YarnCost
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
                return _fabricColor;
            }
            set
            {
                _fabricColor = value;
                FabricIngredientProportionList.Clear();
                if (value == null) return;
                IEnumerable<FabricIngredientProportion> fabricIngredientProportions = FabricModule.GetFabricIngredientProportionByColorNo(new List<int> { value.ColorNo });
                float yarnCost = 0;
                foreach (var item in fabricIngredientProportions)
                {
                    FabricIngredientProportionList.Add(item);
                    yarnCost = yarnCost + item.Price * (item.Proportion / 100);
                }
                YarnCost = yarnCost;
                foreach (var item in ProcessSequenceList)
                {
                    yarnCost = (yarnCost + item.WorkPay) * (1 + item.Loss / 100);
                    item.Cost = yarnCost;
                }
                FabricIngredientProportionGroup = fabricIngredientProportions.Count() == 0 ? FabricIngredientProportionGroup : fabricIngredientProportions.GroupBy(g => g.Group).ToDictionary(g => g.Key, g => new ObservableCollection<FabricIngredientProportion>(g.ToList()));
                _stackPanel.Children.Clear();
                foreach (var fabricIngredientPropertionItem in FabricIngredientProportionGroup)
                {
                    DataGrid dataGrid = new DataGrid
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        AutoGenerateColumns = false,
                        CanUserAddRows = false,
                        Name = string.Concat("Group", fabricIngredientPropertionItem.Key)
                    };

                    DataGridTextColumn yarnMerchant = new DataGridTextColumn
                    {
                        Header = "紗商",
                        Binding = new Binding("Name")
                    };
                    dataGrid.Columns.Add(yarnMerchant);

                    DataGridTextColumn ingredient = new DataGridTextColumn
                    {
                        Header = "成分",
                        Binding = new Binding("Ingredient")
                    };
                    dataGrid.Columns.Add(ingredient);

                    DataGridTextColumn color = new DataGridTextColumn
                    {
                        Header = "顏色",
                        Binding = new Binding("Color")
                    };
                    dataGrid.Columns.Add(color);

                    DataGridTextColumn yarnCount = new DataGridTextColumn
                    {
                        Header = "紗支數",
                        Binding = new Binding("YarnCount")
                    };
                    dataGrid.Columns.Add(yarnCount);

                    DataGridTextColumn price = new DataGridTextColumn
                    {
                        Header = "單價",
                        Binding = new Binding("Price")
                        {
                            StringFormat = "{0:C}"
                        }
                    };
                    dataGrid.Columns.Add(price);

                    DataGridTextColumn proportion = new DataGridTextColumn
                    {
                        Header = "比例",
                        Binding = new Binding("Proportion")
                        {
                            StringFormat = "{0}%"
                        }
                    };
                    dataGrid.Columns.Add(proportion);

                    DataGridTextColumn group = new DataGridTextColumn
                    {
                        Header = "群組",
                        Binding = new Binding("Group")
                    };
                    dataGrid.Columns.Add(group);

                    dataGrid.ItemsSource = fabricIngredientPropertionItem.Value;
                    _stackPanel.Children.Add(dataGrid);
                    float fabricIngredientPropertionItemYarnCost = 0;
                    foreach (var fabricIngredientProportionItem in fabricIngredientPropertionItem.Value)
                    {
                        fabricIngredientPropertionItemYarnCost = fabricIngredientPropertionItemYarnCost + fabricIngredientProportionItem.Price * (fabricIngredientProportionItem.Proportion / 100);
                    }

                    Label label = new Label();
                    label.Content = string.Concat("紗價成本:", fabricIngredientPropertionItemYarnCost);
                    _stackPanel.Children.Add(label);
                }

            }
        }
        public Dictionary<int, ObservableCollection<FabricIngredientProportion>> FabricIngredientProportionGroup { get; set; } = new Dictionary<int, ObservableCollection<FabricIngredientProportion>>();
        public ObservableCollection<FabricIngredientProportion> FabricIngredientProportionList { get; set; }
        private ObservableCollection<ProcessSequenceCost> _processSequenceList { get; set; }
        public ObservableCollection<ProcessSequenceCost> ProcessSequenceList
        {
            get { return _processSequenceList; }
            set { _processSequenceList = value; }
        }
        private StackPanel _stackPanel { get; set; }
        public FabricCostQueryViewModel(StackPanel stackPanel)
        {
            FabricList = new ObservableCollection<Fabric>(FabricModule.GetFabricList());
            FabricColorList = new ObservableCollection<FabricColor>();
            FabricIngredientProportionList = new ObservableCollection<FabricIngredientProportion>();
            ProcessSequenceList = new ObservableCollection<ProcessSequenceCost>();
            FabricIngredientProportionGroup.Add(1, new ObservableCollection<FabricIngredientProportion>());

            _stackPanel = stackPanel;
        }
    }
}
