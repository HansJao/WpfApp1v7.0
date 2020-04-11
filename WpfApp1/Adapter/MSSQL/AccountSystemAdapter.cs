using System;
using System.Collections.Generic;
using System.Data;
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
    }
}
