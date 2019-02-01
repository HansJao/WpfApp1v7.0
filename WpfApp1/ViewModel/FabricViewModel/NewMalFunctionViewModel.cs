using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WpfApp1.DataClass.Entity;
using WpfApp1.Modules.CustomerModule;
using WpfApp1.Modules.CustomerModule.Implement;
using WpfApp1.Modules.FabricModule;
using WpfApp1.Modules.FabricModule.Implement;
using WpfApp1.Modules.FactoryModule;
using WpfApp1.Modules.FactoryModule.Implement;

namespace WpfApp1.ViewModel.FabricViewModel
{
    public class NewMalFunctionViewModel : ViewModelBase
    {
        protected IFabricModule FabricModule { get; } = new FabricModule();
        protected ICustomerModule CustomerModule { get; } = new CustomerModule();
        protected IFactoryModule FactoryModule { get; } = new FactoryModule();


        private Customer _customer { get; set; }
        public Customer Customer
        {
            get { return _customer; }
            set { _customer = value; }
        }

        public IEnumerable<Customer> CustomerList { get; set; }
        private string _searchCustomer { get; set; }
        public string SearchCustomer
        {
            get { return _searchCustomer; }
            set
            {
                _searchCustomer = value;
                CollectionView itemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView(CustomerList);

                itemsViewOriginal.Filter = ((o) =>
                {
                    if (String.IsNullOrEmpty(_searchCustomer)) return true;
                    else
                    {
                        if ((((Customer)o).Name).Contains(_searchCustomer)) return true;
                        else return false;
                    }
                });
            }
        }

        private Fabric _fabric { get; set; }
        public Fabric Fabric
        {
            get { return _fabric; }
            set
            {
                if (value == null)
                {
                    return;
                }
                _fabric = value;
                FabricColorList = FabricModule.GetFabricColorListByFabricID(new List<int> { value.FabricID });
                RaisePropertyChanged("FabricColorList");
            }
        }
        public IEnumerable<Fabric> FabricList { get; set; }
        private string _searchFabric { get; set; }
        public string SearchFabric
        {
            get { return _searchFabric; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    FabricColorList = null;
                    RaisePropertyChanged("FabricColorList");
                    return;
                }
                _searchFabric = value;
                CollectionView itemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView(FabricList);

                itemsViewOriginal.Filter = ((o) =>
                {
                    if (String.IsNullOrEmpty(_searchFabric)) return true;
                    else
                    {
                        if ((((Fabric)o).FabricName).Contains(_searchFabric)) return true;
                        else return false;
                    }
                });
            }
        }

        public IEnumerable<Factory> FactoryList { get; set; }
        public Factory Factory { get; set; }
        private string _searchFactory { get; set; }
        public string SearchFactory
        {
            get { return _searchFactory; }
            set
            {
                _searchFactory = value;
                CollectionView itemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView(FactoryList);

                itemsViewOriginal.Filter = ((o) =>
                {
                    if (String.IsNullOrEmpty(_searchFactory)) return true;
                    else
                    {
                        if ((((Factory)o).Name).Contains(_searchFactory)) return true;
                        else return false;
                    }
                });
            }
        }

        public IEnumerable<FabricColor> FabricColorList { get; set; }
        public FabricColor FabricColor { get; set; }
        public NewMalFunctionViewModel()
        {
            CustomerList = CustomerModule.GetCustomerList();
            FabricList = FabricModule.GetFabricList();
            FactoryList = FactoryModule.GetFactoryList();
        }
    }
}
