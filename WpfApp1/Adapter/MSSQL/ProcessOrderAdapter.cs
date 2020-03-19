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
using WpfApp1.DataClass.Entity.ProcessOrderFile;

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
            string sql = @"INSERT INTO ProcessOrder 
                           (OrderString,Fabric,Specification,ProcessItem,Precautions,Memo,HandFeel,CreateDate) 
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
                           (@OrderNo,@Color,@ColorNumber,@Quantity,@Status,@UpdateDate);";
            List<int> orderColorDetailNoList = new List<int>();

            foreach (var item in processOrderColorDetail)
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                        new SqlParameter("@OrderNo", SqlDbType.Int) { Value = item.OrderNo },
                        new SqlParameter("@Color", SqlDbType.NVarChar) { Value = item.Color },
                        new SqlParameter("@ColorNumber", SqlDbType.NVarChar) { Value = item.ColorNumber },
                        new SqlParameter("@Quantity", SqlDbType.NVarChar) { Value = item.Quantity },
                        new SqlParameter("@Status", SqlDbType.NVarChar) { Value = item.Status },
                        new SqlParameter("@UpdateDate", SqlDbType.DateTime) { Value = DateTime.Now }
                };
                var orderColorDetailNo = DapperHelper.Query<int>(AppSettingConfig.ConnectionString(), CommandType.Text, sql, parameters);
                orderColorDetailNoList.Add(orderColorDetailNo);
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
            var sqlCmd = @"select fs.ShippingNo,fs.OrderColorDetailNo,c.Name,fs.Quantity,fs.CreateDate,fs.ShippingDate from FactoryShipping fs
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
            string sql = @"INSERT INTO FactoryShipping
                            ([OrderColorDetailNo]
                            ,[CustomerID]
                            ,[Quantity]
                            ,[CreateDate]
                            ,[ShippingDate])
                           VALUES 
                           (@OrderColorDetailNo,@CustomerID,@Quantity,@CreateDate,@ShippingDate);";
            SqlParameter[] parameters = new SqlParameter[]
           {
                new SqlParameter("@OrderColorDetailNo", SqlDbType.Int) { Value = factoryShippingName.OrderColorDetailNo },
                new SqlParameter("@CustomerID", SqlDbType.Int) { Value = factoryShippingName.CustomerID },
                new SqlParameter("@Quantity", SqlDbType.Int) { Value = factoryShippingName.Quantity },
                new SqlParameter("@CreateDate", SqlDbType.DateTime) { Value = factoryShippingName.CreateDate },
                new SqlParameter("@ShippingDate", SqlDbType.DateTime) { Value = factoryShippingName.ShippingDate },
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
        public IEnumerable<ProcessOrderColorFactoryShippingDetail> GetProcessOrderColorFactoryShippingDetail(int orderNo)
        {
            string sql = @"select pocd.OrderColorDetailNo,pocd.OrderNo,pocd.Color,pocd.ColorNumber,pocd.Quantity,pocd.Status,SUM(fs.Quantity) as ShippingQuantity from ProcessOrderColorDetail pocd
                          left join FactoryShipping fs on pocd.OrderColorDetailNo = fs.OrderColorDetailNo 
                          where pocd.OrderNo = @OrderNo
                          group by pocd.OrderColorDetailNo,pocd.OrderNo,pocd.Color,pocd.ColorNumber,pocd.Quantity,pocd.Status ";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@OrderNo", SqlDbType.Int) { Value = orderNo }
            };
            return DapperHelper.QueryCollection<ProcessOrderColorFactoryShippingDetail>(AppSettingConfig.ConnectionString(), CommandType.Text, sql, parameters);
        }
        /// <summary>
        /// 依據狀態取得加工訂單
        /// </summary>
        /// <param name="Status"></param>
        /// <returns></returns>
        public IEnumerable<ProcessOrder> GetProcessOrderByStatus(List<ProcessOrderColorStatus> status)
        {
            var sqlCmd = @"select distinct po.OrderNo,po.OrderString,po.Fabric,po.Specification,po.ProcessItem,po.Precautions,po.Memo,po.HandFeel,po.CreateDate from ProcessOrder po
                          left join ProcessOrderColorDetail pocd on po.OrderNo = pocd.OrderNo
                          where Status in @Status";
            var parameter = (new { Status = status });

            var result = DapperHelper.QueryCollection<ProcessOrder, object>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameter);
            return result;
        }

        /// <summary>
        /// 取得加工訂單狀態
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ProcessOrderStatus> GetProcessOrderStatus()
        {
            string sql = @"SELECT PO.OrderString,PO.Fabric,POCD.Color,POCD.Quantity,POCD.Status,POCD.UpdateDate FROM ProcessOrderColorDetail POCD
                           INNER JOIN ProcessOrder PO ON PO.OrderNo = POCD.OrderNo
                           WHERE POCD.Status = 5 OR POCD.Status = 6 OR Status = 7 OR Status = 8 OR Status = 9";

            return DapperHelper.QueryCollection<ProcessOrderStatus>(AppSettingConfig.ConnectionString(), CommandType.Text, sql);
        }

        public int UpdateProcessOrderFlowDate(int orderColorDetailNo, ProcessOrderColorStatus status)
        {
            var sqlCmd = @"UPDATE ProcessOrderColorDetail
                          SET 
                          Status = @Status,
                          UpdateDate = GETDATE()
                          WHERE OrderColorDetailNo = @OrderColorDetailNo";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@OrderColorDetailNo", SqlDbType.Int) { Value = orderColorDetailNo },
                new SqlParameter("@Status", SqlDbType.Int) { Value = status }
            };
            var count = DapperHelper.ExecuteParameter(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameters);
            return count;
        }

        public IEnumerable<ProcessOrderFlowDateDetail> GetProcessOrderFlowDateDetail(List<int> orderColorDetailNo)
        {
            var sqlCmd = @"select pofd.OrderFlowDateNo,pofd.OrderColorDetailNo,pofd.OrderFlowNo,f.Name,pofd.InputDate,pofd.CompleteDate 
                          from ProcessOrderFlow pof
                          inner join ProcessOrderFlowDate pofd on pof.OrderDetailNo = pofd.OrderFlowNo
                          inner join Factory f on f.FactoryID = pof.FactoryID
                          where pofd.OrderColorDetailNo IN @OrderColorDetailNo";
            var parameter = (new { OrderColorDetailNo = orderColorDetailNo });

            var result = DapperHelper.QueryCollection<ProcessOrderFlowDateDetail, object>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameter);
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
        /// <param name="orderFlowNo"></param>
        /// <param name="orderColorDetailNoList"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public int UpdateProcessOrderFlowInputDate(int orderFlowNo, IEnumerable<int> orderColorDetailNoList, DateTime? date)
        {
            var sqlCmd = @"update ProcessOrderFlowDate
                          set InputDate = @InputDate,
                             UpdateDate = GETDATE()
                         where OrderFlowNo = @OrderFlowNo and OrderColorDetailNo in @OrderColorDetailNo";
            var parameter =
                new
                {
                    OrderColorDetailNo = orderColorDetailNoList,
                    InputDate = date,
                    OrderFlowNo = orderFlowNo
                };

            var count = DapperHelper.Execute(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameter);
            return count;
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
            var sqlCmd = @"UPDATE ProcessOrderFlowDate
                          SET CompleteDate = @CompleteDate,
                          UpdateDate = GETDATE()
                          WHERE OrderFlowNo = @OrderFlowNo AND OrderColorDetailNo IN @OrderColorDetailNo";
            var parameter =
                new
                {
                    OrderColorDetailNo = orderColorDetailNoList,
                    CompleteDate = date,
                    OrderFlowNo = orderFlowNo
                };
            var count = DapperHelper.Execute(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameter);
            return count;
        }

        /// <summary>
        /// 依據狀態,工廠取得加工訂單
        /// </summary>
        /// <param name="factoryList"></param>
        /// <param name="statusList"></param>
        /// <param name="containFinish"></param>
        /// <returns></returns>
        public IEnumerable<ProcessOrder> GetProcessOrderFilter(IEnumerable<int> factoryIDList, List<ProcessOrderColorStatus> statusList, bool containFinish)
        {
            var sqlCmd = @"SELECT DISTINCT PO.OrderNo,PO.OrderString,PO.Fabric,PO.Specification,PO.ProcessItem,PO.Precautions,PO.Memo,PO.HandFeel,PO.CreateDate,PO.Remark
                          FROM ProcessOrder PO
                          LEFT JOIN ProcessOrderFlow POF ON PO.OrderNo = POF.OrderNo
                          LEFT JOIN ProcessOrderColorDetail POCD ON PO.OrderNo = POCD.OrderNo
                          WHERE POCD.Status != @Status
                           {0}";

            sqlCmd = string.Format(sqlCmd, BuildProcessOrderFilterCommand(factoryIDList, statusList));
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Status", SqlDbType.Int) { Value = containFinish ? 0:1 }
            };
            var result = DapperHelper.QueryCollection<ProcessOrder>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameters);
            return result;
        }

        private string BuildProcessOrderFilterCommand(IEnumerable<int> factoryIDList, List<ProcessOrderColorStatus> statusList)
        {
            var sqlCmd = string.Empty;
            if (factoryIDList.Count() != 0)
            {
                sqlCmd = string.Concat("AND FactoryID IN (", string.Join(",", factoryIDList), ") ");
            }

            //if (string.IsNullOrEmpty(sqlCmd) && statusList.Count != 0)
            //{
            //    sqlCmd = string.Concat(sqlCmd, "WHERE Status IN (", string.Join(",", statusList.Select(s => Convert.ToInt32(s))), ") ");
            //}
            if (statusList.Count != 0)
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
        /// 修改加工訂單顏色明細數量
        /// </summary>
        /// <param name="orderColorDetailNo"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public int UpdateProcessOrderColorDetailQuantity(int orderColorDetailNo, int quantity)
        {
            var sqlCmd = @"UPDATE ProcessOrderColorDetail
                          SET Quantity = @Quantity,
                          UpdateDate = GETDATE()
                          WHERE OrderColorDetailNo = @OrderColorDetailNo";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Quantity", SqlDbType.Int) { Value = quantity },
                new SqlParameter("@OrderColorDetailNo", SqlDbType.Int) { Value = orderColorDetailNo }
            };
            var count = DapperHelper.ExecuteParameter(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameters);
            return count;
        }
        /// <summary>
        /// 修改加工訂單顏色明細顏色
        /// </summary>
        /// <param name="orderColorDetailNo"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public int UpdateProcessOrderColorDetailColor(int orderColorDetailNo, string color)
        {
            var sqlCmd = @"UPDATE ProcessOrderColorDetail
                          SET Color = @Color,
                          UpdateDate = GETDATE()
                          WHERE OrderColorDetailNo = @OrderColorDetailNo";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Color", SqlDbType.NVarChar) { Value = color },
                new SqlParameter("@OrderColorDetailNo", SqlDbType.Int) { Value = orderColorDetailNo }
            };
            var count = DapperHelper.ExecuteParameter(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameters);
            return count;
        }
        /// <summary>
        /// 修改加工訂單顏色明細色號
        /// </summary>
        /// <param name="orderColorDetailNo"></param>
        /// <param name="colorNumber"></param>
        /// <returns></returns>
        public int UpdateProcessOrderColorDetailColorNumber(int orderColorDetailNo, string colorNumber)
        {
            var sqlCmd = @"UPDATE ProcessOrderColorDetail
                          SET ColorNumber = @ColorNumber,
                          UpdateDate = GETDATE()
                          WHERE OrderColorDetailNo = @OrderColorDetailNo";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ColorNumber", SqlDbType.NVarChar) { Value = colorNumber },
                new SqlParameter("@OrderColorDetailNo", SqlDbType.Int) { Value = orderColorDetailNo }
            };
            var count = DapperHelper.ExecuteParameter(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameters);
            return count;
        }
        /// <summary>
        /// 刪除加工訂單顏色明細
        /// </summary>
        /// <param name="orderColorDetailNo"></param>
        /// <returns></returns>
        public int DeleteFactoryShippingDetail(int orderColorDetailNo)
        {
            string sql = @"delete from FactoryShipping where OrderColorDetailNo = @OrderColorDetailNo;
                           delete from ProcessOrderColorDetail where OrderColorDetailNo = @OrderColorDetailNo;
                           delete from ProcessOrderFlowDate where OrderColorDetailNo = @OrderColorDetailNo;";
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@OrderColorDetailNo", SqlDbType.Int) { Value = orderColorDetailNo }
            };
            var count = DapperHelper.ExecuteParameter(AppSettingConfig.ConnectionString(), CommandType.Text, sql, parameters);
            return count;
        }
        /// </summary>
        /// 取得新增或修改的加工訂單明細
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public IEnumerable<ProcessOrder> GetNewOrEditProcessOrder(DateTime dateTime, IEnumerable<int> orderNo)
        {
            var sqlCmd = @"SELECT DISTINCT PO.* FROM ProcessOrder PO 
                           LEFT JOIN ProcessOrderColorDetail POCD ON POCD.OrderNo = PO.OrderNo
                           LEFT JOIN ProcessOrderFlowDate POFD ON POFD.OrderColorDetailNo = POCD.OrderColorDetailNo
                           LEFT JOIN FactoryShipping FS ON FS.OrderColorDetailNo = POCD.OrderColorDetailNo
                           WHERE PO.CreateDate > @DATE OR POCD.UpdateDate > @DATE OR POFD.UpdateDate > @DATE OR FS.CreateDate > @DATE OR PO.OrderNo IN @OrderNo";
            var parameter = (new { DATE = dateTime, OrderNo = orderNo });

            var result = DapperHelper.QueryCollection<ProcessOrder, object>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameter);
            return result;
        }
        /// <summary>
        /// 依據多個訂單號取得加工訂單顏色明細清單
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public IEnumerable<ProcessOrderColorDetail> GetProcessOrderColorDetailList(IEnumerable<int> orderNo)
        {
            var sqlCmd = "SELECT * FROM ProcessOrderColorDetail WHERE OrderNo IN @OrderNo";
            var parameter = (new { OrderNo = orderNo });
            var result = DapperHelper.QueryCollection<ProcessOrderColorDetail, object>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameter);

            return result;
        }
        /// <summary>
        /// 依據多個加工訂單顏色明細編號號取得工廠直送清單
        /// </summary>
        /// <param name="processOrderColorDetailNo"></param>
        /// <returns></returns>
        public IEnumerable<FactoryShippingName> GetFactoryShippingNameList(IEnumerable<int> processOrderColorDetailNo)
        {
            var sqlCmd = @"select fs.ShippingNo,fs.OrderColorDetailNo,c.Name,fs.Quantity,fs.CreateDate,fs.ShippingDate from FactoryShipping fs
                           inner join Customer c on c.CustomerID = fs.CustomerID
                           where OrderColorDetailNo IN @OrderColorDetailNo";
            var parameter = (new { OrderColorDetailNo = processOrderColorDetailNo });
            return DapperHelper.QueryCollection<FactoryShippingName, object>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameter);
        }
        /// <summary>
        /// 更新加工訂單流程工廠名稱
        /// </summary>
        /// <param name="selectedFactoryID"></param>
        /// <param name="orderFlowNo"></param>
        /// <returns></returns>
        public bool EditProcessOrderFlowFactory(int selectedFactoryID, int orderFlowNo)
        {
            var sqlCmd = @"UPDATE ProcessOrderFlow
                          SET FactoryID = @FactoryID
                          WHERE OrderDetailNo = @OrderDetailNo";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@FactoryID", SqlDbType.Int) { Value = selectedFactoryID },
                new SqlParameter("@OrderDetailNo", SqlDbType.Int) { Value = orderFlowNo }
            };
            var count = DapperHelper.ExecuteParameter(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameters);
            return count == 1;
        }
        /// <summary>
        /// 更新加工訂單備註
        /// </summary>
        /// <param name="processOrderNo"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public int UpdateProcessOrderRemark(int processOrderNo, string text)
        {
            var sqlCmd = @"UPDATE ProcessOrder
                          SET Remark = @Remark
                          WHERE OrderNo = @OrderNo";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@OrderNo", SqlDbType.Int) { Value = processOrderNo },
                new SqlParameter("@Remark", SqlDbType.NVarChar) { Value = text }
            };
            var count = DapperHelper.ExecuteParameter(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameters);
            return count;
        }

        public int UpdateProcessOrderColorDetailStatusByLastComplete(int orderFlowNo, IEnumerable<int> orderColorDetailNoList)
        {
            var sqlCmd = @" DECLARE @lastOrderFlowNo INT

                            SELECT TOP 1 @lastOrderFlowNo = OrderFlowNo  FROM ProcessOrderFlowDate
                            WHERE OrderColorDetailNo = @OrderColorDetailNo
                            ORDER BY OrderFlowNo DESC

                            IF(@lastOrderFlowNo = @OrderFlowNo)
	                             begin
	                                UPDATE ProcessOrderColorDetail
                                    SET Status = 5,
                                    UpdateDate = GETDATE()
                                    WHERE OrderColorDetailNo IN @OrderColorDetailNoList
                                 end";
            var parameter =
                new
                {
                    OrderColorDetailNo = orderColorDetailNoList.First(),
                    OrderFlowNo = orderFlowNo,
                    OrderColorDetailNoList = orderColorDetailNoList
                };
            var count = DapperHelper.Execute(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameter);
            return count;
        }

        public int NewProcessOrderFlow(ProcessOrderFlow processOrderFlow, IEnumerable<int> orderColorDetailNo)
        {
            var sqlCmd = @"INSERT INTO ProcessOrderFlow
                           OUTPUT  inserted.OrderDetailNo
                           VALUES 
                           (@OrderNo,@FactoryID);";
            SqlParameter[] parameter = new SqlParameter[]
                {
                        new SqlParameter("@OrderNo", SqlDbType.Int) { Value = processOrderFlow.OrderNo },
                        new SqlParameter("@FactoryID", SqlDbType.Int) { Value = processOrderFlow.FactoryID }
                };
            var orderDetailNo = DapperHelper.Query<int>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameter);


            var cmd = @"INSERT INTO ProcessOrderFlowDate
                        VALUES(@OrderColorDetailNo,@OrderDetailNo,null,null,GETDATE());";
            var parameters =
                orderColorDetailNo.Select(s => new { OrderColorDetailNo = s, OrderDetailNo = orderDetailNo, });
            var count = DapperHelper.Execute(AppSettingConfig.ConnectionString(), CommandType.Text, cmd, parameters);
            return count;
        }


        /// <summary>
        /// 取得加工訂單依照工廠加工轉入轉出的更新時間排序
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public IEnumerable<ProcessOrder> GetProcessOrderByFactoryUpdateDate(string dateTime)
        {
            var sqlCmd = @"SELECT DISTINCT PO.* FROM ProcessOrder PO
                           INNER JOIN ProcessOrderColorDetail POCD ON POCD.OrderNo = PO.OrderNo
                           INNER JOIN ProcessOrderFlowDate POF ON POF.OrderColorDetailNo = POCD.OrderColorDetailNo
                           WHERE POF.UpdateDate BETWEEN @UpdateDate AND GETDATE()";
            SqlParameter[] parameter = new SqlParameter[]
                {
                        new SqlParameter("@UpdateDate", SqlDbType.DateTime) { Value = dateTime },
                };
            var result = DapperHelper.QueryCollection<ProcessOrder>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameter);
            return result;
        }
        /// <summary>
        /// 依據顏色取得加工訂單
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public IEnumerable<ProcessOrder> GetProcessOrderByColor(string color, bool containFinish)
        {
            var sqlCmd = @"SELECT DISTINCT PO.* FROM ProcessOrderColorDetail POCD
                           INNER JOIN ProcessOrder PO ON POCD.OrderNo = PO.OrderNo
                           WHERE POCD.Color LIKE @Color AND POCD.Status != @Status
                           ORDER BY PO.OrderNo";
            SqlParameter[] parameter = new SqlParameter[]
            {
                new SqlParameter("@Color", SqlDbType.NVarChar) { Value = "%" + color + "%" },
                new SqlParameter("@Status", SqlDbType.Int) { Value = containFinish ? 0 : 1 }
            };
            var result = DapperHelper.QueryCollection<ProcessOrder>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameter);
            return result;
        }
        /// <summary>
        /// 新增客戶訂單關連
        /// </summary>
        /// <param name="customerOrderRelate"></param>
        /// <returns></returns>
        public int InsertCustomerOrderRelate(CustomerOrderRelate customerOrderRelate)
        {
            var sqlCmd = @"INSERT INTO CustomerOrderRelate
                           (CustomerID,ProcessOrderID) 
                           VALUES 
                           (@CustomerID,@ProcessOrderID)";
            SqlParameter[] parameter = new SqlParameter[]
            {
                new SqlParameter("@CustomerID", SqlDbType.Int) { Value = customerOrderRelate.CustomerID },
                new SqlParameter("@ProcessOrderID", SqlDbType.Int) { Value = customerOrderRelate.ProcessOrderID }
            };
            return DapperHelper.ExecuteParameter(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameter);
        }
        /// <summary>
        /// 以客戶編號取得加工訂單
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public IEnumerable<ProcessOrder> GetProcessOrderByCustomer(int customerID)
        {
            var sqlCmd = @"SELECT DISTINCT PO.* FROM ProcessOrder PO
                           INNER JOIN CustomerOrderRelate COR ON COR.ProcessOrderID = PO.OrderNo
                           WHERE COR.CustomerID = @CustomerID";
            SqlParameter[] parameter = new SqlParameter[]
            {
                new SqlParameter("@CustomerID", SqlDbType.Int) {Value = customerID},
            };
            var result = DapperHelper.QueryCollection<ProcessOrder>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameter);
            return result;
        }

        /// <summary>
        /// 依據訂單編號取得顧客資料
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public IEnumerable<ProcessOrderCustomerRelate> GetCustomerByOrderNo(int orderNo)
        {
            var sqlCmd = @"SELECT COR.CustomerOrderID,C.Name FROM Customer C
                           INNER JOIN CustomerOrderRelate COR ON COR.CustomerID = C.CustomerID 
                           WHERE COR.ProcessOrderID = @ProcessOrderID";
            SqlParameter[] parameter = new SqlParameter[]
            {
                new SqlParameter("@ProcessOrderID", SqlDbType.Int) {Value = orderNo},
            };
            var result = DapperHelper.QueryCollection<ProcessOrderCustomerRelate>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameter);
            return result;
        }

        /// <summary>
        /// 刪除客戶訂單關連
        /// </summary>
        /// <param name="customerOrderID"></param>
        /// <returns></returns>
        public int DeleteCustomerOrderRelate(int customerOrderID)
        {
            string sql = @"DELETE FROM CustomerOrderRelate
                           WHERE CustomerOrderID = @CustomerOrderID";
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@CustomerOrderID", SqlDbType.Int) { Value = customerOrderID }
            };
            var count = DapperHelper.ExecuteParameter(AppSettingConfig.ConnectionString(), CommandType.Text, sql, parameters);
            return count;
        }
        /// <summary>
        /// 檢查是否已存在於客戶關連
        /// </summary>
        /// <param name="orderNo"></param>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public int CheckInCustomerOrderRelate(int orderNo, int customerID)
        {
            var sqlCmd = @"SELECT CustomerOrderID FROM CustomerOrderRelate
                           WHERE ProcessOrderID = @ProcessOrderID AND CustomerID = @CustomerID";
            SqlParameter[] parameter = new SqlParameter[]
            {
                new SqlParameter("@ProcessOrderID", SqlDbType.Int) {Value = orderNo},
                new SqlParameter("@CustomerID", SqlDbType.Int) {Value = customerID},
            };
            var result = DapperHelper.QueryCollection<int>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameter);
            return result.Count();
        }
    }
}
