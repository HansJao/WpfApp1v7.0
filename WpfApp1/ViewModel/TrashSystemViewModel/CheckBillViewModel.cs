using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfApp1.Command;
using WpfApp1.DataClass.TrashSystem;
using WpfApp1.Modules.TrashModule;
using WpfApp1.Modules.TrashModule.Implement;

namespace WpfApp1.ViewModel.TrashSystemViewModel
{
    public class CheckBillViewModel : ViewModelBase
    {
        protected ITrashModule TrashModule { get; } = new TrashModule();


        public ICommand CheckBillDateSelect { get { return new RelayCommand(CheckBillDateSelectExecute, CanExecute); } }

        public DateTime CheckBillDate { get; set; } = DateTime.Now;

        private void CheckBillDateSelectExecute()
        {
             InvoSubList = new ObservableCollection<TrashCustomerShipped>(TrashModule.GetInvoSub()
              .Where(w => w.C_Name == "億隆" && w.IN_DATE.ToString("yyyy/MM") == DateTime.Now.ToString("yyyy/MM"))
              .OrderBy(o => o.IN_NO));
            OnPropertyChanged("InvoSubList");
        }

        public ObservableCollection<TrashCustomerShipped> InvoSubList { get; set; }
        public CheckBillViewModel()
        {

        }
    }
}
