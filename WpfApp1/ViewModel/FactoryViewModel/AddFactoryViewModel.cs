using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfApp1.Command;
using WpfApp1.DataClass.Entity;
using WpfApp1.DataClass.Enumeration;
using WpfApp1.Modules.FactoryModule;
using WpfApp1.Modules.FactoryModule.Implement;
using WpfApp1.Utility;

namespace WpfApp1.ViewModel.FactoryViewModel
{
    public class AddFactoryViewModel : ViewModelBase
    {
        private Factory _factory { get; set; }
        protected IFactoryModule FactoryModule { get; } = new FactoryModule();
        //定義一個ICommand型別的參數，他會回傳實作ICommand介面的RelayCommand類別。
        public ICommand AddFactoryClick { get { return new RelayCommand(AddFactoryClickExecute, CanAddFactoryClickExecute); } }

        public AddFactoryViewModel()
        {
            _factory = new Factory();
        }

        public string Name
        {
            get { return _factory.Name; }
            set
            {
                if (_factory.Name != value)
                {
                    _factory.Name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }
        public string PhoneNumber
        {
            get { return _factory.PhoneNumber; }
            set
            {
                if (_factory.PhoneNumber != value)
                {
                    _factory.PhoneNumber = value;
                    RaisePropertyChanged("PhoneNumber");
                }
            }
        }
        public string CellPhone
        {
            get { return _factory.CellPhone; }
            set
            {
                if (_factory.CellPhone != value)
                {
                    _factory.CellPhone = value;
                    RaisePropertyChanged("CellPhone");
                }
            }
        }
        public string Fax
        {
            get { return _factory.Fax; }
            set
            {
                if (_factory.Fax != value)
                {
                    _factory.Fax = value;
                    RaisePropertyChanged("Fax");
                }
            }
        }
        public string Address
        {
            get { return _factory.Address; }
            set
            {
                if (_factory.Address != value)
                {
                    _factory.Address = value;
                    RaisePropertyChanged("Address");
                }
            }
        }
        public ProcessItem Process
        {
            get { return _factory.Process; }
            set
            {
                if (_factory.Process != value)
                {
                    _factory.Process = value;
                    RaisePropertyChanged("Process");
                }
            }
        }

        public int Sort
        {
            get { return _factory.Sort; }
            set
            {
                if (_factory.Sort != value)
                {
                    _factory.Sort = value;
                    RaisePropertyChanged("Sort");
                }
            }
        }

        void AddFactoryClickExecute()
        {
            if (Process.ToInt() == 0)
            {
                MessageBox.Show("請選擇加工項目!!");
                return;
            }
            if (string.IsNullOrEmpty(Name))
            {
                MessageBox.Show("請輸入工廠名稱!!");
                return;
            }
            var factory = new Factory()
            {
                Name = Name,
                PhoneNumber = PhoneNumber,
                CellPhone = CellPhone,
                Fax = Fax,
                Process = Process,
                Address = Address,
                Sort = Sort
            };

            int count = FactoryModule.InsertFactory(factory);
            if (count == 1)
            {
                MessageBox.Show("新增成功!!");
                Name = string.Empty;
                PhoneNumber = string.Empty;
                CellPhone = string.Empty;
                Fax = string.Empty;
                Address = string.Empty;
                Sort = 0;
            }
        }
        //定義是否可以更新Title
        bool CanAddFactoryClickExecute()
        {
            return true;
        }
    }

}
