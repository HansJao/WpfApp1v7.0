﻿using System;
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

namespace WpfApp1.ViewModel.FabricViewModel
{
    public class YarnPriceViewModel : ViewModelBase
    {
        public ICommand AddYarnPriceClick { get { return new RelayCommand(AddYarnPriceExecute, CanExecute); } }
        public ICommand EditYarnPriceClick { get { return new RelayCommand(EditYarnPriceExecute, CanExecute); } }

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
                _merchantYarnPrice = value;
                Ingredient = value.Ingredient;
                YarnCount = value.YarnCount;
                Price = value.Price;
                Color = value.Color;

                RaisePropertyChanged("MerchantYarnPrice");
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


            MerchantYarnPrice = new MerchantYarnPrice();
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
                bool success = FabricModule.InsertYarnPrice(MerchantYarnPrice);
                if (success)
                {
                    MessageBox.Show("新增成功");
                }
                else
                {
                    MessageBox.Show("新增失敗");
                }
            }
        }

        private void EditYarnPriceExecute()
        {
            bool success = FabricModule.InsertYarnPrice(MerchantYarnPrice);
            if (success)
            {
                MessageBox.Show("新增成功");
            }
            else
            {
                MessageBox.Show("新增失敗");
            }
        }
    }
}