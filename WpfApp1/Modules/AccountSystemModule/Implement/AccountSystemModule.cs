using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Adapter;
using WpfApp1.Adapter.MSSQL;
using WpfApp1.DataClass.AccountSystem;
using WpfApp1.DataClass.TrashSystem;

namespace WpfApp1.Modules.AccountSystemModule.Implement
{
    public class AccountSystemModule : IAccountSystemModule
    {
        private IAccountSystemAdapter _accountSystemAdapter;
        protected IAccountSystemAdapter AccountSystemAdapter
        {
            get
            {
                if (this._accountSystemAdapter == null)
                {
                    this._accountSystemAdapter = new AccountSystemAdapter();
                }
                return this._accountSystemAdapter;
            }
            set
            {
                this._accountSystemAdapter = value;
            }
        }


        /// <summary>
        /// 寫入布種預設單價
        /// </summary>
        /// <param name="trashItemPriceSetList"></param>
        /// <returns></returns>
        public bool InsertDefaultPrice(IEnumerable<AccountTextile> accountTextileList)
        {
            int count = AccountSystemAdapter.InsertDefaultPrice(accountTextileList);
            return accountTextileList.Count() == count;
        }
    }
}
