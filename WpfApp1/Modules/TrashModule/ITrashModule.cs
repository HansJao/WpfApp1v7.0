using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.AccountSystem;
using WpfApp1.DataClass.TrashSystem;

namespace WpfApp1.Modules.TrashModule
{
    public interface ITrashModule
    {

        /// <summary>
        /// 取得布種名稱
        /// </summary>
        /// <returns></returns>
        IEnumerable<TrashItem> GetTrashItems();
        IEnumerable<TrashItem> GetTrashItemsByFeature(string feature);
        /// <summary>
        /// 依據日期取得布種出貨總數
        /// </summary>
        /// <param name="datePickerBegin"></param>
        /// <param name="datePickerEnd"></param>
        /// <returns></returns>        
        IEnumerable<TrashShipped> GetTrashShippedQuantitySum(DateTime datePickerBegin, DateTime datePickerEnd);
        /// <summary>
        /// 依據日期取得布種出貨清單
        /// </summary>
        /// <param name="datePickerBegin"></param>
        /// <param name="datePickerEnd"></param>
        /// <returns></returns>        
        IEnumerable<TrashShipped> GetTrashShippedList(DateTime datePickerBegin, DateTime datePickerEnd);
        /// <summary>
        /// 取得客戶出貨紀錄
        /// </summary>
        /// <param name="CheckBillDate"></param>
        /// <returns></returns>
        IEnumerable<TrashCustomerShipped> GetInvoSub(DateTime CheckBillDate);
        void UpdateProductName(string v);

        /// <summary>
        /// 取得客戶出貨紀錄
        /// </summary>
        /// <param name="customerName"></param>
        /// <param name="customerDatePickerBegin"></param>
        /// <param name="customerDatePickerEnd"></param>
        /// <returns></returns>
        IEnumerable<TrashCustomerShipped> GetCustomerShippedList(string customerName, DateTime customerDatePickerBegin, DateTime customerDatePickerEnd);
        /// <summary>
        /// 取得客戶清單
        /// </summary>
        IEnumerable<TrashCustomer> GetCustomerList();
        /// <summary>
        /// 以布種取得客戶出貨紀錄
        /// </summary>
        /// <param name="trashItem"></param>
        /// <returns></returns>
        IEnumerable<TrashCustomerShipped> GetCustomerShippedListByFeature(TrashItem trashItem);
        /// <summary>
        /// 以布種名稱取得客戶出貨紀錄
        /// </summary>
        /// <param name="textileName"></param>
        /// <returns></returns>
        IEnumerable<TrashCustomerShipped> GetCustomerShippedListByTextileName(string textileName, DateTime datePickerBegin, DateTime datePickerEnd);
        /// <summary>
        /// 更新帳務系統單價
        /// </summary>
        /// <param name="customerCheckBillSheet"></param>
        /// <param name="newPrice"></param>
        /// <param name="CheckBillDate"></param>
        /// <returns></returns>
        int UpdateInvoSubPrice(CustomerCheckBillSheet customerCheckBillSheet, int newPrice, DateTime CheckBillDate);
    }
}
