using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using WpfApp1.Command;
using WpfApp1.DataClass.Entity;
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
        public ICommand AddYarnPriceClick { get { return new RelayCommand(AddYarnPriceExecute, CanExecute); } }
        public ICommand EditYarnPriceClick { get { return new RelayCommand(EditYarnPriceExecute, CanExecute); } }
        public ICommand DeleteYarnPriceClick { get { return new RelayCommand(DeleteYarnPriceExecute, CanExecute); } }

        private void DeleteYarnPriceExecute()
        {
            bool success = FabricModule.DeleteYarnPrice(MerchantYarnPrice.YarnPriceNo);
        }

        protected IFactoryModule FactoryModule { get; } = new FactoryModule();
        protected IFabricModule FabricModule { get; } = new FabricModule();

        public ObservableCollection<Factory> FactoryList { get; set; }
        private Factory _factory { get; set; }
        public Factory Factory
        {
            get { return _factory; }
            set
            {
                _factory = value;
                RaisePropertyChanged("Factory");
                ComboBoxFactoryListSelected();
            }
        }

        private void ComboBoxFactoryListSelected()
        {
            int filterText = Factory.FactoryID;
            MerchantYarnPriceCVS.View.Refresh();

        }
        internal CollectionViewSource MerchantYarnPriceCVS { get; set; }
        public ICollectionView MerchantYarnPriceView
        {
            get { return MerchantYarnPriceCVS.View; }
        }
        public ObservableCollection<MerchantYarnPrice> MerchantYarnPrices { get; set; }
        private MerchantYarnPrice _merchantYarnPrice { get; set; }
        public MerchantYarnPrice MerchantYarnPrice
        {
            get { return _merchantYarnPrice; }
            set
            {
                if (value == null) return;
                _merchantYarnPrice = value;
                YarnMerchant = value.YarnMerchant;
                Ingredient = value.Ingredient;
                YarnCount = value.YarnCount;
                Price = value.Price;
                Color = value.Color;
                var factory = FactoryList.Where(w => w.FactoryID == value.YarnMerchant).First();
                if (Factory == null || factory.FactoryID != Factory.FactoryID)
                    Factory = factory;
                RaisePropertyChanged("MerchantYarnPrice");
            }
        }
        private int _yarnMerchant { get; set; }
        public int YarnMerchant
        {
            get { return _yarnMerchant; }
            set
            {
                _yarnMerchant = value;
                RaisePropertyChanged("YarnMerchant");
            }
        }
        private string _ingredient { get; set; }
        public string Ingredient
        {
            get { return _ingredient; }
            set
            {
                _ingredient = value;
                RaisePropertyChanged("Ingredient");
            }
        }
        private int _yarnCount { get; set; }
        public int YarnCount
        {
            get { return _yarnCount; }
            set
            {
                _yarnCount = value;
                RaisePropertyChanged("YarnCount");
            }
        }
        private string _color { get; set; }
        public string Color
        {
            get { return _color; }
            set
            {
                _color = value;
                RaisePropertyChanged("Color");
            }
        }
        private int _price { get; set; }
        public int Price
        {
            get { return _price; }
            set
            {
                _price = value;
                RaisePropertyChanged("Price");
            }
        }
        public YarnPriceViewModel()
        {
            var factoryList = FactoryModule.GetFactoryList().Where(w => w.Process == DataClass.Enumeration.ProcessItem.Yarn);
            FactoryList = new ObservableCollection<Factory>(factoryList);
            MerchantYarnPriceCVS = new CollectionViewSource();


            var merchantYarnPrices = FabricModule.GetMerchantYarnPriceList();
            MerchantYarnPrices = new ObservableCollection<MerchantYarnPrice>(merchantYarnPrices);
            MerchantYarnPriceCVS.Source = MerchantYarnPrices;
            MerchantYarnPriceCVS.Filter += ApplyFilter;
        }

        void ApplyFilter(object sender, FilterEventArgs e)
        {
            MerchantYarnPrice merchantYarnPrice = (MerchantYarnPrice)e.Item;

            if (Factory == null)
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = merchantYarnPrice.Name.Contains(Factory.Name);
            }
        }

        private void AddYarnPriceExecute()
        {
            if (Ingredient == string.Empty || Price == 0 || YarnCount == 0 || Color == string.Empty)
            {
                MessageBox.Show("有數值未填");
            }
            else if (Factory == null)
            {
                MessageBox.Show("未選擇紗商");
            }
            else
            {
                YarnPrice yarnPrice = new YarnPrice
                {
                    YarnMerchant = Factory.FactoryID,
                    Ingredient = Ingredient,
                    Color = Color,
                    Price = Price,
                    YarnCount = YarnCount,

                };
                bool success = FabricModule.InsertYarnPrice(yarnPrice);

                success.CheckSuccessMessageBox("新增成功", "新增失敗");
               
            }
        }

        private void EditYarnPriceExecute()
        {
            if (Ingredient == string.Empty || Price == 0 || YarnCount == 0 || Color == string.Empty)
            {
                MessageBox.Show("有數值未填");
            }
            else if (YarnMerchant == 0)
            {
                MessageBox.Show("未選擇紗商");
            }
            else if (MerchantYarnPrice == null)
            {
                MessageBox.Show("未選擇一筆資料");
            }
            else
            {
                YarnPrice yarnPrice = new YarnPrice
                {
                    YarnPriceNo = MerchantYarnPrice.YarnPriceNo,
                    YarnMerchant = YarnMerchant,
                    Ingredient = Ingredient,
                    Color = Color,
                    Price = Price,
                    YarnCount = YarnCount,

                };
                bool success = FabricModule.EditYarnPrice(yarnPrice);
                success.CheckSuccessMessageBox("修改成功", "修改失敗");               
            }
        }
    }
}
