using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using WpfApp1.Command;
using WpfApp1.DataClass.AccountSystem;
using WpfApp1.DataClass.TrashSystem;
using WpfApp1.Modules.AccountSystemModule;
using WpfApp1.Modules.AccountSystemModule.Implement;
using WpfApp1.Modules.TrashModule;
using WpfApp1.Modules.TrashModule.Implement;

namespace WpfApp1.ViewModel.TrashSystemViewModel
{
    public class CheckBillViewModel : ViewModelBase
    {
        protected ITrashModule TrashModule { get; } = new TrashModule();
        protected IAccountSystemModule AccountSystemModule { get; } = new AccountSystemModule();

        //public ICommand CheckBillDateSelect { get { return new RelayCommand(CheckBillDateSelectExecute, CanExecute); } }
        public ICommand ComboBoxCustomerKeyUp { get { return new RelayCommand(ComboBoxCustomerKeyUpExecute, CanExecute); } }
        public ICommand ComboBoxCustomerSelectionChanged { get { return new RelayCommand(ComboBoxCustomerSelectionChangedExecute, CanExecute); } }


        public TrashCustomer SelectedTrashCustomer { get; set; }
        private void ComboBoxCustomerSelectionChangedExecute()
        {
            List<TrashCustomerShipped> invoSubList = new List<TrashCustomerShipped>(TrashModule.GetInvoSub(CheckBillDate)
          .Where(w => w.C_01 == SelectedTrashCustomer.CARD_NO));

            IEnumerable<AccountTextile> accountTextiles = AccountSystemModule.GetAccountTextile();
            IEnumerable<CustomerTextilePrice> customerTextilePrices = AccountSystemModule.GetCustomerTextilePrice(SelectedTrashCustomer.CARD_NO);
            IEnumerable<CustomerCheckBillSheet> customerCheckBillSheets = AccountSystemModule.GetCheckBillSheet(accountTextiles, customerTextilePrices, invoSubList);
            InvoSubList = new ObservableCollection<TrashCustomerShipped>(customerCheckBillSheets);
            OnPropertyChanged("InvoSubList");
        }

        public DateTime CheckBillDate { get; set; } = DateTime.Now;

        //private void CheckBillDateSelectExecute()
        //{
        //    InvoSubList = new ObservableCollection<TrashCustomerShipped>(TrashModule.GetInvoSub()
        //     .Where(w => w.C_01 == SelectedTrashCustomer.CARD_NO && w.IN_DATE.ToString("yyyy/MM") == DateTime.Now.ToString("yyyy/MM"))
        //     .OrderBy(o => o.IN_NO));
        //    OnPropertyChanged("InvoSubList");
        //}

        public ObservableCollection<TrashCustomerShipped> InvoSubList { get; set; }
        public IEnumerable<TrashCustomer> TrashCustomerList { get; set; }
        public string TrashCustomerText { get; set; }

        public CheckBillViewModel()
        {
            TrashCustomerList = TrashModule.GetCustomerList();
        }


        private void ComboBoxCustomerKeyUpExecute()
        {
            CollectionView itemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView(TrashCustomerList);

            itemsViewOriginal.Filter = ((o) =>
            {
                TrashCustomer trashCustomer = o as TrashCustomer;
                if (string.IsNullOrEmpty(TrashCustomerText))
                    return false;
                else
                {
                    if (trashCustomer.C_NAME.Contains(TrashCustomerText)) return true;
                    else return false;
                }
            });

            //cmb.IsDropDownOpen = true;
            itemsViewOriginal.Refresh();
        }
    }
}
