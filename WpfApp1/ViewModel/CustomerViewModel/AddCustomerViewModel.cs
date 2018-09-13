using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfApp1.Command;
using WpfApp1.DataClass.Entity;
using WpfApp1.Modules.CustomerModule;
using WpfApp1.Modules.CustomerModule.Implement;

namespace WpfApp1.ViewModel.CustomerViewModel
{
    public class AddCustomerViewModel : ViewModelBase
    {
        private Customer _customer { get; set; }
        protected ICustomerModule CustomerModule { get; } = new CustomerModule();
        //定義一個ICommand型別的參數，他會回傳實作ICommand介面的RelayCommand類別。
        public ICommand AddFactoryClick { get { return new RelayCommand(AddFactoryClickExecute, CanExecute); } }

        public AddCustomerViewModel()
        {
            _customer = new Customer();
        }

        public string Name
        {
            get { return _customer.Name; }
            set
            {
                if (_customer.Name != value)
                {
                    _customer.Name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }
        public string PhoneNumber
        {
            get { return _customer.PhoneNumber; }
            set
            {
                if (_customer.PhoneNumber != value)
                {
                    _customer.PhoneNumber = value;
                    RaisePropertyChanged("PhoneNumber");
                }
            }
        }
        public string CellPhone
        {
            get { return _customer.CellPhone; }
            set
            {
                if (_customer.CellPhone != value)
                {
                    _customer.CellPhone = value;
                    RaisePropertyChanged("CellPhone");
                }
            }
        }
        public string Fax
        {
            get { return _customer.Fax; }
            set
            {
                if (_customer.Fax != value)
                {
                    _customer.Fax = value;
                    RaisePropertyChanged("Fax");
                }
            }
        }
        public string Address
        {
            get { return _customer.Address; }
            set
            {
                if (_customer.Address != value)
                {
                    _customer.Address = value;
                    RaisePropertyChanged("Address");
                }
            }
        }
        public string Memo
        {
            get { return _customer.Memo; }
            set
            {
                if (_customer.Memo != value)
                {
                    _customer.Memo = value;
                    RaisePropertyChanged("Memo");
                }
            }
        }

        public int Sort
        {
            get { return _customer.Sort; }
            set
            {
                if (_customer.Sort != value)
                {
                    _customer.Sort = value;
                    RaisePropertyChanged("Sort");
                }
            }
        }

        void AddFactoryClickExecute()
        {

            if (string.IsNullOrEmpty(Name))
            {
                MessageBox.Show("請輸入客戶名稱!!");
                return;
            }
            var customer = new Customer()
            {
                Name = Name,
                PhoneNumber = PhoneNumber,
                CellPhone = CellPhone,
                Fax = Fax,
                Memo = Memo,
                Address = Address,
                Sort = Sort
            };

            bool success = CustomerModule.InsertCustomer(customer);
            if (success)
            {
                MessageBox.Show("新增成功!!");
                Name = string.Empty;
                PhoneNumber = string.Empty;
                CellPhone = string.Empty;
                Fax = string.Empty;
                Address = string.Empty;
                Memo = string.Empty;
                Sort = 0;
            }
        }
    }
}
