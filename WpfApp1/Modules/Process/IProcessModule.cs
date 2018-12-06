using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.Entity;
using WpfApp1.DataClass.Enumeration;
using WpfApp1.DataClass.ProcessOrder;

namespace WpfApp1.Modules.Process
{
    public interface IProcessModule
    {
        /// <summary>
        /// 取得加工訂單明細
        /// </summary>
        /// <returns></returns>
        IEnumerable<ProcessOrder> GetProcessOrder();

        /// <summary>
        /// 取得加工訂單明細
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        IEnumerable<ProcessOrderFlowFactoryName> GetProcessOrderFlow(int orderNo);

        /// </summary>
        /// 取得新增或修改的加工訂單明細
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        List<ProcessOrderStructure> GetNewOrEditProcessOrderStructures(DateTime dateTime, IEnumerable<int> orderNo);

        /// <summary>
        /// 以訂單編號取得加工訂單顏色明細
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        IEnumerable<ProcessOrderColorDetail> GetProcessOrderColorDetailByOrderNo(int orderNo);

        /// <summary>
        /// 新增加工訂單
        /// </summary>
        /// <param name="processOrder"></param>
        /// <returns></returns>
        int InsertProcessOrder(ProcessOrder processOrder);

        int InsertProcessOrderFlow(List<ProcessOrderFlow> processOrderFlow);
        /// <summary>
        /// 新增加工訂單顏色明細
        /// </summary>
        /// <param name="processOrderColorDetail"></param>
        /// <returns>取得加工訂單顏色明細id清單</returns>
        IEnumerable<int> InsertProcessOrderColorDetail(List<ProcessOrderColorDetail> processOrderColorDetail);

        int DeleteProcessOrder(ProcessOrder processOrder);

        int UpdateProcessOrderFlowDate(List<ProcessOrderFlowFactoryName> processOrderFlowFactoryName);
        

        /// <summary>
        /// 更新加工訂單流程工廠名稱
        /// </summary>
        /// <param name="selectedFactoryItem"></param>
        /// <param name="orderFlowNo"></param>
        /// <returns></returns>
        bool EditProcessOrderFlowFactory(int selectedFactoryItem, int orderFlowNo);

        /// <summary>
        /// 取得工廠直送清單
        /// </summary>
        /// <param name="orderColorDetailNo"></param>
        /// <returns></returns>
        IEnumerable<FactoryShippingName> GetFactoryShipping(int orderColorDetailNo);

        /// <summary>
        /// 新增工廠直送
        /// </summary>
        /// <param name="factoryShipping"></param>
        /// <returns></returns>
        int InsertFactoryShipping(FactoryShippingName factoryShipping);

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
        IEnumerable<ProcessOrder> GetProcessOrderByStatus(ProcessOrderColorStatus Status);
        /// <summary>
        /// 更新加工訂單顏色狀態
        /// </summary>
        /// <param name="orderColorDetailNo"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        int UpdateProcessOrderColorStatus(int orderColorDetailNo, ProcessOrderColorStatus status);
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
        bool UpdateProcessOrderFlowCompleteDate(int orderFlowNo, IEnumerable<int> orderColorDetailNoList, DateTime? date);
        /// <summary>
        /// 依據狀態,工廠取得加工訂單
        /// </summary>
        /// <param name="factoryList"></param>
        /// <param name="statusList"></param>
        /// <returns></returns>
        IEnumerable<ProcessOrder> GetProcessOrderFilter(List<Factory> factoryList, List<ProcessOrderColorStatus> statusList);

       

        /// <summary>
        /// 取得加工訂單顏色明細
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        IEnumerable<ProcessOrderColorDetail> GetProcessOrderColorDetail(IEnumerable<int> processOrderColorDetailNoList);

        /// <summary>
        /// 建立加工訂單流程
        /// </summary>
        /// <param name="processOrderColorDetailList"></param>
        /// <param name="orderNo"></param>
        void CreateProcessOrderColorFlow(List<ProcessOrderColorDetail> processOrderColorDetailList, int orderNo);


        /// <summary>
        /// 修改加工訂單顏色明細
        /// </summary>
        /// <param name="orderColorDetailNo"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        int UpdateProcessOrderColorDetailQuantity(int orderColorDetailNo,int quantity);
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
        /// 更新加工訂單備註
        /// </summary>
        /// <param name="processOrderNo"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        bool UpdateProcessOrderRemark(int processOrderNo, string text);

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
        bool UpdateProcessOrderColorDetailStatusByLastComplete(int orderFlowNo, IEnumerable<int> orderColorDetailNoList);

        /// <summary>
        /// 新增加工訂單流程
        /// </summary>
        /// <param name="processOrderFlow"></param>
        /// <param name="orderColorDetailNo"></param>
        /// <returns></returns>
        int NewProcessOrderFlow(ProcessOrderFlow processOrderFlow, IEnumerable<int> orderColorDetailNo);

        /// <summary>
        /// 取得加工訂單依照工廠加工轉入轉出的更新時間排序
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        IEnumerable<ProcessOrder> GetProcessOrderByFactoryUpdateDate(string dateTime);

        /// <summary>
        /// 依據顏色取得加工訂單
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        IEnumerable<ProcessOrder> GetProcessOrderByColor(string color);
    }
}
