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
    }
}
