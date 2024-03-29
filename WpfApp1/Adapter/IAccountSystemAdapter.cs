﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.AccountSystem;
using WpfApp1.DataClass.TrashSystem;

namespace WpfApp1.Adapter
{
    public interface IAccountSystemAdapter
    {
        /// <summary>
        /// 寫入布種預設單價
        /// </summary>
        /// <param name="trashItemPriceSetList"></param>
        /// <returns></returns>
        int InsertDefaultPrice(IEnumerable<AccountTextile> accountTextileList);

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
        /// 新增客戶布種單價
        /// </summary>
        /// <returns></returns>
        int InsertCustomerTextilePrice(CustomerTextilePrice customerTextilePrice);
        /// <summary>
        /// 更新客戶布種單價
        /// </summary>
        /// <returns></returns>
        int UpdateCustomerTextilePrice(CustomerCheckBillSheet selectedCustomerCheckBillSheet,int updateCustomerPrice);
    }
}
