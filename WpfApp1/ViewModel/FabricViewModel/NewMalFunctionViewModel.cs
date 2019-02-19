using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using WpfApp1.Command;
using WpfApp1.DataClass.Entity;
using WpfApp1.Modules.CustomerModule;
using WpfApp1.Modules.CustomerModule.Implement;
using WpfApp1.Modules.FabricModule;
using WpfApp1.Modules.FabricModule.Implement;
using WpfApp1.Modules.FactoryModule;
using WpfApp1.Modules.FactoryModule.Implement;
using WpfApp1.Modules.Process;
using WpfApp1.Modules.Process.Implement;

namespace WpfApp1.ViewModel.FabricViewModel
{
    public class NewMalFunctionViewModel : ViewModelBase
    {
        protected IFabricModule FabricModule { get; } = new FabricModule();
        protected ICustomerModule CustomerModule { get; } = new CustomerModule();
        protected IFactoryModule FactoryModule { get; } = new FactoryModule();
        protected IProcessModule ProcessModule { get; } = new ProcessModule();

        public IEnumerable<Customer> CustomerList { get; set; }
        private Customer _customer { get; set; }
        public Customer Customer
        {
            get { return _customer; }
            set { _customer = value; }
        }
        public IEnumerable<ProcessOrder> ProcessOrderList { get; set; }
        public ProcessOrder ProcessOrder { get; set; }
        public NewMalFunctionViewModel()
        {
            CustomerList = CustomerModule.GetCustomerList();
            FabricList = FabricModule.GetFabricList();
            FactoryList = FactoryModule.GetFactoryList();
            ProcessOrderList = ProcessModule.GetProcessOrder();
        }

        private MalFunction _malFunction { get; set; } = new MalFunction();
        public MalFunction MalFunction
        {
            get
            { return _malFunction; }
            set
            { _malFunction = value; }
        }
        public ICommand AddFabricColorClick { get { return new RelayCommand(AddMalFunctionExecute, CanExecute); } }
        private void AddMalFunctionExecute()
        {

            MalFunction.CustomerID = Customer.CustomerID;
            MalFunction.FactoryID = Factory.FactoryID;
            MalFunction.ColorNo = FabricColor.ColorNo;
        }

        public Visibility ProcessOrderVisbility { get; set; } = Visibility.Collapsed;
        public ICommand TextBoxUnFocusCommand { get { return new RelayCommand(TextBoxUnFocusExecute, CanExecute); } }
        private void TextBoxUnFocusExecute()
        {
            ProcessOrderVisbility = Visibility.Collapsed;
            RaisePropertyChanged("ProcessOrderVisbility");
        }
      
        private string _repairOrderString { get; set; }
        public string RepairOrderString
        {
            get { return _repairOrderString; }
            set
            {
                _repairOrderString = value;
                if (string.IsNullOrEmpty(value))
                {
                    ProcessOrderVisbility = Visibility.Collapsed;
                }
                else
                {
                    ProcessOrderVisbility = Visibility.Visible;
                    RaisePropertyChanged("ProcessOrderVisbility");
                    string filterText = value;
                    ICollectionView cv = CollectionViewSource.GetDefaultView(ProcessOrderList);
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
            }
        }
        
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
        public IEnumerable<FabricColor> FabricColorList { get; set; }
        public FabricColor FabricColor { get; set; }
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
    }
}
