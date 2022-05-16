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
    public class CustomerOrderHistoryViewModel : ViewModelBase
    {
        protected ITrashModule TrashModule { get; } = new TrashModule();

        public IEnumerable<TrashCustomerShipped> TrashCustomerShippeds { get; set; }
        public TrashCustomer TrashCustomer { get; set; } = new TrashCustomer();
        public CustomerOrderHistoryViewModel(TrashCustomer trashCustomer, DateTime begin, DateTime end)
        {
            TrashCustomer = trashCustomer;
            TrashCustomerShippeds = TrashModule.GetCustomerShippedList(trashCustomer.C_NAME, begin, end).OrderByDescending(o => o.IN_DATE);
            var x  = TrashCustomerShippeds.Select(s => s.IN_DATE);
        }
    }
}
