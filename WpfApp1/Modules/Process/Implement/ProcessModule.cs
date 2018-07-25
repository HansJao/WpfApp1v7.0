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

        public IEnumerable<ProcessFactoryShippingDetail> GetProcessFactoryShippingDetail(int orderNo)
        {
            var result = ProcessOrderAdapter.GetProcessFactoryShippingDetail(orderNo);
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
        public IEnumerable<ProcessOrderFlowDateDetail> GetProcessOrderFlowDateDetail(int OrderColorDetailNo)
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
        public int UpdateProcessOrderFlowCompleteDate(int orderFlowNo, IEnumerable<int> orderColorDetailNoList, DateTime? date)
        {
            int result = ProcessOrderAdapter.UpdateProcessOrderFlowCompleteDate(orderFlowNo, orderColorDetailNoList, date);
            return result;
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

        public void CreateProcessOrderColorFlow(List<ProcessOrderColorDetail> processOrderColorDetailList,int orderNo)
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
    }
}
