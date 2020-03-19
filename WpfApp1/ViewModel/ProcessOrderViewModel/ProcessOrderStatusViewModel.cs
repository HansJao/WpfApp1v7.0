using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
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
        public ICommand FabricChanged { get { return new RelayCommand(FabricChangedExecute, CanExecute); } }

        public ObservableCollection<ProcessOrderStatus> ReadyDye { get; set; }
        public ObservableCollection<ProcessOrderStatus> Dying { get; set; }
        public ObservableCollection<ProcessOrderStatus> ReadyClear { get; set; }
        public ObservableCollection<ProcessOrderStatus> Clearing { get; set; }
        public ObservableCollection<ProcessOrderStatus> Finish3Day { get; set; }
        public ObservableCollection<ProcessOrderStatus> FinishAfter4Day { get; set; }

        private void RefreshExecute()
        {
            IEnumerable<ProcessOrderStatus> processOrderStatuses = ProcessModule.GetProcessOrderStatus();

            ReadyDye = new ObservableCollection<ProcessOrderStatus>(processOrderStatuses.Where(w => w.Status == DataClass.Enumeration.ProcessOrderColorStatus.已排染).OrderByDescending(o => o.UpdateDate));
            RaisePropertyChanged("ReadyDye");

            Dying = new ObservableCollection<ProcessOrderStatus>(processOrderStatuses.Where(w => w.Status == DataClass.Enumeration.ProcessOrderColorStatus.在染缸).OrderByDescending(o => o.UpdateDate));
            RaisePropertyChanged("Dying");

            ReadyClear = new ObservableCollection<ProcessOrderStatus>(processOrderStatuses.Where(w => w.Status == DataClass.Enumeration.ProcessOrderColorStatus.待定型).OrderByDescending(o => o.UpdateDate));
            RaisePropertyChanged("ReadyClear");

            Clearing = new ObservableCollection<ProcessOrderStatus>(processOrderStatuses.Where(w => w.Status == DataClass.Enumeration.ProcessOrderColorStatus.定型中).OrderByDescending(o => o.UpdateDate));
            RaisePropertyChanged("Clearing");

            Finish3Day = new ObservableCollection<ProcessOrderStatus>(processOrderStatuses.Where(w => w.Status == DataClass.Enumeration.ProcessOrderColorStatus.已完成).Where(w => (DateTime.Now - w.UpdateDate).TotalDays <= 3).OrderByDescending(o => o.UpdateDate));
            RaisePropertyChanged("Finish3Day");

            FinishAfter4Day = new ObservableCollection<ProcessOrderStatus>(processOrderStatuses.Where(w => w.Status == DataClass.Enumeration.ProcessOrderColorStatus.已完成).Where(w => (DateTime.Now - w.UpdateDate).TotalDays >= 4).OrderByDescending(o => o.UpdateDate));
            RaisePropertyChanged("FinishAfter4Day");
        }

        public ProcessOrderStatusViewModel()
        {
            RefreshExecute();
        }

        public string Fabric { get; set; }
        private void FabricChangedExecute()
        {
            DataGridFilter(ReadyDye);
            DataGridFilter(Dying);
            DataGridFilter(ReadyClear);
            DataGridFilter(Clearing);
            DataGridFilter(Finish3Day);
            DataGridFilter(FinishAfter4Day);
        }

        private void DataGridFilter(ObservableCollection<ProcessOrderStatus> processOrderStatuses)
        {
            ICollectionView cv = CollectionViewSource.GetDefaultView(processOrderStatuses);
            if (!string.IsNullOrEmpty(Fabric))
            {
                cv.Filter = o =>
                {
                    ProcessOrderStatus p = o as ProcessOrderStatus;
                    return (p.Fabric.ToUpper().Contains(Fabric.ToUpper()));
                };
            }
            else
            {
                cv.Filter = o =>
                {
                    return (true);
                };
            };
        }
    }
}
