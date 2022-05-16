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
using WpfApp1.DataClass.TrashSystem;
using WpfApp1.Modules.TrashModule;
using WpfApp1.Modules.TrashModule.Implement;
using WpfApp1.Windows.TrashSystemWindows;

namespace WpfApp1.ViewModel.TrashSystemViewModel
{
    public class FeatureSearchViewModel : ViewModelBase
    {
        protected ITrashModule TrashModule { get; } = new TrashModule();

        public ICommand DataGridDoubleClick { get { return new RelayCommand(DataGridDoubleClicExecute, CanExecute); } }
        public ICommand DataGridCustomerDoubleClick { get { return new RelayCommand(DataGridCustomerDoubleClickExecute, CanExecute); } }

        public TrashItem TrashItem { get; set; }
        public TrashCustomer SelectedTrashCustomer { get; set; }

        private void DataGridDoubleClicExecute()
        {
            CustomerOrderHistoryByFeatureDialog customerOrderHistoryByFeatureDialog = new CustomerOrderHistoryByFeatureDialog(TrashItem);
            customerOrderHistoryByFeatureDialog.Show();
        }

        private void DataGridCustomerDoubleClickExecute()
        {
            CustomerOrderHistoryDialog customerOrderHistoryDialog = new CustomerOrderHistoryDialog(SelectedTrashCustomer, DateTime.Now.AddYears(-3), DateTime.Now);
            customerOrderHistoryDialog.Show();
        }

        private string _feature { get; set; }
        public string Feature
        {
            get
            {
                return _feature;
            }
            set
            {
                string filterText = value;
                ICollectionView cv = CollectionViewSource.GetDefaultView(_trashItemList);
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
                _feature = value;
            }
        }
        private string _productNumber { get; set; }
        public string ProductNumber
        {
            get
            {
                return _productNumber;
            }
            set
            {
                string filterText = value;
                ICollectionView cv = CollectionViewSource.GetDefaultView(_trashItemList);
                if (!string.IsNullOrEmpty(filterText))
                {
                    cv.Filter = o =>
                    {
                        TrashItem p = o as TrashItem;
                        bool isContains = true;

                        if (!p.I_01.ToUpper().Contains(filterText.ToUpper()))
                        {
                            isContains = false;
                        }
                        return isContains;
                    };
                }
                else
                {
                    cv.Filter = o =>
                    {
                        return (true);
                    };
                };
                _productNumber = value; ;
            }
        }

        private string _customerNumber { get; set; }
        public string CustomerNumber
        {
            get
            {
                return _customerNumber;
            }
            set
            {
                string filterText = value;
                ICollectionView cv = CollectionViewSource.GetDefaultView(TrashCustomerList);
                if (!string.IsNullOrEmpty(filterText))
                {
                    cv.Filter = o =>
                    {
                        TrashCustomer p = o as TrashCustomer;
                        bool isContains = true;

                        if (!p.CARD_NO.ToUpper().Contains(filterText.ToUpper()))
                        {
                            isContains = false;
                        }
                        return isContains;
                    };
                }
                else
                {
                    cv.Filter = o =>
                    {
                        return (true);
                    };
                };
                _customerNumber = value; ;
            }
        }

        private string _customerName { get; set; }
        public string CustomerName
        {
            get
            {
                return _customerName;
            }
            set
            {
                string filterText = value;
                ICollectionView cv = CollectionViewSource.GetDefaultView(TrashCustomerList);
                if (!string.IsNullOrEmpty(filterText))
                {
                    cv.Filter = o =>
                    {
                        TrashCustomer p = o as TrashCustomer;
                        bool isContains = true;

                        if (!p.C_NAME.ToUpper().Contains(filterText.ToUpper()))
                        {
                            isContains = false;
                        }
                        return isContains;
                    };
                }
                else
                {
                    cv.Filter = o =>
                    {
                        return (true);
                    };
                };
                _customerName = value; ;
            }
        }

        private ObservableCollection<TrashItem> _trashItemList { get; set; }

        public ObservableCollection<TrashItem> TrashItemList
        {
            get { return _trashItemList; }
            set { _trashItemList = value; }
        }
        public IEnumerable<TrashCustomer> TrashCustomerList { get; set; }
        public FeatureSearchViewModel()
        {
            _trashItemList = new ObservableCollection<TrashItem>(TrashModule.GetTrashItems().OrderBy(o => o.I_01));
            TrashCustomerList = TrashModule.GetCustomerList();
        }
    }
}
