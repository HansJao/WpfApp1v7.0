using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Adapter;
using WpfApp1.Adapter.MSSQL;
using WpfApp1.DataClass.Entity;
using WpfApp1.DataClass.Enumeration;
using WpfApp1.DataClass.ProcessOrder;

namespace WpfApp1.Modules.Process.Implement
{
    public class ProcessModule : IProcessModule
    {
        private IProcessOrderAdapter _processOrderAdapter;
        protected IProcessOrderAdapter ProcessOrderAdapter
        {
            get
            {
                if (this._processOrderAdapter == null)
                {
                    this._processOrderAdapter = new ProcessOrderAdapter();
                }
                return this._processOrderAdapter;
            }
            set
            {
                this._processOrderAdapter = value;
            }
        }

        public int DeleteFactoryShipping(int shippingNo)
        {
            var result = ProcessOrderAdapter.DeleteFactoryShipping(shippingNo);
            return result;
        }

        public int DeleteProcessOrder(ProcessOrder processOrder)
        {
            var result = ProcessOrderAdapter.DeleteProcessOrder(processOrder);
            return result;
        }
        /// <summary>
        /// 取得工廠直送清單
        /// </summary>
        /// <param name="orderColorDetailNo"></param>
        /// <returns></returns>
        public IEnumerable<FactoryShippingName> GetFactoryShipping(int orderColorDetailNo)
        {
            var result = ProcessOrderAdapter.GetFactoryShipping(orderColorDetailNo);
            return result;

        }

        public IEnumerable<ProcessOrderColorFactoryShippingDetail> GetProcessOrderColorFactoryShippingDetail(int orderNo)
        {
            var result = ProcessOrderAdapter.GetProcessOrderColorFactoryShippingDetail(orderNo);
            return result;
        }

        public IEnumerable<ProcessOrder> GetProcessOrder()
        {
            var result = ProcessOrderAdapter.GetProcessOrder().OrderByDescending(o => o.OrderNo);
            return result;
        }

        public IEnumerable<ProcessOrder> GetProcessOrderByStatus(ProcessOrderColorStatus status)
        {
            IEnumerable<ProcessOrder> result = ProcessOrderAdapter.GetProcessOrderByStatus(status).OrderByDescending(o => o.OrderNo);
            return result;
        }

        public IEnumerable<ProcessOrderColorDetail> GetProcessOrderColorDetailByOrderNo(int orderNo)
        {
            var result = ProcessOrderAdapter.GetProcessOrderColorDetailByOrderNo(orderNo);
            return result;
        }

        /// <summary>
        /// 取得加工訂單明細
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public IEnumerable<ProcessOrderFlowFactoryName> GetProcessOrderFlow(int orderNo)
        {
            var result = ProcessOrderAdapter.GetProcessOrderFlow(orderNo);
            return result;
        }
        /// <summary>
        /// 新增工廠直送
        /// </summary>
        /// <param name="factoryShippingName"></param>
        /// <returns></returns>
        public int InsertFactoryShipping(FactoryShippingName factoryShippingName)
        {
            var result = ProcessOrderAdapter.InsertFactoryShipping(factoryShippingName);
            return result;
        }
        /// <summary>
        /// 新增加工訂單
        /// </summary>
        /// <param name="processOrder"></param>
        /// <returns></returns>
        public int InsertProcessOrder(ProcessOrder processOrder)
        {
            var result = ProcessOrderAdapter.InsertProcessOrder(processOrder);
            return result;
        }
        /// <summary>
        /// 新增加工訂單顏色明細
        /// </summary>
        /// <param name="processOrderColorDetail"></param>
        /// <returns>取得加工訂單顏色明細id清單</returns>
        public IEnumerable<int> InsertProcessOrderColorDetail(List<ProcessOrderColorDetail> processOrderColorDetail)
        {
            var result = ProcessOrderAdapter.InsertProcessOrderColorDetail(processOrderColorDetail);
            return result;
        }

        public int InsertProcessOrderFlow(List<ProcessOrderFlow> processOrderFlow)
        {
            var result = ProcessOrderAdapter.InsertProcessOrderFlow(processOrderFlow);
            return result;
        }

        public int UpdateProcessOrderFlowDate(List<ProcessOrderFlowFactoryName> processOrderFlow)
        {
            var result = ProcessOrderAdapter.UpdateProcessOrderFlowDate(processOrderFlow);
            return result;
        }

