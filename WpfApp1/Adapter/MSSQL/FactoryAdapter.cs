using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.Entity;
using WpfApp1.DataClass.FactoryClass;
using WpfApp1.Utility;

namespace WpfApp1.Adapter.MSSQL
{
    public class FactoryAdapter : IFacroryAdapter
    {
        /// <summary>
        /// 取得工廠編號
        /// </summary>
        /// <param name="FactoryName"></param>
        /// <returns></returns>
        public IEnumerable<FactoryIdentity> GetFactoryIdentiys(string[] FactoryNameString)
        {
            var sqlCmd = "SELECT FactoryID,Name FROM Factory WHERE Name IN @Name";
            var parameter = (new { Name = FactoryNameString });
            //var y = (new { Name = new[] { "強金利", "祥玉" } });
            var result = DapperHelper.QueryCollection<FactoryIdentity, object>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameter);

            return result;
        }

        public IEnumerable<Factory> GetFactoryList()
        {
            var sqlCmd = "SELECT * FROM Factory";
            var result = DapperHelper.QueryCollection<Factory>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd);

            return result;
        }

        /// <summary>
        /// 新增工廠清單
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        public int InsertFactory(Factory factory)
        {
            var sqlCmd = @"INSERT INTO [dbo].[Factory]
                           VALUES
                           (@Name,@PhoneNumber,@CellPhone,@Fax,@Process,@Address,@Sort)";
            var result = DapperHelper.Execute(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, factory);
            return result;
        }
        /// <summary>
        /// 修改工廠明細
        /// </summary>
        /// <returns></returns>
        public int UpdateFactory(Factory factory)
        {
            var sqlCmd = @"UPDATE [dbo].[Factory]
                           SET [PhoneNumber] = @PhoneNumber
                           ,[CellPhone] = @CellPhone
                           ,[Fax] = @Fax
                           ,[Address] = @Address
                           ,[Sort] = @Sort
                           WHERE FactoryID=@FactoryID";
            var result = DapperHelper.Execute(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, factory);
            return result;
        }
    }
}
