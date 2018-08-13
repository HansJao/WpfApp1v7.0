using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.Entity;
using WpfApp1.Utility;

namespace WpfApp1.Adapter.MSSQL
{
    public class FabricAdapter : IFabricAdapter
    {
        /// <summary>
        /// 取得布種清單
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Fabric> GetFabricList()
        {
            string sqlCmd = "SELECT * FROM Fabric";
            var result = DapperHelper.QueryCollection<Fabric>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd);
            return result;
        }
        /// <summary>
        /// 新增布種
        /// </summary>
        /// <param name="fabric"></param>
        /// <returns></returns>
        public int AddFabric(Fabric fabric)
        {

            var sqlCmd = @"INSERT INTO [dbo].[Fabric]
                           VALUES
                           (@FabricName,@AverageUnitPrice,@AverageCost,@UpdateDate)";
            var result = DapperHelper.Execute(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, fabric);
            return result;

        }
        /// <summary>
        /// 更新布種
        /// </summary>
        /// <param name="fabric"></param>
        /// <returns></returns>
        public int EditFabric(Fabric fabric)
        {
            var sqlCmd = @"UPDATE [dbo].[Fabric]
                           SET [FabricName] = @FabricName
                           ,[AverageUnitPrice] = @AverageUnitPrice
                           ,[AverageCost] = @AverageCost
                           ,[UpdateDate] = @UpdateDate
                           WHERE FabricID=@FabricID";
            var result = DapperHelper.Execute(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, fabric);
            return result;
        }

    }
}
