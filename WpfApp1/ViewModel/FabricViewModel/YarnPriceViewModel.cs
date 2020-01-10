using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using WpfApp1.Command;
using WpfApp1.DataClass.Entity;
using WpfApp1.DataClass.Entity.FabricEntity;
using WpfApp1.DataClass.Fabric;
using WpfApp1.Modules.FabricModule;
using WpfApp1.Modules.FabricModule.Implement;
using WpfApp1.Modules.FactoryModule;
using WpfApp1.Modules.FactoryModule.Implement;
using WpfApp1.Utility;

namespace WpfApp1.ViewModel.FabricViewModel
{
    public class YarnPriceViewModel : ViewModelBase
    {
        protected IFactoryModule FactoryModule { get; } = new FactoryModule();
        protected IFabricModule FabricModule { get; } = new FabricModule();

        public ICommand AddYarnPriceClick { get { return new RelayCommand(AddYarnPriceExecute, CanExecute); } }
        public ICommand AddYarnSpecificationClick { get { return new RelayCommand(AddYarnSpecificationExecute, CanExecute); } }
        public ICommand OnSelectionChangedCommand { get { return new RelayCommand(OnSelectionChangedExecute, CanExecute); } }
        public ICommand ComboBoxSelectionChanged { get { return new RelayCommand(ComboBoxSelectionChangedExecute, CanExecute); } }
        public Factory SearchFactory { get; set; }
        private void ComboBoxSelectionChangedExecute()
        {
            IEnumerable<YarnSpecification> yarnSpecifications = FabricModule.GetYarnSpecificationListByYarnMerchant(SearchFactory.FactoryID);
            YarnSpecificationList = new ObservableCollection<YarnSpecification>(yarnSpecifications);
            RaisePropertyChanged("YarnSpecificationList");
        }

        public ObservableCollection<YarnPrice> MerchantYarnPrices { get; set; }
        public YarnSpecification SelectedYarnSpecification { get; set; }
        private void OnSelectionChangedExecute()
        {
            _stackPanel.Children.Clear();

            if (SelectedYarnSpecification == null) return;
            IEnumerable<MerchantYarnPrice> yarnPrices = FabricModule.GetYarnPriceByYarnSpecificationNo(SelectedYarnSpecification.YarnSpecificationNo);
            MerchantYarnPrices = new ObservableCollection<YarnPrice>(yarnPrices);
            MerchantYarnPrice merchantYarnPriceMax = yarnPrices.OrderByDescending(o => o.PiecePrice).First();
            MerchantYarnPrice merchantYarnPriceMin = yarnPrices.OrderBy(o => o.PiecePrice).First();
            MerchantYarnPrice merchantYarnPriceTime = yarnPrices.OrderBy(o => o.CreateDate).First();
            var merchantYarnPriceGroup = yarnPrices.GroupBy(g => g.YarnMerchant).ToDictionary(g => g.Key, g => g.ToList());
            CreateDataGrid(0, merchantYarnPriceMax, merchantYarnPriceMin, merchantYarnPriceTime, yarnPrices);
            foreach (var item in merchantYarnPriceGroup)
            {
                IEnumerable<MerchantYarnPrice> merchants = item.Value;
                CreateDataGrid(item.Key, merchants.OrderByDescending(o => o.PiecePrice).First(), merchants.OrderBy(o => o.PiecePrice).First(), merchants.OrderBy(o => o.CreateDate).First(), merchants);
            }
            RaisePropertyChanged("MerchantYarnPrices");
        }

        private void CreateDataGrid(int yarnMerchentKey, MerchantYarnPrice max, MerchantYarnPrice min, MerchantYarnPrice late, IEnumerable<MerchantYarnPrice> merchantYarnPrices)
        {
            DataGrid dataGrid = new DataGrid
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                AutoGenerateColumns = false,
                CanUserAddRows = false,
                Name = string.Concat("Group", yarnMerchentKey),
                Margin = new Thickness(5, 5, 5, 5)
            };
            ControllerHelper.CreateDataGridTextColumn(dataGrid, "紗商", "Name", null);
            ControllerHelper.CreateDataGridTextColumn(dataGrid, "單價", "Price", null);
            ControllerHelper.CreateDataGridTextColumn(dataGrid, "紗價", "PiecePrice", null);
            ControllerHelper.CreateDataGridTextColumn(dataGrid, "建立時間", "CreateDate", "{0:yy.MM.dd}");
            dataGrid.ItemsSource = merchantYarnPrices;
            StackPanel stackPanel = new StackPanel();

