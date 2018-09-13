using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.Customer;
using WpfApp1.DataClass.Entity;

namespace WpfApp1.Modules.CustomerModule
{
    public interface ICustomerModule
    {
        /// <summary>
        /// 取得客戶名稱清單
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetCustomerNameList();
        /// <summary>
        /// 取得客戶清單
        /// </summary>
        /// <returns></returns>
        IEnumerable<Customer> GetCustomerList();
        /// <summary>
        /// 更新客戶明細
        /// </summary>
        /// <returns></returns>
        int UpdateCustomer(Customer customer);
        /// <summary>
        /// 新增客戶明細
        /// </summary>
        /// <returns></returns>
        bool InsertCustomer(Customer customer);
    }
}
