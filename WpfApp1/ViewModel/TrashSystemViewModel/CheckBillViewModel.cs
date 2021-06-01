using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        public ICommand InsertCustomerPriceClick { get { return new RelayCommand(InsertCustomerPriceClickExecute, CanExecute); } }
        public ICommand UpdateCustomerPriceClick { get { return new RelayCommand(UpdateCustomerPriceClickExecute, CanExecute); } }
        public ICommand DatePickerSelectedDateChanged { get { return new RelayCommand(DatePickerSelectedDateChangedExecute, CanExecute); } }
        public ICommand EnterKeyCommand { get { return new RelayCommand(EnterKeyCommandExecute, CanExecute); } }
        public ICommand SelectedCustomerCheckBillSheetsCommand
        {
            get
            {
                return new RelayCommand<object>(
                    o =>
                    {
                        SelectedCustomerCheckBillSheets = (o as ObservableCollection<object>).Cast<CustomerCheckBillSheet>().ToList();
                    },
                    CanExecute);
            }
        }

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
            if (SelectedCustomerCheckBillSheets == null)
            {
                MessageBox.Show("未選擇欲更新的布種！");
                return;
            }
            if (SelectedCustomerCheckBillSheets.Where(w => w.CustomerPrice == 0).Count() > 0)
            {
                MessageBox.Show("此客戶的布種尚未設定單價！！");
                return;
            }

            int successUpdateCustomerTextilePriceCount = 0;

            List<CustomerCheckBillSheet> UpdateCustomerTextilePriceError = new List<CustomerCheckBillSheet>();
            List<CustomerCheckBillSheet> UpdateTrashSystemPriceError = new List<CustomerCheckBillSheet>();

            foreach (CustomerCheckBillSheet item in SelectedCustomerCheckBillSheets)
            {
                bool success = AccountSystemModule.UpdateCustomerTextilePrice(item, UpdateCustomerPrice);
                if (success)
                {
                    successUpdateCustomerTextilePriceCount++;
                    item.CustomerPrice = UpdateCustomerPrice;
                }
                else
                    UpdateCustomerTextilePriceError.Add(item);
            }

            if (successUpdateCustomerTextilePriceCount == SelectedCustomerCheckBillSheets.Count())
            {
                int successUpdateTrashSystemPriceCount = 0;
                foreach (var item in SelectedCustomerCheckBillSheets)
                {
                    bool updateTrashSystemsuccess = UpdateTrashSystemPrice(item, UpdateCustomerPrice);
                    if (updateTrashSystemsuccess)
                        successUpdateTrashSystemPriceCount++;
                    else
                        UpdateTrashSystemPriceError.Add(item);
                }

                if (successUpdateTrashSystemPriceCount == SelectedCustomerCheckBillSheets.Count())
                {
                    DisplayUpdate(successUpdateCustomerTextilePriceCount == SelectedCustomerCheckBillSheets.Count(), UpdateCustomerPrice);
                    MessageBox.Show("更新客戶單價成功！");
                }
                else
                    MessageBox.Show("帳務系統更新失敗！！", "錯誤！", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            else
                MessageBox.Show("更新失敗！！\n" + string.Join("\n", UpdateCustomerTextilePriceError.Select(s => s.I_03)), "錯誤！", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public int NewCustomerPrice { get; set; }
        public int UpdateCustomerPrice { get; set; }
        public CustomerCheckBillSheet SelectedCustomerCheckBillSheet { get; set; }
        public List<CustomerCheckBillSheet> SelectedCustomerCheckBillSheets { get; set; }
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
                    bool updateTrashSystemsuccess = UpdateTrashSystemPrice(SelectedCustomerCheckBillSheet, NewCustomerPrice);
                    if (updateTrashSystemsuccess)
                    {
                        DisplayUpdate(newDefaultPriceSuccess, NewCustomerPrice);
                        MessageBox.Show("新增客戶單價成功！");
                    }
                    else
                    {

                        MessageBox.Show("帳務系統新增失敗！！", "錯誤！", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                    MessageBox.Show("新增失敗！！", "錯誤！", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show("已設定過該客戶的布種單價，請使用更新單價功能！！");
            }
        }

        private bool UpdateTrashSystemPrice(CustomerCheckBillSheet customerCheckBillSheet, int newPrice)
        {
            int updateCount = TrashModule.UpdateInvoSubPrice(customerCheckBillSheet, newPrice, CheckBillDate);
            bool updateTrashSystemsuccess = CustomerCheckBillSheets.Where(w => w.I_01 == customerCheckBillSheet.I_01 && w.F_01 == customerCheckBillSheet.F_01).Count() == updateCount;
            return updateTrashSystemsuccess;
        }

        private void DisplayUpdate(bool newDefaultPriceSuccess, int customPrice)
        {
            foreach (var selectedCustomerCheckBillSheet in SelectedCustomerCheckBillSheets)
            {
                foreach (var customerCheckBillSheet in CustomerCheckBillSheets.Where(w => w.F_01 == selectedCustomerCheckBillSheet.F_01 && w.I_01 == selectedCustomerCheckBillSheet.I_01))
                {
                    if (newDefaultPriceSuccess)
                        customerCheckBillSheet.DefaultPrice = customPrice;
                    customerCheckBillSheet.CustomerPrice = customPrice;
                    customerCheckBillSheet.Price = customPrice;
                }
            }           
        }

        public int InvoSubSelected { get; set; }
        private TrashCustomer _SelectedTrashCustomer { get; set; }
        public TrashCustomer SelectedTrashCustomer
        {
            get
            {
                return _SelectedTrashCustomer;
            }
            set
            {
                _SelectedTrashCustomer = value;
                ComboBoxCustomerSelectionChangedExecute();
            }
        }
        private TrashCustomer priviousTrashCustomer = null;
        private DateTime priviousCheckBillDate;
        private void ComboBoxCustomerSelectionChangedExecute()
        {
            if (priviousTrashCustomer == SelectedTrashCustomer && priviousCheckBillDate.ToString("yyyy/MM") == CheckBillDate.ToString("yyyy/MM"))
                return;
            if (SelectedTrashCustomer == null)
            {
                return;
            };
            priviousTrashCustomer = SelectedTrashCustomer;
            priviousCheckBillDate = CheckBillDate;
            List<TrashCustomerShipped> invoSubList = new List<TrashCustomerShipped>(TrashModule.GetInvoSub(CheckBillDate)
          .Where(w => w.C_01 == SelectedTrashCustomer.CARD_NO));

            IEnumerable<AccountTextile> accountTextiles = AccountSystemModule.GetAccountTextile();
            IEnumerable<CustomerTextilePrice> customerTextilePrices = AccountSystemModule.GetCustomerTextilePrice(SelectedTrashCustomer.CARD_NO);
            IEnumerable<CustomerCheckBillSheet> customerCheckBillSheets = AccountSystemModule.GetCheckBillSheet(accountTextiles, customerTextilePrices, invoSubList).OrderBy(o => o.IN_DATE).ThenBy(t => t.Time);
            CustomerCheckBillSheets = new ObservableCollection<CustomerCheckBillSheet>(customerCheckBillSheets);
            OnPropertyChanged("CustomerCheckBillSheets");
        }

        public DateTime CheckBillDate { get; set; } = DateTime.Now;

        public ObservableCollection<CustomerCheckBillSheet> CustomerCheckBillSheets { get; set; }
        public IEnumerable<TrashCustomer> TrashCustomerList { get; set; }
        private string _TextBoxTrashCustomerID { get; set; }
        public string TextBoxTrashCustomerID
        {
            get { return _TextBoxTrashCustomerID; }
            set
            {
                _TextBoxTrashCustomerID = value;
                ICollectionView cv = CollectionViewSource.GetDefaultView(TrashCustomerList);
                if (!string.IsNullOrEmpty(value))
                {
                    cv.Filter = o =>
                    {
                        /* change to get data row value */
                        TrashCustomer p = o as TrashCustomer;
                        return (p.CARD_NO.Contains(value));
                        /* end change to get data row value */
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
        private string _TextBoxTrashCustomer { get; set; }
        public string TextBoxTrashCustomer
        {
            get { return _TextBoxTrashCustomer; }
            set
            {
                _TextBoxTrashCustomer = value;
                ICollectionView cv = CollectionViewSource.GetDefaultView(TrashCustomerList);
                if (!string.IsNullOrEmpty(value))
                {
                    cv.Filter = o =>
                    {
                        /* change to get data row value */
                        TrashCustomer p = o as TrashCustomer;
                        return (p.C_NAME.Contains(value));
                        /* end change to get data row value */
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

        readonly CollectionView ItemsViewOriginal;
        public CheckBillViewModel()
        {
            TrashCustomerList = TrashModule.GetCustomerList();
            ItemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView(TrashCustomerList);
        }
    }
}
