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

        private Fabric _fabric { get; set; }
        public Fabric Fabric
        {
            get { return _fabric; }
            set
            {
                _fabric = value;
                FabricColorList = FabricModule.GetFabricColorListByFabricID(new List<int> { value.FabricID });
                RaisePropertyChanged("FabricColorList");
            }
        }
        public IEnumerable<Fabric> FabricList { get; set; }

        public IEnumerable<Factory> FactoryList { get; set; }
        public Factory Factory { get; set; }

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
