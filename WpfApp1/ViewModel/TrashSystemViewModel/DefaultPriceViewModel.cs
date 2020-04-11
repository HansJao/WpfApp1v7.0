using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
    public class DefaultPriceViewModel : ViewModelBase
    {

        protected IAccountSystemModule AccountSystemModule { get; } = new AccountSystemModule();
        protected ITrashModule TrashModule { get; } = new TrashModule();

        public ICommand ButtonTrashItemInsertClick { get { return new RelayCommand(ButtonTrashItemInsertClickExecute, CanExecute); } }
        public ICommand ButtonUpdateDefaultPriceClick { get { return new RelayCommand(ButtonUpdateDefaultPriceClickExecute, CanExecute); } }

        private void ButtonUpdateDefaultPriceClickExecute()
        {
            bool success = AccountSystemModule.InsertDefaultPrice(AccountTextileList);
        }
        public List<AccountTextile> AccountTextileList { get; set; } = new List<AccountTextile>();
        private void ButtonTrashItemInsertClickExecute()
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
        }
        public int DefaultPrice { get; set; }
        public ObservableCollection<TrashItem> TrashItemList { get; set; }
        public ObservableCollection<TrashItem> TrashItemPriceSetList { get; set; } = new ObservableCollection<TrashItem>();

        public TrashItem SelectedTrashItem { get; set; }
        public DefaultPriceViewModel()
        {
            TrashItemList = new ObservableCollection<TrashItem>(TrashModule.GetTrashItems().OrderBy(o => o.I_01));
        }
    }
}
