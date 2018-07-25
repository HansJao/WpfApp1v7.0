using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.Customer;
using WpfApp1.DataClass.Entity;
using WpfApp1.Utility;

namespace WpfApp1.Adapter.MSSQL
{
    public class CustomerAdapter : ICustomerAdapter
    {
        /// <summary>
        /// 取得客戶清單
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Customer> GetCustomerList()
        {
            var sqlCmd = "SELECT * FROM Customer";
            var result = DapperHelper.QueryCollection<Customer>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd);
            return result;
        }

        /// <summary>
        /// 取得客戶名稱清單
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CustomerName> GetCustomerNameList()
        {
            var sqlCmd = "SELECT Name FROM Customer";
            var result = DapperHelper.QueryCollection<CustomerName>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd);

            return result;
        }

        public int UpdateCustomer(Customer customer)
        {
            var sqlCmd = @"UPDATE [dbo].[Customer]
                           SET [PhoneNumber] = @PhoneNumber
                           ,[CellPhone] = @CellPhone
                           ,[Fax] = @Fax
                           ,[Address] = @Address
                           ,[Memo] = @Memo
                           ,[Sort] = @Sort
                           WHERE CustomerID=@CustomerID";
            var result = DapperHelper.Execute(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, customer);
            return result;
        }
    }
}
