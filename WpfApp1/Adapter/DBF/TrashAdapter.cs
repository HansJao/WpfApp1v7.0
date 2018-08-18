using System;
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
    }
}
