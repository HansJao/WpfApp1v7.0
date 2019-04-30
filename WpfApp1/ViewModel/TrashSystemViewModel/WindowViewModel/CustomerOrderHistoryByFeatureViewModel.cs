using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.TrashSystem;
using WpfApp1.Modules.TrashModule;
using WpfApp1.Modules.TrashModule.Implement;

namespace WpfApp1.ViewModel.TrashSystemViewModel.WindowViewModel
{
    public class CustomerOrderHistoryByFeatureViewModel : ViewModelBase
    {
        protected ITrashModule TrashModule { get; } = new TrashModule();

        public IEnumerable<TrashCustomerShipped> TrashCustomerShippeds { get; set; }
        public TrashItem TrashItem { get; set; } = new TrashItem();
        public CustomerOrderHistoryByFeatureViewModel(TrashItem trashItem)
        {
            TrashItem = trashItem;
            RaisePropertyChanged("TrashItem");
            TrashCustomerShippeds = TrashModule.GetCustomerShippedListByFeature(trashItem);
        }
    }
}