        /// <summary>
        /// 更新加工訂單顏色狀態
        /// </summary>
        /// <param name="orderColorDetailNo"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public int UpdateProcessOrderColorStatus(int orderColorDetailNo, ProcessOrderColorStatus status)
        {
            int result = ProcessOrderAdapter.UpdateProcessOrderFlowDate(orderColorDetailNo, status);
            return result;
        }
        /// <summary>
        /// 取得加工訂單流程明細
        /// </summary>
        /// <param name="OrderColorDetailNo"></param>
        /// <returns></returns>
        public IEnumerable<ProcessOrderFlowDateDetail> GetProcessOrderFlowDateDetail(List<int> OrderColorDetailNo)
        {
            IEnumerable<ProcessOrderFlowDateDetail> result = ProcessOrderAdapter.GetProcessOrderFlowDateDetail(OrderColorDetailNo);
            return result;
        }
        /// <summary>
        /// 新增加工訂單流程時間
        /// </summary>
        /// <param name="proecessOrderFlowDateList"></param>
        /// <returns></returns>
        public int InsertProcessOrderFlowDate(List<ProcessOrderFlowDate> proecessOrderFlowDateList)
        {
            int result = ProcessOrderAdapter.InsertProcessOrderFlowDate(proecessOrderFlowDateList);
            return result;
        }
        /// <summary>
        /// 更新加工訂單流程時間
        /// </summary>
        /// <param name="orderFlowDateNo"></param>
        /// <param name="orderColorDetailNoList"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public int UpdateProcessOrderFlowInputDate(int orderFlowNo, IEnumerable<int> orderColorDetailNoList, DateTime? date)
        {
            int result = ProcessOrderAdapter.UpdateProcessOrderFlowInputDate(orderFlowNo, orderColorDetailNoList, date);
            return result;
        }
        /// <summary>
        /// 更新加工訂單流程時間
        /// </summary>
        /// <param name="orderFlowNo"></param>
        /// <param name="orderColorDetailNoList"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public bool UpdateProcessOrderFlowCompleteDate(int orderFlowNo, IEnumerable<int> orderColorDetailNoList, DateTime? date)
        {
            int result = ProcessOrderAdapter.UpdateProcessOrderFlowCompleteDate(orderFlowNo, orderColorDetailNoList, date);
            return orderColorDetailNoList.Count() == result;
        }

        /// <summary>
        /// 依據狀態,工廠取得加工訂單
        /// </summary>
        /// <param name="factoryList"></param>
        /// <param name="statusList"></param>
        /// <returns></returns>
        public IEnumerable<ProcessOrder> GetProcessOrderFilter(List<Factory> factoryList, List<ProcessOrderColorStatus> statusList)
        {
            IEnumerable<int> factoryIDList = factoryList.Select(s => s.FactoryID);
            var result = ProcessOrderAdapter.GetProcessOrderFilter(factoryIDList, statusList).OrderByDescending(o => o.OrderNo);
            return result;
        }
        /// <summary>
        /// 取得加工訂單顏色明細
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public IEnumerable<ProcessOrderColorDetail> GetProcessOrderColorDetail(IEnumerable<int> processOrderColorDetailNoList)
        {
            IEnumerable<ProcessOrderColorDetail> processOrderColorDetails = ProcessOrderAdapter.GetProcessOrderColorDetail(processOrderColorDetailNoList);

            return processOrderColorDetails;
        }

