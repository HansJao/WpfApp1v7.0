using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.Customer;
using WpfApp1.DataClass.Entity;

namespace WpfApp1.Adapter
{
    public interface ICustomerAdapter
    {

        /// <summary>
        /// 取得客戶名稱清單
        /// </summary>
        /// <returns></returns>
        IEnumerable<CustomerName> GetCustomerNameList();
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
    }
}
