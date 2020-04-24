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
                ICollectionView cv = CollectionViewSource.GetDefaultView(TrashItemList);
                if (!string.IsNullOrEmpty(filterText))
                {
                    var splitText = filterText.Split(' ');
                    cv.Filter = o =>
                    {
                        /* change to get data row value */
                        TrashItem p = o as TrashItem;
                        string spec = p.I_03 ?? "";

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
                if (AccountTextileList.Where(w => w.FactoryID == SelectedTrashItem.F_01 && w.ItemID == SelectedTrashItem.I_01).Count() > 0)
                {
                    MessageBox.Show("此布種已加入清單！");

                }
                else
                    AccountTextileList.Add(new AccountTextile
                    {
                        FactoryID = SelectedTrashItem.F_01,
                        ItemID = SelectedTrashItem.I_01,
                        ItemName = SelectedTrashItem.I_03,
                        DefaultPrice = DefaultPrice
                    });

                OnPropertyChanged("AccountTextileList");
            }
        }
        public int DefaultPrice { get; set; }
        public ObservableCollection<TrashItem> TrashItemList { get; set; }

        public TrashItem SelectedTrashItem { get; set; }
        public DefaultPriceViewModel()
        {
            TrashItemList = new ObservableCollection<TrashItem>(TrashModule.GetTrashItems().OrderBy(o => o.I_01));
        }
    }
}
