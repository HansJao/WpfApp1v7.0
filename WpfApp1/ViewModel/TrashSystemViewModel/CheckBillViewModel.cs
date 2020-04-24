﻿using System;
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
        public ICommand InsertCustomerPriceClick { get { return new RelayCommand(InsertCustomerPriceClickExecute, CanExecute); } }

        public int CustomerPrice { get; set; }
        public CustomerCheckBillSheet SelectedCustomerCheckBillSheet { get; set; }
        private void InsertCustomerPriceClickExecute()
        {
            if (SelectedCustomerCheckBillSheet.DefaultPrice == 0)
            {
                List<AccountTextile> accountTextile = new List<AccountTextile>()
                { new AccountTextile
                {
                    FactoryID = SelectedCustomerCheckBillSheet.F_01,
                    ItemID = SelectedCustomerCheckBillSheet.I_01,
                    ItemName = SelectedCustomerCheckBillSheet.I_03,
                    DefaultPrice = Convert.ToInt32(SelectedCustomerCheckBillSheet.Price)
                }
            };
                AccountSystemModule.InsertDefaultPrice(accountTextile);
            }
            IEnumerable<AccountTextile> accountTextiles = AccountSystemModule.GetAccountTextile();
            CustomerTextilePrice customerTextilePrice = new CustomerTextilePrice
            {
                AccountTextileID = accountTextiles.Where(w => w.FactoryID == SelectedCustomerCheckBillSheet.F_01 && w.ItemID == SelectedCustomerCheckBillSheet.I_01).FirstOrDefault().AccountTextileID,
                AccountCustomerID = SelectedTrashCustomer.CARD_NO,
                Price = CustomerPrice
            };
            bool success = AccountSystemModule.InsertCustomerTextilePrice(customerTextilePrice);
            CustomerCheckBillSheets.ElementAt(CustomerCheckBillSheets.IndexOf(SelectedCustomerCheckBillSheet)).DefaultPrice = Convert.ToInt32(SelectedCustomerCheckBillSheet.Price);
            CustomerCheckBillSheets.ElementAt(CustomerCheckBillSheets.IndexOf(SelectedCustomerCheckBillSheet)).CustomerPrice = Convert.ToInt32(CustomerPrice);
        }

        public int InvoSubSelected { get; set; }
        public TrashCustomer SelectedTrashCustomer { get; set; }
        private void ComboBoxCustomerSelectionChangedExecute()
        {
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
