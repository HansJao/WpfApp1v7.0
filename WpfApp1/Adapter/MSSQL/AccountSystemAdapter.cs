﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.AccountSystem;
using WpfApp1.DataClass.TrashSystem;
using WpfApp1.Utility;

namespace WpfApp1.Adapter.MSSQL
{
    public class AccountSystemAdapter : IAccountSystemAdapter
    {
        /// <summary>
        /// 寫入布種預設單價
        /// </summary>
        /// <param name="trashItemPriceSetList"></param>
        /// <returns></returns>
        public int InsertDefaultPrice(IEnumerable<AccountTextile> accountTextileList)
        {
            string sql = @"INSERT INTO AccountTextile
                           VALUES 
                           (@FactoryID,@ItemID,@ItemName,@DefaultPrice);";
            var result = DapperHelper.Execute(AppSettingConfig.ConnectionString(), CommandType.Text, sql, accountTextileList);
            return result;
        }
        /// <summary>
        /// 取得所有布種預設單價
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AccountTextile> GetAccountTextile()
        {
            string sql = @"SELECT * FROM AccountTextile;";
            var result = DapperHelper.QueryCollection<AccountTextile>(AppSettingConfig.ConnectionString(), CommandType.Text, sql);
            return result;
        }
        /// <summary>
        /// 取得客戶布種單價
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CustomerTextilePrice> GetCustomerTextilePrice(string accountCustomerID)
        {
            string sql = @"SELECT * FROM CustomerTextilePrice
                            WHERE AccountCustomerID = @AccountCustomerID";
            SqlParameter[] parameters = new SqlParameter[]
          {
                new SqlParameter("@AccountCustomerID", SqlDbType.NChar) { Value = accountCustomerID }
          };
            var result = DapperHelper.QueryCollection<CustomerTextilePrice>(AppSettingConfig.ConnectionString(), CommandType.Text, sql, parameters);
            return result;
        }
        /// <summary>
        /// 新增客戶布種單價
        /// </summary>
        /// <returns></returns>
        public int InsertCustomerTextilePrice(CustomerTextilePrice customerTextilePrice)
        {
            string sql = @"INSERT INTO CustomerTextilePrice
                           VALUES 
                           (@AccountCustomerID,@AccountTextileID,@Price);";
            var result = DapperHelper.Execute(AppSettingConfig.ConnectionString(), CommandType.Text, sql, customerTextilePrice);
            return result;
        }
    }
}