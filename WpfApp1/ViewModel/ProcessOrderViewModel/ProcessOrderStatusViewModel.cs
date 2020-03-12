using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfApp1.Command;
using WpfApp1.DataClass.Entity;
using WpfApp1.Modules.Process;
using WpfApp1.Modules.Process.Implement;

namespace WpfApp1.ViewModel.ProcessOrderViewModel
{
    public class ProcessOrderStatusViewModel : ViewModelBase
    {
        protected IProcessModule ProcessModule { get; } = new ProcessModule();

        public ICommand RefreshClick { get { return new RelayCommand(RefreshExecute, CanExecute); } }

        public ObservableCollection<ProcessOrderStatus> ReadyDye { get; set; }
        public ObservableCollection<ProcessOrderStatus> Dying { get; set; }
        public ObservableCollection<ProcessOrderStatus> ReadyClear { get; set; }
        public ObservableCollection<ProcessOrderStatus> Clearing { get; set; }

        private void RefreshExecute()
        {
            IEnumerable<ProcessOrderStatus> processOrderStatuses = ProcessModule.GetProcessOrderStatus();

            ReadyDye = new ObservableCollection<ProcessOrderStatus>(processOrderStatuses.Where(w => w.Status == 6).OrderByDescending(o => o.UpdateDate));
            RaisePropertyChanged("ReadyDye");

            Dying = new ObservableCollection<ProcessOrderStatus>(processOrderStatuses.Where(w => w.Status == 7).OrderByDescending(o => o.UpdateDate));
            RaisePropertyChanged("Dying");

            ReadyClear = new ObservableCollection<ProcessOrderStatus>(processOrderStatuses.Where(w => w.Status == 8).OrderByDescending(o => o.UpdateDate));
            RaisePropertyChanged("ReadyClear");

            Clearing = new ObservableCollection<ProcessOrderStatus>(processOrderStatuses.Where(w => w.Status == 9).OrderByDescending(o => o.UpdateDate));
            RaisePropertyChanged("Clearing");
        }

        public ProcessOrderStatusViewModel()
        {
            RefreshExecute();
        }
    }
}
