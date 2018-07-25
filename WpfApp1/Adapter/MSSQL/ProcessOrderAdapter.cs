using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.Entity;
using WpfApp1.DataClass.Enumeration;
using WpfApp1.DataClass.ProcessOrder;
using WpfApp1.Modules.Process;
using WpfApp1.Utility;
using System.Transactions;

namespace WpfApp1.Adapter.MSSQL
{
    public class ProcessOrderAdapter : IProcessOrderAdapter
    {
        /// <summary>
        /// 取得商品所屬分類
        /// </summary>
        /// <param name="goodID">商品品號</param>
        /// <returns>商品所屬分類清單</returns>
        public IEnumerable<ProcessOrder> GetProcessOrder()
        {
            var sqlCmd = "select * from ProcessOrder";
            return DapperHelper.QueryCollection<ProcessOrder>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd);
        }
        public IEnumerable<ProcessOrderFlowFactoryName> GetProcessOrderFlow(int orderNo)
        {
            var sqlCmd = @"select OrderDetailNo,OrderNo,Name as Factory from [ProcessOrderFlow] pod
                          inner join Factory f on f.FactoryID = pod.FactoryID
                          where OrderNo = @OrderNo";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@OrderNo", SqlDbType.Int) { Value = orderNo }
            };
            return DapperHelper.QueryCollection<ProcessOrderFlowFactoryName>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameters);
        }

        public IEnumerable<ProcessOrderColorDetail> GetProcessOrderColorDetailByOrderNo(int orderNo)
        {
            var sqlCmd = "select * from ProcessOrderColorDetail WHERE OrderNo = @OrderNo";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@OrderNo", SqlDbType.Int) { Value = orderNo }
            };
            return DapperHelper.QueryCollection<ProcessOrderColorDetail>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameters);

        }

        public int InsertProcessOrder(ProcessOrder processOrder)
        {
            string sql = @"INSERT INTO ProcessOrder (OrderString,Fabric,Specification,ProcessItem,Precautions,Memo,HandFeel,CreateDate) 
                           VALUES 
                            (@OrderString,@Fabric,@Specification,@ProcessItem,@Precautions,@Memo,@HandFeel,@CreateDate);
                            SELECT CAST(SCOPE_IDENTITY() as int)";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@OrderString", SqlDbType.NVarChar) { Value = processOrder.OrderString },
                new SqlParameter("@Fabric", SqlDbType.NVarChar) { Value = processOrder.Fabric },
                new SqlParameter("@Specification", SqlDbType.NVarChar) { Value = processOrder.Specification },
                new SqlParameter("@ProcessItem", SqlDbType.NVarChar) { Value = processOrder.ProcessItem },
                new SqlParameter("@Precautions", SqlDbType.NVarChar) { Value = processOrder.Precautions },
                new SqlParameter("@Memo", SqlDbType.NVarChar) { Value = processOrder.Memo },
                new SqlParameter("@HandFeel", SqlDbType.NVarChar) { Value = processOrder.HandFeel },
                new SqlParameter("@CreateDate", SqlDbType.DateTime) { Value = processOrder.CreateDate },
            };
            return DapperHelper.Query<int>(AppSettingConfig.ConnectionString(), CommandType.Text, sql, parameters);
        }

        public int InsertProcessOrderFlow(List<ProcessOrderFlow> processOrderFlow)
        {
            string sql = @"INSERT INTO ProcessOrderFlow
                           VALUES 
                           (@OrderNo,@FactoryID);";
            var result = DapperHelper.Execute(AppSettingConfig.ConnectionString(), CommandType.Text, sql, processOrderFlow);
            return result;
        }
        /// <summary>
        /// 新增加工訂單顏色明細
        /// </summary>
        /// <param name="processOrderColorDetail"></param>
        /// <returns>取得加工訂單顏色明細id清單</returns>
        public IEnumerable<int> InsertProcessOrderColorDetail(List<ProcessOrderColorDetail> processOrderColorDetail)
        {
            string sql = @"INSERT INTO ProcessOrderColorDetail
                           output inserted.OrderColorDetailNo
                           VALUES 
                           (@OrderNo,@Color,@ColorNumber,@Quantity,@Status);";
            List<int> orderColorDetailNoList = new List<int>();
            using (var scope = new TransactionScope())
            {
                foreach (var item in processOrderColorDetail)
                {
                    SqlParameter[] parameters = new SqlParameter[]
                    {
                        new SqlParameter("@OrderNo", SqlDbType.Int) { Value = item.OrderNo },
                        new SqlParameter("@Color", SqlDbType.NVarChar) { Value = item.Color },
                        new SqlParameter("@ColorNumber", SqlDbType.NVarChar) { Value = item.ColorNumber },
                        new SqlParameter("@Quantity", SqlDbType.NVarChar) { Value = item.Quantity },
                        new SqlParameter("@Status", SqlDbType.NVarChar) { Value = item.Status }
                    };
                    var orderColorDetailNo = DapperHelper.Query<int>(AppSettingConfig.ConnectionString(), CommandType.Text, sql, parameters);
                    orderColorDetailNoList.Add(orderColorDetailNo);
                }
                scope.Complete();
            }
            return orderColorDetailNoList;//DapperHelper.Execute(AppSettingConfig.ConnectionString(), CommandType.Text, sql, processOrderColorDetail);
        }

        public int DeleteProcessOrder(ProcessOrder processOrder)
        {
            string sql = @"delete from ProcessOrder where OrderNo = @OrderNo;
                           delete from ProcessOrderFlow where OrderNo = @OrderNo;
                           declare @ColorDetailNoList Table(OrderColorDetailNo int);
                           insert into @ColorDetailNoList
                           select OrderColorDetailNo from ProcessOrderColorDetail where OrderNo = @OrderNo;
                           delete from FactoryShipping where OrderColorDetailNo in (select OrderColorDetailNo from @ColorDetailNoList);
                           delete from ProcessOrderFlowDate where OrderColorDetailNo in (select OrderColorDetailNo from @ColorDetailNoList);
                           delete from ProcessOrderColorDetail where OrderNo = @OrderNo;";
            var count = DapperHelper.Execute(AppSettingConfig.ConnectionString(), CommandType.Text, sql, processOrder);
            return count;
        }

        public int UpdateProcessOrderFlowDate(List<ProcessOrderFlowFactoryName> processOrderFlow)
        {
            string sql = @"UPDATE ProcessOrderFlow SET InputDate = @InputDate,CompleteDate = @CompleteDate WHERE OrderDetailNo = @OrderDetailNo;";
            var count = DapperHelper.Execute(AppSettingConfig.ConnectionString(), CommandType.Text, sql, processOrderFlow);
            return count;
        }

        public IEnumerable<FactoryShippingName> GetFactoryShipping(int orderColorDetailNo)
        {
            var sqlCmd = @"select fs.ShippingNo,fs.OrderColorDetailNo,c.Name,fs.Quantity,fs.CreateDate from FactoryShipping fs
                           inner join Customer c on c.CustomerID = fs.CustomerID
                           where OrderColorDetailNo = @OrderColorDetailNo";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@OrderColorDetailNo", SqlDbType.Int) { Value = orderColorDetailNo }
            };
            return DapperHelper.QueryCollection<FactoryShippingName>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameters);
        }

        public int InsertFactoryShipping(FactoryShippingName factoryShippingName)
        {
            string sql = @"declare @CustomerID int
                           select @CustomerID = CustomerID from Customer
                           where Name = @Customer
                           INSERT INTO FactoryShipping
                           VALUES 
                           (@OrderColorDetailNo,@CustomerID,@Quantity,@CreateDate);";
            SqlParameter[] parameters = new SqlParameter[]
           {
                new SqlParameter("@OrderColorDetailNo", SqlDbType.Int) { Value = factoryShippingName.OrderColorDetailNo },
                new SqlParameter("@Customer", SqlDbType.NVarChar) { Value = factoryShippingName.Name },
                new SqlParameter("@Quantity", SqlDbType.Int) { Value = factoryShippingName.Quantity },
                new SqlParameter("@CreateDate", SqlDbType.DateTime) { Value = factoryShippingName.CreateDate },
           };
            return DapperHelper.ExecuteParameter(AppSettingConfig.ConnectionString(), CommandType.Text, sql, parameters);
        }

        public int DeleteFactoryShipping(int shippingNo)
        {
            string sql = @"delete from FactoryShipping where ShippingNo = @ShippingNo";
            SqlParameter[] parameters = new SqlParameter[]
           {
                new SqlParameter("@ShippingNo", SqlDbType.Int) { Value = shippingNo }
           };
            var count = DapperHelper.ExecuteParameter(AppSettingConfig.ConnectionString(), CommandType.Text, sql, parameters);
            return count;
        }

        /// <summary>
        /// 取得加工廠出貨明細
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public IEnumerable<ProcessFactoryShippingDetail> GetProcessFactoryShippingDetail(int orderNo)
        {
            string sql = @"select pocd.OrderColorDetailNo,pocd.OrderNo,pocd.Color,pocd.ColorNumber,pocd.Quantity,pocd.Status,SUM(fs.Quantity) as ShippingQuantity from ProcessOrderColorDetail pocd
                          left join FactoryShipping fs on pocd.OrderColorDetailNo = fs.OrderColorDetailNo 
                          where pocd.OrderNo = @OrderNo
                          group by pocd.OrderColorDetailNo,pocd.OrderNo,pocd.Color,pocd.ColorNumber,pocd.Quantity,pocd.Status ";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@OrderNo", SqlDbType.Int) { Value = orderNo }
            };
            return DapperHelper.QueryCollection<ProcessFactoryShippingDetail>(AppSettingConfig.ConnectionString(), CommandType.Text, sql, parameters);
        }
        /// <summary>
        /// 依據狀態取得加工訂單
        /// </summary>
        /// <param name="Status"></param>
        /// <returns></returns>
        public IEnumerable<ProcessOrder> GetProcessOrderByStatus(ProcessOrderColorStatus status)
        {
            var sqlCmd = @"select distinct po.OrderNo,po.OrderString,po.Fabric,po.Specification,po.ProcessItem,po.Precautions,po.Memo,po.HandFeel,po.CreateDate from ProcessOrder po
                          left join ProcessOrderColorDetail pocd on po.OrderNo = pocd.OrderNo
                          where Status = @Status";
            SqlParameter[] parameters = new SqlParameter[]
          {
                new SqlParameter("@Status", SqlDbType.Int) { Value = status }
          };
            var result = DapperHelper.QueryCollection<ProcessOrder>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameters);

            return result;
        }

        public int UpdateProcessOrderFlowDate(int orderColorDetailNo, ProcessOrderColorStatus status)
        {
            var sqlCmd = @"update ProcessOrderColorDetail
                          set Status = @Status
                          where OrderColorDetailNo = @OrderColorDetailNo";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@OrderColorDetailNo", SqlDbType.Int) { Value = orderColorDetailNo },
                new SqlParameter("@Status", SqlDbType.Int) { Value = status }
            };
            var count = DapperHelper.ExecuteParameter(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameters);
            return count;
        }

        public IEnumerable<ProcessOrderFlowDateDetail> GetProcessOrderFlowDateDetail(int orderColorDetailNo)
        {
            var sqlCmd = @"select pofd.OrderFlowDateNo,pofd.OrderColorDetailNo,f.Name,pofd.InputDate,pofd.CompleteDate 
                          from ProcessOrderFlow pof
                          inner join ProcessOrderFlowDate pofd on pof.OrderDetailNo = pofd.OrderFlowNo
                          inner join Factory f on f.FactoryID = pof.FactoryID
                          where pofd.OrderColorDetailNo=@OrderColorDetailNo";
            SqlParameter[] parameters = new SqlParameter[]
           {
                new SqlParameter("@OrderColorDetailNo", SqlDbType.Int) { Value = orderColorDetailNo }
           };
            var result = DapperHelper.QueryCollection<ProcessOrderFlowDateDetail>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameters);
            return result;
        }

        public int InsertProcessOrderFlowDate(List<ProcessOrderFlowDate> proecessOrderFlowDateList)
        {
            string sql = @"INSERT INTO ProcessOrderFlowDate
                           VALUES 
                           (@OrderColorDetailNo,@OrderFlowNo,@InputDate,@CompleteDate,@UpdateDate);";
            return DapperHelper.Execute(AppSettingConfig.ConnectionString(), CommandType.Text, sql, proecessOrderFlowDateList);
        }
        /// <summary>
        /// 更新加工訂單流程時間
        /// </summary>
        /// <param name="orderFlowDateNo"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public int UpdateProcessOrderFlowInputDate(int orderFlowDateNo, DateTime? date)
        {
            var sqlCmd = @"update ProcessOrderFlowDate
                          set InputDate = @InputDate,
                             UpdateDate = GETDATE()
                          where OrderFlowDateNo = @OrderFlowDateNo";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@OrderFlowDateNo", SqlDbType.Int) { Value = orderFlowDateNo },
                new SqlParameter("@InputDate", SqlDbType.DateTime) { Value = date }
            };
            var count = DapperHelper.ExecuteParameter(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameters);
            return count;
        }

        public int UpdateProcessOrderFlowCompleteDate(int orderFlowDateNo, DateTime? date)
        {
            var sqlCmd = @"update ProcessOrderFlowDate
                          set CompleteDate = @CompleteDate,
                             UpdateDate = GETDATE()
                          where OrderFlowDateNo = @OrderFlowDateNo";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@OrderFlowDateNo", SqlDbType.Int) { Value = orderFlowDateNo },
                new SqlParameter("@CompleteDate", SqlDbType.DateTime) { Value = date }
            };
            var count = DapperHelper.ExecuteParameter(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameters);
            return count;
        }

        public IEnumerable<ProcessOrder> GetProcessOrderFilter(IEnumerable<int> factoryIDList, List<ProcessOrderColorStatus> statusList)
        {
            var sqlCmd = @"select distinct po.OrderNo,po.OrderString,po.Fabric,po.Specification,po.ProcessItem,po.Precautions,po.Memo,po.HandFeel,po.CreateDate 
                          from ProcessOrder po
                          left join ProcessOrderFlow pof on po.OrderNo = pof.OrderNo
                          left join ProcessOrderColorDetail pocd on po.OrderNo = pocd.OrderNo
                           {0}";

            sqlCmd = string.Format(sqlCmd, BuildProcessOrderFilterCommand(factoryIDList, statusList));
            var result = DapperHelper.QueryCollection<ProcessOrder>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd);
            return result;
        }

        private string BuildProcessOrderFilterCommand(IEnumerable<int> factoryIDList, List<ProcessOrderColorStatus> statusList)
        {
            var sqlCmd = string.Empty;
            if (factoryIDList.Count() != 0)
            {
                sqlCmd = string.Concat("WHERE FactoryID IN (", string.Join(",", factoryIDList), ") ");
            }

            if (string.IsNullOrEmpty(sqlCmd) && statusList.Count != 0)
            {
                sqlCmd = string.Concat(sqlCmd, "WHERE Status IN (", string.Join(",", statusList.Select(s => Convert.ToInt32(s)), ") "));
            }
            else if (statusList.Count != 0)
            {
                sqlCmd = string.Concat(sqlCmd, "AND Status IN (", string.Join(",", statusList.Select(s => Convert.ToInt32(s))), ") ");
            }

            return sqlCmd;
        }
        /// <summary>
        /// 取得加工訂單顏色明細
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public IEnumerable<ProcessOrderColorDetail> GetProcessOrderColorDetail(IEnumerable<int> processOrderColorDetailNoList)
        {
            var sqlCmd = "SELECT * FROM ProcessOrderColorDetail WHERE OrderColorDetailNo IN @OrderColorDetailNo";
            var parameter = (new { OrderColorDetailNo = processOrderColorDetailNoList });
            var result = DapperHelper.QueryCollection<ProcessOrderColorDetail, object>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameter);

            return result;
        }
        /// <summary>
        /// 修改加工訂單顏色明細
        /// </summary>
        /// <param name="orderColorDetailNo"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public int UpdateProcessOrderColorDetail(int orderColorDetailNo, int quantity)
        {
            var sqlCmd = @"update ProcessOrderColorDetail
                          set Quantity = @Quantity
                          where OrderColorDetailNo = @OrderColorDetailNo";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Quantity", SqlDbType.Int) { Value = quantity },
                new SqlParameter("@OrderColorDetailNo", SqlDbType.Int) { Value = orderColorDetailNo }
            };
            var count = DapperHelper.ExecuteParameter(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameters);
            return count;
        }
    }
}
