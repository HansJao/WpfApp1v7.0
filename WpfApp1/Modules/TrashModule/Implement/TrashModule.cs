﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Adapter;
using WpfApp1.Adapter.DBF;
using WpfApp1.DataClass.AccountSystem;
using WpfApp1.DataClass.Fabric;
using WpfApp1.DataClass.TrashSystem;
using WpfApp1.Utility;

namespace WpfApp1.Modules.TrashModule.Implement
{
    public class TrashModule : ITrashModule
    {
        private ITrashAdapter _trashAdapter;
        protected ITrashAdapter TrashAdapter
        {
            get
            {
                if (this._trashAdapter == null)
                {
                    this._trashAdapter = new TrashAdapter();
                }
                return this._trashAdapter;
            }
            set
            {
                this._trashAdapter = value;
            }
        }

        public IEnumerable<TrashItem> GetTrashItems()
        {
            var trashItems = TrashAdapter.GetTrashItems();
            return trashItems;
        }

        public IEnumerable<TrashItem> GetTrashItemsByFeature(string feature)
        {
            IEnumerable<TrashItem> trashItems = TrashAdapter.GetTrashItemsByFeature(feature);
            return trashItems;
        }

        /// <summary>
        /// 依據日期取得布種出貨總數
        /// </summary>
        /// <param name="datePickerBegin"></param>
        /// <param name="datePickerEnd"></param>
        /// <returns></returns>        
        public IEnumerable<TrashShipped> GetTrashShippedQuantitySum(DateTime datePickerBegin, DateTime datePickerEnd)
        {
            IEnumerable<TrashShipped> trashList = TrashAdapter.GetTrashShippedList(datePickerBegin, datePickerEnd).GroupBy(g =>
             new
             {
                 g.IN_DATE,
                 g.I_01,
                 g.I_03
             }).Select(s => new TrashShipped
             {
                 IN_DATE = s.Key.IN_DATE,
                 I_01 = s.Key.I_01,
                 I_03 = s.Key.I_03,
                 Quantity = s.Sum(ss => ss.Quantity)
             });
            return trashList;
        }
        /// <summary>
        /// 依據日期取得布種出貨清單
        /// </summary>
        /// <param name="datePickerBegin"></param>
        /// <param name="datePickerEnd"></param>
        /// <returns></returns>        
        public IEnumerable<TrashShipped> GetTrashShippedList(DateTime datePickerBegin, DateTime datePickerEnd)
        {
            IEnumerable<TrashShipped> trashList = TrashAdapter.GetTrashShippedList(datePickerBegin, datePickerEnd);
            return trashList;
        }

        /// <summary>
        /// 取得客戶出貨紀錄
        /// </summary>
        /// <param name="CheckBillDate"></param>
        /// <returns></returns>        
        public IEnumerable<TrashCustomerShipped> GetInvoSub(DateTime CheckBillDate)
        {
            IEnumerable<TrashCustomerShipped> invoSub = TrashAdapter.GetInvoSub(CheckBillDate);
            return invoSub;
        }

        public void UpdateProductName(string v)
        {
            TrashAdapter.UpdateProductName(v);
        }

        /// <summary>
        /// 取得客戶出貨紀錄
        /// </summary>
        /// <param name="customerName"></param>
        /// <param name="customerDatePickerBegin"></param>
        /// <param name="customerDatePickerEnd"></param>
        /// <returns></returns>
        public IEnumerable<TrashCustomerShipped> GetCustomerShippedList(string customerName, DateTime customerDatePickerBegin, DateTime customerDatePickerEnd)
        {
            IEnumerable<TrashCustomerShipped> trashCustomerShipped = TrashAdapter.GetCustomerShippedList(customerName, customerDatePickerBegin, customerDatePickerEnd);
            //var test = trashCustomerShipped.Where(w => w.I_03.Contains("仿韓國棉黑")).OrderBy(o => o.I_03);
            return trashCustomerShipped;
        }
        /// <summary>
        /// 取得客戶清單
        /// </summary>
        public IEnumerable<TrashCustomer> GetCustomerList()
        {
            IEnumerable<TrashCustomer> trashCustomers = TrashAdapter.GetCustomerList();
            return trashCustomers;
        }
        /// <summary>
        /// 以布種取得客戶出貨紀錄
        /// </summary>
        /// <param name="trashItem"></param>
        /// <returns></returns>
        public IEnumerable<TrashCustomerShipped> GetCustomerShippedListByFeature(TrashItem trashItem)
        {
            IEnumerable<TrashCustomerShipped> trashCustomerShippeds = TrashAdapter.GetCustomerShippedListByFeature(trashItem);
            return trashCustomerShippeds;
        }
        /// <summary>
        /// 以布種名稱取得客戶出貨紀錄
        /// </summary>
        /// <param name="textileName"></param>
        /// <returns></returns>
        public IEnumerable<TrashCustomerShipped> GetCustomerShippedListByTextileName(string textileName, DateTime datePickerBegin, DateTime datePickerEnd)
        {
            IEnumerable<TrashCustomerShipped> trashCustomerShippeds = TrashAdapter.GetCustomerShippedListByTextileName(textileName, datePickerBegin, datePickerEnd);
            return trashCustomerShippeds;
        }
        /// <summary>
        /// 更新帳務系統單價
        /// </summary>
        /// <param name="customerCheckBillSheet"></param>
        /// <param name="newPrice"></param>
        /// <param name="CheckBillDate"></param>
        /// <returns></returns>
        public int UpdateInvoSubPrice(CustomerCheckBillSheet customerCheckBillSheet, int newPrice, DateTime CheckBillDate)
        {
            int updateCount = TrashAdapter.UpdateInvoSubPrice(customerCheckBillSheet, newPrice, CheckBillDate);
            return updateCount;
        }
    }
}
