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

        /// <summary>
        /// 取得所有布種預設單價
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AccountTextile> GetAccountTextile()
        {
            IEnumerable<AccountTextile> accountTextiles = AccountSystemAdapter.GetAccountTextile();
            return accountTextiles;
        }
        /// <summary>
        /// 取得客戶布種單價
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CustomerTextilePrice> GetCustomerTextilePrice(string accountCustomerID)
        {
            IEnumerable<CustomerTextilePrice> customerTextilePrices = AccountSystemAdapter.GetCustomerTextilePrice(accountCustomerID);
            return customerTextilePrices;
        }
        /// <summary>
        /// 合併帳務清單
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CustomerCheckBillSheet> GetCheckBillSheet(IEnumerable<AccountTextile> accountTextiles, IEnumerable<CustomerTextilePrice> customerTextilePrices, List<TrashCustomerShipped> invoSubList)
        {
            List<CustomerCheckBillSheet> customerCheckBillSheets = new List<CustomerCheckBillSheet>();

            //var result = from c in T_Classes
            //             join
            //s in T_Students on c.ClassID equals s.ClassID
            //             where c.ClassName == "一年級"
            //             orderby c.ClassID descending
            //             select new { c.ClassName, s.StudentName };
            //result.Dump();

            IEnumerable<CustomerCheckBillSheet> result = from invoSub in invoSubList
                                                         join accountTextile in accountTextiles on new { x1 = invoSub.F_01, x2 = invoSub.I_01 } equals new { x1 = accountTextile.FactoryID, x2 = accountTextile.ItemID } into leftjoin
                                                         from invoSubListLeft in leftjoin.DefaultIfEmpty()
                                                         join CTP in customerTextilePrices on invoSubListLeft == null ? -1 : invoSubListLeft.AccountTextileID equals CTP.AccountTextileID into leftjoin2
                                                         from CTPLeft in leftjoin2.DefaultIfEmpty()
                                                         select new CustomerCheckBillSheet
                                                         {
                                                             IN_DATE = invoSub.IN_DATE,
                                                             C_Name = invoSub.C_Name,
                                                             F_01 = invoSub.F_01,
                                                             I_01 = invoSub.I_01,
                                                             C_01 = invoSub.C_01,
                                                             IN_NO = invoSub.IN_NO,
                                                             I_03 = invoSub.I_03,
                                                             Quantity = invoSub.Quantity,
                                                             Price = invoSub.Price,
                                                             CustomerPrice = CTPLeft == null ? 0 : CTPLeft.Price,
                                                             DefaultPrice = invoSubListLeft == null ? 0 : invoSubListLeft.DefaultPrice
                                                         };
            return result;
        }
    }
}
