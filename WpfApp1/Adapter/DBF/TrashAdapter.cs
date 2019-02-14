﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.TrashSystem;
using WpfApp1.Utility;

namespace WpfApp1.Adapter.DBF
{
    public class TrashAdapter : ITrashAdapter
    {
        public IEnumerable<TrashItem> GetTrashItems()
        {
            var sqlCmd = "SELECT * FROM ITEM.dbf";
            var result = DapperHelper.QueryDbfCollection<TrashItem>(AppSettingConfig.DbfConnectionString(), CommandType.Text, sqlCmd);
            return result;
        }

        public IEnumerable<TrashItem> GetTrashItemsByFeature(string feature)
        {
            var sqlCmd = "SELECT * FROM ITEM.dbf WHERE I_03 like @I_03";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@I_03", SqlDbType.NVarChar) { Value = feature }
            };
            var result = DapperHelper.QueryDbfCollection<TrashItem>(AppSettingConfig.DbfConnectionString(), CommandType.Text, sqlCmd, parameters);
            return result;
        }

        public IEnumerable<TrashShipped> GetTrashShippedList(DateTime datePickerBegin, DateTime datePickerEnd)
        {
            string sqlCmd = "SELECT invosub.IN_DATE,invosub.I_01,item.I_03,SUM(invosub.QUANTITY) as Quantity FROM INVOSUB.dbf invosub " +
                           "INNER JOIN ITEM.dbf item ON invosub.I_01 = item.I_01 AND invosub.F_01 = item.F_01 " +
                           "WHERE invosub.IN_DATE Between cDate('" + datePickerBegin.ToString() + "') and cDate('" + datePickerEnd.ToString() + "') " +
                           "GROUP BY invosub.IN_DATE,invosub.I_01,item.I_03 " +
                           "ORDER BY invosub.IN_DATE,invosub.I_01";
            var result = DapperHelper.QueryDbfCollection<TrashShipped>(AppSettingConfig.DbfConnectionString(), CommandType.Text, sqlCmd);
            return result;
        }
        //var sqlCmd = @"update ProcessOrderFlowDate
        //                  set InputDate = @InputDate,
        //                     UpdateDate = GETDATE()
        //                 where OrderFlowNo = @OrderFlowNo and OrderColorDetailNo in @OrderColorDetailNo";
        //var parameter =
        //    new
        //    {
        //        OrderColorDetailNo = orderColorDetailNoList,
        //        InputDate = date,
        //        OrderFlowNo = orderFlowNo
        //    };
        public void UpdateProductName(string v)
        {
            string sqlCmd = @"SELECT * from ITEM.dbf WHERE I_03 Like '%*%'";//@"UPDATE ITEM.dbf SET I_03=REPLACE(I_03,'*','X') WHERE I_03 LIKE '%*%'";

            var result = DapperHelper.QueryDbfCollection<TrashItem>(AppSettingConfig.DbfConnectionString(), CommandType.Text, sqlCmd);

            var replace = result.Select(s => new TrashItem { I_03 = s.I_03.Replace("*", "X"), F_01 = s.F_01, I_01 = s.I_01 });
            foreach (var item in replace)
            {
                string sqlCmd2 = @"UPDATE ITEM.dbf SET I_03=@I_03 WHERE F_01=@F_01 AND I_01=@I_01";
                SqlParameter[] parameters = new SqlParameter[]
                   {
                        new SqlParameter("@I_03", SqlDbType.NVarChar) { Value = item.I_03 },
                        new SqlParameter("@F_01", SqlDbType.NVarChar) { Value = item.F_01 },
                        new SqlParameter("@I_01", SqlDbType.NVarChar) { Value = item.I_01 },
                   };
                var count = DapperHelper.QueryDbfCollection<int>(AppSettingConfig.DbfConnectionString(), CommandType.Text, sqlCmd2, parameters);
            }
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
            string sqlCmd = @"SELECT cust.C_Name,invosub.IN_DATE,invosub.I_01,item.I_03,invosub.QUANTITY FROM (CUST.dbf AS cust 
                            INNER JOIN INVOSUB.dbf AS invosub ON cust.CARD_NO = invosub.C_01)
                            INNER JOIN ITEM.dbf AS item ON invosub.I_01 = ITEM.I_01 
                            WHERE cust.C_Name = @CustomerName AND invosub.IN_DATE Between cDate('" + customerDatePickerBegin.ToString() + "') and cDate('" + customerDatePickerEnd.ToString() + "')";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@CustomerName", SqlDbType.NVarChar) { Value = customerName },
            };
            var result = DapperHelper.QueryDbfCollection<TrashCustomerShipped>(AppSettingConfig.DbfConnectionString(), CommandType.Text, sqlCmd, parameters);
            return result;
        }

    }
}
