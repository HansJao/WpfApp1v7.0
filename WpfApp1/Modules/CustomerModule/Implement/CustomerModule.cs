using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Adapter;
using WpfApp1.Adapter.MSSQL;
using WpfApp1.DataClass.Customer;
using WpfApp1.DataClass.Entity;

namespace WpfApp1.Modules.CustomerModule.Implement
{
    public class CustomerModule : ICustomerModule
    {
        private ICustomerAdapter _customerAdapter;
        protected ICustomerAdapter CustomerAdapter
        {
            get
            {
                if (this._customerAdapter == null)
                {
                    this._customerAdapter = new CustomerAdapter();
                }
                return this._customerAdapter;
            }
            set
            {
                this._customerAdapter = value;
            }
        }
        /// <summary>
        /// 取得客戶清單
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Customer> GetCustomerList()
        {
            var result = CustomerAdapter.GetCustomerList().OrderBy(o => o.Sort == 0).ThenBy(o => o.Sort);
            return result;
        }

        /// <summary>
        /// 取得客戶名稱清單
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetCustomerNameList()
        {
            try
            {
                var result = CustomerAdapter.GetCustomerList().OrderBy(o => o.Sort == 0).ThenBy(o => o.Sort);
                return result.Select(s => s.Name.ToString());
            }
            catch (Exception)
            {
                return AppSettingConfig.CustomerList();
            }

        }
        /// <summary>
        /// 更新客戶明細
        /// </summary>
        /// <returns></returns>
        public int UpdateCustomer(Customer customer)
        {
            int count = CustomerAdapter.UpdateCustomer(customer);
            return count;
        }
    }
}
