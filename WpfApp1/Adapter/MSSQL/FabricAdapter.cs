using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.Entity;
using WpfApp1.DataClass.Fabric;
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
        /// <summary>
        /// 以布種編號取得布種顏色
        /// </summary>
        /// <param name="fabricIDList"></param>
        /// <returns></returns>
        public IEnumerable<FabricColor> GetFabricColorListByFabricID(IEnumerable<int> fabricIDList)
        {
            string sqlCmd = @"SELECT * FROM FabricColor WHERE FabricID IN @FabricID";
            var parameter =
               new
               {
                   FabricID = fabricIDList
               };
            var result = DapperHelper.QueryCollection<FabricColor, object>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameter);
            return result;

        }
        /// <summary>
        /// 新增布種顏色
        /// </summary>
        /// <param name="fabricColors"></param>
        public int AddFabricColorList(List<FabricColor> fabricColors)
        {
            string sqlCmd = @"INSERT INTO FabricColor
                            (FabricID,Color)
                           VALUES 
                           (@FabricID,@Color);";

            int count = DapperHelper.Execute(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, fabricColors);
            return count;
        }
        /// <summary>
        /// 以布種顏色編號取得顏色比例
        /// </summary>
        /// <param name="fabricColorNoList"></param>
        /// <returns></returns>
        public IEnumerable<FabricProportion> GetFabricProportionByColorNo(List<int> fabricColorNoList)
        {
            string sqlCmd = @"SELECT * FROM FabricProportion WHERE ColorNo IN @ColorNo";
            var parameter =
               new
               {
                   ColorNo = fabricColorNoList
               };
            var result = DapperHelper.QueryCollection<FabricProportion, object>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameter);
            return result;
        }
        /// <summary>
        /// 以布種顏色編號取得布種顏色比例
        /// </summary>
        /// <param name="fabricColorNoList"></param>
        /// <returns></returns>
        public IEnumerable<FabricIngredientProportion> GetFabricIngredientProportionByColorNo(List<int> fabricColorNoList)
        {

            string sqlCmd = @"SELECT FP.ProportionNo,F.Name,FP.YarnPriceNo,YP.Ingredient,YP.Color,YP.YarnCount,YP.Price,FP.Proportion,FP.[Group] FROM FabricProportion FP
                              INNER JOIN YarnPrice YP ON FP.YarnPriceNo = YP.YarnPriceNo
							  INNER JOIN Factory F ON YP.YarnMerchant = F.FactoryID
                              WHERE ColorNo IN @ColorNo";
            var parameter =
               new
               {
                   ColorNo = fabricColorNoList
               };
            var result = DapperHelper.QueryCollection<FabricIngredientProportion, object>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameter);
            return result;
        }
        /// <summary>
        /// 以布種編號取得加工順序
        /// </summary>
        /// <param name="fabricIDList"></param>
        /// <returns></returns>
        public IEnumerable<ProcessSequence> GetProcessSequences(IEnumerable<int> fabricIDList)
        {
            string sqlCmd = @"SELECT * FROM ProcessSequence WHERE FabricID IN @FabricID";
            var parameter =
               new
               {
                   FabricID = fabricIDList
               };
            var result = DapperHelper.QueryCollection<ProcessSequence, object>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameter);
            return result;
        }
        /// <summary>
        /// 取得所有紗商的紗價
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MerchantYarnPrice> GetMerchantYarnPriceList()
        {
            string sqlCmd = @"SELECT YP.YarnPriceNo,F.Name,YP.Ingredient,YP.Color,YP.YarnCount,YP.Price FROM YarnPrice YP
                              INNER JOIN Factory F ON YP.YarnMerchant = F.FactoryID";
            var result = DapperHelper.QueryCollection<MerchantYarnPrice>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd);
            return result;
        }
        /// <summary>
        /// 更新布種比例成分
        /// </summary>
        /// <param name="fabricIngredientProportions"></param>
        /// <returns></returns>
        public int UpdateFabricProportion(List<FabricIngredientProportion> fabricIngredientProportions)
        {
            var sqlCmd = @"UPDATE [dbo].[FabricProportion]
                           SET [YarnPriceNo] = @YarnPriceNo
                           ,[UpdateDate] = GETDATE()
                           WHERE ProportionNo=@ProportionNo";
            var result = DapperHelper.Execute(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, fabricIngredientProportions);
            return result;
        }
        /// <summary>
        /// 新增布種顏色
        /// </summary>
        /// <param name="fabricID"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public int InsertFabricColor(int fabricID, string text)
        {
           
            var sqlCmd = @"INSERT INTO FabricColor
                           OUTPUT INSERTED.ColorNo
                           VALUES
                           (@FabricID, @Color, GETDATE());";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@FabricID", SqlDbType.Int) { Value = fabricID },
                new SqlParameter("@Color", SqlDbType.NVarChar) { Value = text }
            };
            var result = DapperHelper.Query<int>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameters);
            return result;
        }
        /// <summary>
        /// 新增布種顏色的成分比例
        /// </summary>
        /// <param name="colorNo"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public int InsertFabricIngredientProportions(IEnumerable<FabricProportion> fabricProportions)
        {
            var sqlCmd = @"INSERT INTO [dbo].[FabricProportion]
                           ([ColorNo],[YarnPriceNo],[Proportion],[Group],[CreateDate])
                           VALUES
                           (@ColorNo,@YarnPriceNo,@Proportion,@Group,@CreateDate)";
            var result = DapperHelper.Execute(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, fabricProportions);
            return result;
        }
        /// <summary>
        /// 取得布種的成分群組資訊
        /// </summary>
        /// <param name="fabricID"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public IngredientGroupInfo GetIngredientGroupInfo(int fabricID, string color)
        {
            var sqlCmd = @"SELECT TOP 1 FC.ColorNo,FP.[GROUP] FROM FabricColor FC
                          INNER JOIN FabricProportion FP ON FC.ColorNo=FP.ColorNo
                          WHERE FabricID = @FabricID AND Color = @Color
                          ORDER BY FP.[GROUP] DESC";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@FabricID", SqlDbType.Int) { Value = fabricID },
                new SqlParameter("@Color", SqlDbType.NVarChar) { Value = color }
            };
            var result = DapperHelper.Query<IngredientGroupInfo>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameters);
            return result;
        }
    }
}
