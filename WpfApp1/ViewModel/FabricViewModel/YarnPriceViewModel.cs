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

        public ObservableCollection<YarnPrice> MerchantYarnPrices { get; set; }
        public YarnSpecification SelectedYarnSpecification { get; set; }
        private void OnSelectionChangedExecute()
        {
            if (SelectedYarnSpecification == null) return;
            IEnumerable<MerchantYarnPrice> yarnPrices = FabricModule.GetYarnPriceByYarnSpecificationNo(SelectedYarnSpecification.YarnSpecificationNo);
            MerchantYarnPrices = new ObservableCollection<YarnPrice>(yarnPrices);
            RaisePropertyChanged("MerchantYarnPrices");
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
        public YarnPriceViewModel()
        {
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
