using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.Entity;
using WpfApp1.DataClass.Enumeration;
using WpfApp1.DataClass.ProcessOrder;

namespace WpfApp1.Adapter
{
    public interface IProcessOrderAdapter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<ProcessOrder> GetProcessOrder();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        IEnumerable<ProcessOrderFlowFactoryName> GetProcessOrderFlow(int orderNo);

        IEnumerable<ProcessOrderColorDetail> GetProcessOrderColorDetailByOrderNo(int orderNo);

        int InsertProcessOrder(ProcessOrder processOrder);

        int InsertProcessOrderFlow(List<ProcessOrderFlow> processOrderFlow);

        /// <summary>
        /// 新增加工訂單顏色明細
        /// </summary>
        /// <param name="processOrderColorDetail"></param>
        /// <returns>取得加工訂單顏色明細id清單</returns>
        IEnumerable<int> InsertProcessOrderColorDetail(List<ProcessOrderColorDetail> processOrderColorDetail);

        int DeleteProcessOrder(ProcessOrder processOrder);

        int UpdateProcessOrderFlowDate(List<ProcessOrderFlowFactoryName> processOrderFlow);

        /// <summary>
        /// 取得工廠直送清單
        /// </summary>
        /// <param name="orderColorDetailNo"></param>
        /// <returns></returns>
        IEnumerable<FactoryShippingName> GetFactoryShipping(int orderColorDetailNo);
        /// <summary>
        /// 新增工廠直送
        /// </summary>
        /// <param name="factoryShippingName"></param>
        /// <returns></returns>
        int InsertFactoryShipping(FactoryShippingName factoryShippingName);

        int DeleteFactoryShipping(int shippingNo);

        /// <summary>
        /// 取得加工廠出貨明細
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        IEnumerable<ProcessOrderColorFactoryShippingDetail> GetProcessOrderColorFactoryShippingDetail(int orderNo);
        /// <summary>
        /// 依據狀態取得加工訂單
        /// </summary>
        /// <param name="Status"></param>
        /// <returns></returns>
        IEnumerable<ProcessOrder> GetProcessOrderByStatus(ProcessOrderColorStatus status);
        /// <summary>
        /// 更新加工訂單顏色狀態
        /// </summary>
        /// <param name="orderColorDetailNo"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        int UpdateProcessOrderFlowDate(int orderColorDetailNo, ProcessOrderColorStatus status);
        /// <summary>
        /// 取得加工訂單流程明細
        /// </summary>
        /// <param name="OrderColorDetailNo"></param>
        /// <returns></returns>
        IEnumerable<ProcessOrderFlowDateDetail> GetProcessOrderFlowDateDetail(List<int> OrderColorDetailNo);
        /// <summary>
        /// 新增加工訂單流程時間
        /// </summary>
        /// <param name="proecessOrderFlowDateList"></param>
        /// <returns></returns>
        int InsertProcessOrderFlowDate(List<ProcessOrderFlowDate> proecessOrderFlowDateList);
        /// <summary>
        /// 更新加工訂單流程時間
        /// </summary>
        /// <param name="orderFlowNo"></param>
        /// <param name="orderColorDetailNoList"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        int UpdateProcessOrderFlowInputDate(int orderFlowNo, IEnumerable<int> orderColorDetailNoList, DateTime? date);
        /// <summary>
        /// 更新加工訂單流程時間
        /// </summary>
        /// <param name="orderFlowNo"></param>
        /// <param name="orderColorDetailNoList"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        int UpdateProcessOrderFlowCompleteDate(int orderFlowNo, IEnumerable<int> orderColorDetailNoList, DateTime? date);
        /// <summary>
        /// 依據狀態,工廠取得加工訂單
        /// </summary>
        /// <param name="factoryList"></param>
        /// <param name="statusList"></param>
        /// <returns></returns>
        IEnumerable<ProcessOrder> GetProcessOrderFilter(IEnumerable<int> factoryIDList, List<ProcessOrderColorStatus> statusList);
        /// <summary>
        /// 取得加工訂單顏色明細
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        IEnumerable<ProcessOrderColorDetail> GetProcessOrderColorDetail(IEnumerable<int> processOrderColorDetailNoList);
        /// <summary>
        /// 修改加工訂單顏色明細數量
        /// </summary>
        /// <param name="orderColorDetailNo"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        int UpdateProcessOrderColorDetailQuantity(int orderColorDetailNo, int quantity);
        /// <summary>
        /// 修改加工訂單顏色明細顏色
        /// </summary>
        /// <param name="orderColorDetailNo"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        int UpdateProcessOrderColorDetailColor(int orderColorDetailNo, string color);
        /// <summary>
        /// 修改加工訂單顏色明細色號
        /// </summary>
        /// <param name="orderColorDetailNo"></param>
        /// <param name="colorNumber"></param>
        /// <returns></returns>
        int UpdateProcessOrderColorDetailColorNumber(int orderColorDetailNo, string colorNumber);
        /// <summary>
        /// 刪除加工訂單顏色明細
        /// </summary>
        /// <param name="orderColorDetailNo"></param>
        /// <returns></returns>
        int DeleteFactoryShippingDetail(int orderColorDetailNo);
        /// <summary>
        /// 取得新增或修改的加工訂單明細
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        IEnumerable<ProcessOrder> GetNewOrEditProcessOrder(DateTime dateTime, IEnumerable<int> orderNo);
        /// <summary>
        /// 依據多個訂單號取得加工訂單顏色明細清單
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        IEnumerable<ProcessOrderColorDetail> GetProcessOrderColorDetailList(IEnumerable<int> orderNo);
        /// <summary>
        /// 依據多個加工訂單顏色明細編號號取得工廠直送清單
        /// </summary>
        /// <param name="processOrderColorDetailNo"></param>
        /// <returns></returns>
        IEnumerable<FactoryShippingName> GetFactoryShippingNameList(IEnumerable<int> processOrderColorDetailNo);
        /// <summary>
        /// 更新加工訂單流程工廠名稱
        /// </summary>
        /// <param name="selectedFactoryItem"></param>
        /// <param name="orderFlowNo"></param>
        /// <returns></returns>
        bool EditProcessOrderFlowFactory(int selectedFactoryItem, int orderFlowNo);
        /// <summary>
        /// 更新加工訂單備註
        /// </summary>
        /// <param name="processOrderNo"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        int UpdateProcessOrderRemark(int processOrderNo, string text);
        /// <summary>
        /// 取得加工訂單備註
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        string GetProcessOrderRemark(int orderNo);
        /// <summary>
        /// 更新加工訂單顏色明細狀態為已完成
        /// </summary>
        /// <param name="orderFlowNo"></param>
        /// <param name="orderColorDetailNoList"></param>
        /// <returns></returns>
        int UpdateProcessOrderColorDetailStatusByLastComplete(int orderFlowNo, IEnumerable<int> orderColorDetailNoList);
        /// <summary>
        /// 新增加工訂單流程
        /// </summary>
        /// <param name="processOrderFlow"></param>
        /// <param name="orderColorDetailNo"></param>
        /// <returns></returns>
        int NewProcessOrderFlow(ProcessOrderFlow processOrderFlow, IEnumerable<int> orderColorDetailNo);
    }
}