            #region 最大值
            StackPanel stackPanelMax = new StackPanel
            {
                Background = new SolidColorBrush(Colors.LightYellow)
            };
            Label labelMax = new Label
            {
                Content = string.Concat("    最高價"),
                Background = new SolidColorBrush(Colors.LightGray)
            };
            stackPanelMax.Children.Add(labelMax);
            Label labelMaxName = new Label
            {
                Content = string.Concat("紗商:", max.Name)
            };
            stackPanelMax.Children.Add(labelMaxName);
            Label labelMaxPrice = new Label
            {
                Content = string.Concat("價格:", max.Price)
            };
            stackPanelMax.Children.Add(labelMaxPrice);
            Label labelMaxDate = new Label
            {
                Content = string.Concat("建立時間:", max.CreateDate.ToShortDateString(), "\r\n", "            ", max.CreateDate.ToLongTimeString())
            };
            stackPanelMax.Children.Add(labelMaxDate);
            stackPanel.Children.Add(stackPanelMax);
            #endregion

            #region 最小值
            StackPanel stackPanelMin = new StackPanel
            {
                Background = new SolidColorBrush(Colors.LightBlue)
            };
            Label labelMin = new Label
            {
                Content = string.Concat("    最低價"),
                Background = new SolidColorBrush(Colors.LightGray)
            };
            stackPanelMin.Children.Add(labelMin);
            Label labelMinName = new Label
            {
                Content = string.Concat("紗商:", min.Name)
            };
            stackPanelMin.Children.Add(labelMinName);
            Label labelMinPrice = new Label
            {
                Content = string.Concat("價格:", min.Price)
            };
            stackPanelMin.Children.Add(labelMinPrice);
            Label labelMinDate = new Label
            {
                Content = string.Concat("建立時間:", min.CreateDate.ToShortDateString(), "\r\n", "            ", min.CreateDate.ToLongTimeString())
            };
            stackPanelMin.Children.Add(labelMinDate);
            stackPanel.Children.Add(stackPanelMin);
            #endregion


            #region 最新
            StackPanel stackPanelLate = new StackPanel
            {
                Background = new SolidColorBrush(Colors.LightPink)
            };
            Label labelLate = new Label
            {
                Content = string.Concat("    最新"),
                Background = new SolidColorBrush(Colors.LightGray)
            };
            stackPanelLate.Children.Add(labelLate);
            Label labellateName = new Label
            {
                Content = string.Concat("紗商:", late.Name)
            };
            stackPanelLate.Children.Add(labellateName);
            Label labellatePrice = new Label
            {
                Content = string.Concat("價格:", late.Price)
            };
            stackPanelLate.Children.Add(labellatePrice);
            Label labellateDate = new Label
            {
                Content = string.Concat("建立時間:", late.CreateDate.ToShortDateString(), "\r\n", "            ", late.CreateDate.ToLongTimeString())
            };
            stackPanelLate.Children.Add(labellateDate);
            stackPanel.Children.Add(stackPanelLate);
            #endregion

            _stackPanel.Children.Add(dataGrid);
            _stackPanel.Children.Add(stackPanel);
        }

        public YarnSpecification YarnSpecification { get; set; } = new YarnSpecification();
        private void AddYarnSpecificationExecute()
        {
            bool success = FabricModule.AddYarnSpecification(YarnSpecification);
            if (success)
            {
                MessageBox.Show("新增成功！！");
                IEnumerable<YarnSpecification> yarnSpecificationList = FabricModule.GetYarnSpecificationList();
                YarnSpecificationList = new ObservableCollection<YarnSpecification>(yarnSpecificationList);
                RaisePropertyChanged("YarnSpecificationList");
            }
            else
            {
                MessageBox.Show("新增失敗！！");
            }
        }

        public ObservableCollection<Factory> FactoryList { get; set; }
        public ObservableCollection<YarnSpecification> YarnSpecificationList { get; set; }
        private StackPanel _stackPanel { get; set; }
        public YarnPriceViewModel(StackPanel stackPanel)
        {
            _stackPanel = stackPanel;
            var factoryList = FactoryModule.GetFactoryList().Where(w => w.Process == DataClass.Enumeration.ProcessItem.Yarn);
            FactoryList = new ObservableCollection<Factory>(factoryList);

            IEnumerable<YarnSpecification> yarnSpecificationList = FabricModule.GetYarnSpecificationList();
            YarnSpecificationList = new ObservableCollection<YarnSpecification>(yarnSpecificationList);
        }

        public YarnPrice YarnPrice { get; set; } = new YarnPrice();
        private void AddYarnPriceExecute()
        {
            if (YarnPrice.Price == 0)
            {
                MessageBox.Show("單價未填！！");
            }
            else if (YarnPrice.YarnSpecificationNo == 0)
            {
                MessageBox.Show("未選擇紗規格！！");
            }
            else
            {
                bool success = FabricModule.InsertYarnPrice(YarnPrice);
                success.CheckSuccessMessageBox("新增成功", "新增失敗");
            }
        }
    }
}
