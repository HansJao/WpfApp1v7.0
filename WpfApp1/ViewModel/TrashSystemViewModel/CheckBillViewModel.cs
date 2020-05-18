using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        public ICommand InsertCustomerPriceClick { get { return new RelayCommand(InsertCustomerPriceClickExecute, CanExecute); } }
        public ICommand UpdateCustomerPriceClick { get { return new RelayCommand(UpdateCustomerPriceClickExecute, CanExecute); } }
        public ICommand DatePickerSelectedDateChanged { get { return new RelayCommand(DatePickerSelectedDateChangedExecute, CanExecute); } }
        public ICommand EnterKeyCommand { get { return new RelayCommand(EnterKeyCommandExecute, CanExecute); } }
        public ICommand SelectedCustomerEnter { get { return new RelayCommand(SelectedCustomerEnterExecute, CanExecute); } }



        private void EnterKeyCommandExecute()
        {
            InsertCustomerPriceClickExecute();
        }

        private void DatePickerSelectedDateChangedExecute()
        {
            if (SelectedTrashCustomer != null)
            {
                ComboBoxCustomerSelectionChangedExecute();
            }
        }

        private void UpdateCustomerPriceClickExecute()
        {
            if (UpdateCustomerPrice <= 0)
            {
                MessageBox.Show("欲更新的單價設定錯誤！！");
                return;
            }
            if (SelectedCustomerCheckBillSheet == null)
            {
                MessageBox.Show("未選擇欲更新的布種！");
            }
            if (SelectedCustomerCheckBillSheet.CustomerPrice == 0)
            {
                MessageBox.Show("此客戶的布種尚未設定單價！！");
                return;
            }

            SelectedCustomerCheckBillSheet.CustomerPrice = UpdateCustomerPrice;
            bool success = AccountSystemModule.UpdateCustomerTextilePrice(SelectedCustomerCheckBillSheet);
            if (success) MessageBox.Show("更新成功！");
        }

        public int NewCustomerPrice { get; set; }
        public int UpdateCustomerPrice { get; set; }
        public CustomerCheckBillSheet SelectedCustomerCheckBillSheet { get; set; }
        private void InsertCustomerPriceClickExecute()
        {
            if (NewCustomerPrice <= 0)
            {
                MessageBox.Show("設定的客戶單價不正確！！");
                return;
            }
            bool newDefaultPriceSuccess = false;
            if (SelectedCustomerCheckBillSheet.DefaultPrice == 0)
            {
                List<AccountTextile> accountTextile = new List<AccountTextile>(){
                new AccountTextile
                {
                    FactoryID = SelectedCustomerCheckBillSheet.F_01,
                    ItemID = SelectedCustomerCheckBillSheet.I_01,
                    ItemName = SelectedCustomerCheckBillSheet.I_03,
                    DefaultPrice = Convert.ToInt32(SelectedCustomerCheckBillSheet.Price)
                }};
                newDefaultPriceSuccess = AccountSystemModule.InsertDefaultPrice(accountTextile);
            }
            IEnumerable<AccountTextile> accountTextiles = AccountSystemModule.GetAccountTextile();
            CustomerTextilePrice customerTextilePrice = new CustomerTextilePrice
            {
                AccountTextileID = accountTextiles.Where(w => w.FactoryID == SelectedCustomerCheckBillSheet.F_01 && w.ItemID == SelectedCustomerCheckBillSheet.I_01).FirstOrDefault().AccountTextileID,
                AccountCustomerID = SelectedTrashCustomer.CARD_NO,
                Price = NewCustomerPrice
            };
            if (!AccountSystemModule.GetCustomerTextilePrice(SelectedCustomerCheckBillSheet.C_01).Any(a => a.AccountTextileID == SelectedCustomerCheckBillSheet.AccountTextileID))
            {
                bool success = AccountSystemModule.InsertCustomerTextilePrice(customerTextilePrice);
                if (success)
                {
                    foreach (var item in CustomerCheckBillSheets.Where(w => w.F_01 == SelectedCustomerCheckBillSheet.F_01 && w.I_01 == SelectedCustomerCheckBillSheet.I_01))
                    {
                        if (newDefaultPriceSuccess)
                            item.DefaultPrice = Convert.ToInt32(SelectedCustomerCheckBillSheet.Price);
                        item.CustomerPrice = Convert.ToInt32(NewCustomerPrice);
                    }
                    MessageBox.Show("新增客戶單價成功！");
                }
                else
                    MessageBox.Show("新增失敗！！", "錯誤！", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show("已設定過該客戶的布種單價，請使用更新單價功能！！");
            }
        }

        public int InvoSubSelected { get; set; }
        public TrashCustomer SelectedTrashCustomer { get; set; }

        private void SelectedCustomerEnterExecute()
        {
            ComboBoxCustomerSelectionChangedExecute();
        }
        private TrashCustomer priviousTrashCustomer = null;
        private void ComboBoxCustomerSelectionChangedExecute()
        {
            if (priviousTrashCustomer == SelectedTrashCustomer)
                return;
            if (SelectedTrashCustomer == null)
            {
                if (TrashCustomerText != string.Empty)
                {
                    SelectedTrashCustomer = TrashCustomerList.Where(w => w.C_NAME == TrashCustomerText).FirstOrDefault();
                    if (SelectedTrashCustomer == null)
                        return;
                }
                else
                    return;
            };
            priviousTrashCustomer = SelectedTrashCustomer;
            List<TrashCustomerShipped> invoSubList = new List<TrashCustomerShipped>(TrashModule.GetInvoSub(CheckBillDate)
          .Where(w => w.C_01 == SelectedTrashCustomer.CARD_NO));

            IEnumerable<AccountTextile> accountTextiles = AccountSystemModule.GetAccountTextile();
            IEnumerable<CustomerTextilePrice> customerTextilePrices = AccountSystemModule.GetCustomerTextilePrice(SelectedTrashCustomer.CARD_NO);
            IEnumerable<CustomerCheckBillSheet> customerCheckBillSheets = AccountSystemModule.GetCheckBillSheet(accountTextiles, customerTextilePrices, invoSubList);
            CustomerCheckBillSheets = new ObservableCollection<CustomerCheckBillSheet>(customerCheckBillSheets);
            OnPropertyChanged("CustomerCheckBillSheets");
        }

        public DateTime CheckBillDate { get; set; } = DateTime.Now;

        //private void CheckBillDateSelectExecute()
        //{
        //    InvoSubList = new ObservableCollection<TrashCustomerShipped>(TrashModule.GetInvoSub()
        //     .Where(w => w.C_01 == SelectedTrashCustomer.CARD_NO && w.IN_DATE.ToString("yyyy/MM") == DateTime.Now.ToString("yyyy/MM"))
        //     .OrderBy(o => o.IN_NO));
        //    OnPropertyChanged("InvoSubList");
        //}

        public ObservableCollection<CustomerCheckBillSheet> CustomerCheckBillSheets { get; set; }
        public IEnumerable<TrashCustomer> TrashCustomerList { get; set; }
        public string TrashCustomerText { get; set; }
        CollectionView ItemsViewOriginal;
        public CheckBillViewModel()
        {
            TrashCustomerList = TrashModule.GetCustomerList();
            ItemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView(TrashCustomerList);
        }


        private void ComboBoxCustomerKeyUpExecute()
        {
            if (string.IsNullOrEmpty(TrashCustomerText)) return;
            ItemsViewOriginal.Filter = ((o) =>
            {
                TrashCustomer trashCustomer = o as TrashCustomer;
                if (string.IsNullOrEmpty(TrashCustomerText))
                    return true;
                else
                {
                    if (trashCustomer.C_NAME.ToUpper().Contains(TrashCustomerText)) return true;
                    else return false;
                }
            });

            //ItemsViewOriginal.Refresh();
        }
    }
}
