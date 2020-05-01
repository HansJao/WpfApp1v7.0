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
using WpfApp1.Utility.EqualityComparer;

namespace WpfApp1.ViewModel.TrashSystemViewModel
{
    public class DefaultPriceViewModel : ViewModelBase
    {
        protected IAccountSystemModule AccountSystemModule { get; } = new AccountSystemModule();
        protected ITrashModule TrashModule { get; } = new TrashModule();

        public ICommand ButtonTrashItemInsertClick { get { return new RelayCommand(ButtonTrashItemInsertClickExecute, CanExecute); } }
        public ICommand ButtonUpdateDefaultPriceClick { get { return new RelayCommand(ButtonUpdateDefaultPriceClickExecute, CanExecute); } }

        private void ButtonUpdateDefaultPriceClickExecute()
        {
            if (AccountTextileList.Count() == 0)
            {
                MessageBox.Show("未選取要新增的布種！", "錯誤！", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            bool success = AccountSystemModule.InsertDefaultPrice(AccountTextileList);
            if (success)
                MessageBox.Show("新增成功！");
            else
                MessageBox.Show("新增失敗！", "錯誤！", MessageBoxButton.OK, MessageBoxImage.Error);
            AccountTextileList.Clear();

        }
        public ObservableCollection<AccountTextile> AccountTextileList { get; set; } = new ObservableCollection<AccountTextile>();

        private string _itemName { get; set; }
        public string ItemName
        {
            get
            {
                return _itemName;
            }
            set
            {
                string filterText = value;
                ICollectionView cv = CollectionViewSource.GetDefaultView(AccountTextiles);
                if (!string.IsNullOrEmpty(filterText))
                {
                    var splitText = filterText.Split(' ');
                    cv.Filter = o =>
                    {
                        /* change to get data row value */
                        AccountTextile p = o as AccountTextile;
                        string spec = p.ItemName ?? "";

                        bool isContains = true;
                        foreach (var item in splitText)
                        {
                            if (!spec.ToUpper().Contains(item.ToUpper()))
                            {
                                isContains = false;
                                break;
                            }
                        }
                        //isContains = p.I_03.ToUpper().Contains(filterText.ToUpper());
                        return isContains;
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
                _itemName = value;
            }
        }
        private void ButtonTrashItemInsertClickExecute()
        {
            if (DefaultPrice <= 0)
            {
                MessageBox.Show("未輸入預設單價！");
            }
            else
            {
                if (AccountTextileList.Where(w => w.FactoryID == SelectedAccountTextile.FactoryID && w.ItemID == SelectedAccountTextile.ItemID).Count() > 0)
                {
                    MessageBox.Show("此布種已加入清單！");

                }
                else
                    AccountTextileList.Add(new AccountTextile
                    {
                        FactoryID = SelectedAccountTextile.FactoryID,
                        ItemID = SelectedAccountTextile.ItemID,
                        ItemName = SelectedAccountTextile.ItemName,
                        DefaultPrice = DefaultPrice
                    });

                OnPropertyChanged("AccountTextileList");
            }
        }
        public int DefaultPrice { get; set; }
        public ObservableCollection<AccountTextile> AccountTextiles { get; set; }

        public AccountTextile SelectedAccountTextile { get; set; }
        public DefaultPriceViewModel()
        {
            IEnumerable<TrashItem> trashItemList = TrashModule.GetTrashItems();
            IEnumerable<AccountTextile> accountTextileList = AccountSystemModule.GetAccountTextile();
            IEnumerable<AccountTextile> accountTextiles = from trashItems in trashItemList
                                                          join accountTextile in accountTextileList on new { x1 = trashItems.F_01, x2 = trashItems.I_01 } equals new { x1 = accountTextile.FactoryID, x2 = accountTextile.ItemID } into leftjoin
                                                          from trashItemListLeft in leftjoin.DefaultIfEmpty()
                                                          select new AccountTextile
                                                          {
                                                              ItemID = trashItems.I_01,
                                                              FactoryID = trashItems.F_01,
                                                              ItemName = trashItems.I_03,
                                                              DefaultPrice = trashItemListLeft == null ? 0 : trashItemListLeft.DefaultPrice
                                                          };
            AccountTextiles = new ObservableCollection<AccountTextile>(accountTextiles);
        }
    }
}