        public void CreateProcessOrderColorFlow(List<ProcessOrderColorDetail> processOrderColorDetailList, int orderNo)
        {
            var processOrderColorDetailCount = InsertProcessOrderColorDetail(processOrderColorDetailList);
            var processOrderColorDetail = GetProcessOrderColorDetail(processOrderColorDetailCount);
            var processOrderFlow = GetProcessOrderFlow(orderNo);
            var proecessOrderFlowDateList = new List<ProcessOrderFlowDate>();
            foreach (var item in processOrderColorDetail)
            {
                foreach (var flowItem in processOrderFlow)
                {
                    proecessOrderFlowDateList.Add(new ProcessOrderFlowDate
                    {
                        OrderColorDetailNo = item.OrderColorDetailNo,
                        OrderFlowNo = flowItem.OrderDetailNo
                    });
                }
            }
            int count = InsertProcessOrderFlowDate(proecessOrderFlowDateList);
        }
        /// <summary>
        /// 修改加工訂單顏色明細
        /// </summary>
        /// <param name="orderColorDetailNo"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public int UpdateProcessOrderColorDetail(int orderColorDetailNo, int quantity)
        {
            int count = ProcessOrderAdapter.UpdateProcessOrderColorDetail(orderColorDetailNo, quantity);
            return count;
        }
        /// <summary>
        /// 刪除加工訂單顏色明細
        /// </summary>
        /// <param name="orderColorDetailNo"></param>
        /// <returns></returns>
        public int DeleteFactoryShippingDetail(int orderColorDetailNo)
        {
            int count = ProcessOrderAdapter.DeleteFactoryShippingDetail(orderColorDetailNo);
            return count;
        }
        /// <summary>
        /// 依據時間取得加工訂單明細
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public List<ProcessOrderStructure> GetNewOrEditProcessOrderStructures(DateTime dateTime, IEnumerable<int> orderNo)
        {

            IEnumerable<ProcessOrder> processOrders = ProcessOrderAdapter.GetNewOrEditProcessOrder(dateTime, orderNo);
            IEnumerable<ProcessOrderColorDetail> processOrderColorDetails = ProcessOrderAdapter.GetProcessOrderColorDetailList(processOrders.Select(s => s.OrderNo));
            IEnumerable<ProcessOrderFlowDateDetail> processOrderFlowDateDetails = ProcessOrderAdapter.GetProcessOrderFlowDateDetail(processOrderColorDetails.Select(s => s.OrderColorDetailNo).ToList());
            IEnumerable<FactoryShippingName> factoryShippingNames = ProcessOrderAdapter.GetFactoryShippingNameList(processOrderColorDetails.Select(s => s.OrderColorDetailNo));
            List<ProcessOrderStructure> processOrderStructures = new List<ProcessOrderStructure>();
            var processOrderStructure = processOrders
                .Select(s =>
                new ProcessOrderStructure
                {
                    ProcessOrder = s,
                    ProcessOrderColorGroups = processOrderColorDetails.Where(w => w.OrderNo == s.OrderNo)
                                                .Select(s1 =>
                                                new ProcessOrderColorGroup
                                                {
                                                    ProcessOrderColorDetail = s1,
                                                    ProcessOrderFlowDateDetails = processOrderFlowDateDetails.Where(w => w.OrderColorDetailNo == s1.OrderColorDetailNo),
                                                    FactoryShippings = factoryShippingNames.Where(w => w.OrderColorDetailNo == s1.OrderColorDetailNo)
                                                })
                });
            return processOrderStructure.ToList();
        }

        /// <summary>
        /// 更新加工訂單流程工廠名稱
        /// </summary>
        /// <param name="selectedFactoryItem"></param>
        /// <param name="orderFlowNo"></param>
        /// <returns></returns>
        public bool EditProcessOrderFlowFactory(int selectedFactoryItem, int orderFlowNo)
        {
            bool success = ProcessOrderAdapter.EditProcessOrderFlowFactory(selectedFactoryItem, orderFlowNo);
            return success;
        }
        /// <summary>
        /// 更新加工訂單備註
        /// </summary>
        /// <param name="processOrderNo"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool UpdateProcessOrderRemark(int processOrderNo, string text)
        {
            int count = ProcessOrderAdapter.UpdateProcessOrderRemark(processOrderNo, text);
            return count == 1;
        }
        /// <summary>
        /// 取得加工訂單備註
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public string GetProcessOrderRemark(int orderNo)
        {
            string remark = ProcessOrderAdapter.GetProcessOrderRemark(orderNo);
            return remark;
        }
        /// <summary>
        /// 取得已完成的顏色
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public IEnumerable<int> GetIsCompleteColor(int orderNo)
        {
            IEnumerable<int> result = ProcessOrderAdapter.GetIsCompleteColor(orderNo);
            return result;
        }
        /// <summary>
        /// 更新加工訂單顏色明細狀態為已完成
        /// </summary>
        /// <param name="orderFlowNo"></param>
        /// <param name="orderColorDetailNoList"></param>
        /// <returns></returns>
        public bool UpdateProcessOrderColorDetailStatusByLastComplete(int orderFlowNo, IEnumerable<int> orderColorDetailNoList)
        {
            int result = ProcessOrderAdapter.UpdateProcessOrderColorDetailStatusByLastComplete(orderFlowNo, orderColorDetailNoList);
            return result == orderColorDetailNoList.Count();
        }
        /// <summary>
        /// 新增加工訂單流程
        /// </summary>
        /// <param name="list"></param>
        /// <param name="orderColorDetailNo"></param>
        /// <returns></returns>
        public int NewProcessOrderFlow(ProcessOrderFlow processOrderFlow, IEnumerable<int> orderColorDetailNo)
        {
            int rsult = ProcessOrderAdapter.NewProcessOrderFlow(processOrderFlow, orderColorDetailNo);
            return rsult;
        }
    }
}
