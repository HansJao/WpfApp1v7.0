using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.AccountSystem;
using WpfApp1.DataClass.TrashSystem;

namespace WpfApp1.Modules.AccountSystemModule
{
    public interface IAccountSystemModule
    {
        /// <summary>
        /// 寫入布種預設單價
        /// </summary>
        /// <param name="trashItemPriceSetList"></param>
        /// <returns></returns>
        bool InsertDefaultPrice(IEnumerable<AccountTextile> trashItemPriceSetList);
        /// <summary>
        /// 取得所有布種預設單價
        /// </summary>
        /// <returns></returns>
        IEnumerable<AccountTextile> GetAccountTextile();
        /// <summary>
        /// 取得客戶布種單價
        /// </summary>
        /// <returns></returns>
        IEnumerable<CustomerTextilePrice> GetCustomerTextilePrice(string accountCustomerID);
        /// <summary>
        /// 合併帳務清單
        /// </summary>
        /// <returns></returns>
        IEnumerable<CustomerCheckBillSheet> GetCheckBillSheet(IEnumerable<AccountTextile> accountTextiles, IEnumerable<CustomerTextilePrice> customerTextilePrices, List<TrashCustomerShipped> invoSubList);
        /// <summary>
        /// 新增客戶布種單價
        /// </summary>
        /// <returns></returns>
        bool InsertCustomerTextilePrice(CustomerTextilePrice customerTextilePrice);
        /// <summary>
        /// 更新客戶布種單價
        /// </summary>
        /// <returns></returns>
        bool UpdateCustomerTextilePrice(CustomerCheckBillSheet selectedCustomerCheckBillSheet);
    }
}
