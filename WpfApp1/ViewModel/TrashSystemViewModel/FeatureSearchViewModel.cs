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

namespace WpfApp1.ViewModel.TrashSystemViewModel
{
    public class FeatureSearchViewModel : ViewModelBase
    {
        protected ITrashModule TrashModule { get; } = new TrashModule();
        public ICommand GetFeatureExecuteClick { get { return new RelayCommand(GetFeatureExecute, CanExecute); } }

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
                    cv.Filter = o =>
                    {
                        /* change to get data row value */
                        TrashItem p = o as TrashItem;
                        return (p.I_03.ToUpper().Contains(filterText.ToUpper()));
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
                _feature = value; ;
            }
        }

        private ObservableCollection<TrashItem> _trashItemList { get; set; }

        public ObservableCollection<TrashItem> TrashItemList
        {
            get { return _trashItemList; }
            set { _trashItemList = value; }
        }

        private void GetFeatureExecute()
        {
            //_trashItemList = TrashModule.GetTrashItems();
        }

        public FeatureSearchViewModel()
        {
            _trashItemList = new ObservableCollection<TrashItem>(TrashModule.GetTrashItems());
        }
    }
}
