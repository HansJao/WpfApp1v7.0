using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WpfApp1.Modules.CustomerModule;
using WpfApp1.Modules.CustomerModule.Implement;
using WpfApp1.Modules.FabricModule;
using WpfApp1.Modules.FabricModule.Implement;

namespace WpfApp1.ViewModel.FabricViewModel
{
    public class MalFunctionViewModel : ViewModelBase
    {
        protected IFabricModule FabricModule { get; } = new FabricModule();
        protected ICustomerModule CustomerModule { get; } = new CustomerModule();


        private string _customerName { get; set; }
        public string CustomerName
        {
            get { return _customerName; }
            set
            {
                _customerName = value;
                string filterText = value;
                ICollectionView cv = CollectionViewSource.GetDefaultView(CustomerNameList);
                if (!string.IsNullOrEmpty(filterText))
                {
                    cv.Filter = o =>
                    {
                        /* change to get data row value */
                        string p = o as string;
                        return (p.ToUpper().Contains(filterText.ToUpper()));
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

        public IEnumerable<string> CustomerNameList { get; set; }
        public MalFunctionViewModel()
        {
            CustomerNameList = CustomerModule.GetCustomerNameList();
        }
    }
}
