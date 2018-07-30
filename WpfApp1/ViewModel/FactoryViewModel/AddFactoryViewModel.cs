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
        public Factory Factory { get; set; }
        protected IFactoryModule FactoryModule { get; } = new FactoryModule();
        //定義一個ICommand型別的參數，他會回傳實作ICommand介面的RelayCommand類別。
        public ICommand AddFactoryClick { get { return new RelayCommand(AddFactoryClickExecute, CanAddFactoryClickExecute); } }

        public AddFactoryViewModel()
        {
            Factory = new Factory();
        }

        public string Name
        {
            get { return Factory.Name; }
            set
            {
                if (Factory.Name != value)
                {
                    Factory.Name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }
        public string PhoneNumber
        {
            get { return Factory.PhoneNumber; }
            set
            {
                if (Factory.PhoneNumber != value)
                {
                    Factory.PhoneNumber = value;
                    RaisePropertyChanged("PhoneNumber");
                }
            }
        }
        public string CellPhone
        {
            get { return Factory.CellPhone; }
            set
            {
                if (Factory.CellPhone != value)
                {
                    Factory.CellPhone = value;
                    RaisePropertyChanged("CellPhone");
                }
            }
        }
        public string Fax
        {
            get { return Factory.Fax; }
            set
            {
                if (Factory.Fax != value)
                {
                    Factory.Fax = value;
                    RaisePropertyChanged("Fax");
                }
            }
        }
        public string Address
        {
            get { return Factory.Address; }
            set
            {
                if (Factory.Address != value)
                {
                    Factory.Address = value;
                    RaisePropertyChanged("Address");
                }
            }
        }
        public ProcessItem Process
        {
            get { return Factory.Process; }
            set
            {
                if (Factory.Process != value)
                {
                    Factory.Process = value;
                    RaisePropertyChanged("Process");
                }
            }
        }

        public int Sort
        {
            get { return Factory.Sort; }
            set
            {
                if (Factory.Sort != value)
                {
                    Factory.Sort = value;
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
