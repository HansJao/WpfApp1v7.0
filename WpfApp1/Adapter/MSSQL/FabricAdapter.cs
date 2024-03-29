﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.Entity;
using WpfApp1.DataClass.Entity.FabricEntity;
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

            string sqlCmd = @"SELECT FP.ProportionNo,F.Name,FP.YarnPriceNo,YP.Price,YS.Ingredient,YS.Color,YS.YarnCount,FP.Proportion,FP.[Group] FROM FabricProportion FP
                              INNER JOIN YarnPrice YP ON FP.YarnPriceNo = YP.YarnPriceNo
							  INNER JOIN Factory F ON YP.YarnMerchant = F.FactoryID
							  INNER JOIN YarnSpecification YS ON YS.YarnSpecificationNo = YP.YarnSpecificationNo
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
        /// 取得加工順序
        /// </summary>
        /// <param name="fabricID"></param>
        /// <param name="colorNo"></param>
        /// <returns></returns>
        public IEnumerable<ProcessSequenceDetail> GetProcessSequences(int fabricID, int colorNo)
        {
            string sqlCmd = @"SELECT PSCM.ColorNo,F.Name,PS.SequenceNo,PS.FabricID,PS.FactoryID,PS.ProcessItem,PS.Loss,PS.WorkPay,PS.[Order],PS.[Group] 
                              FROM [hungyidb].[dbo].[ProcessSequenceColorMapping] PSCM
                              RIGHT JOIN ProcessSequence PS ON PSCM.SequenceNo = PS.SequenceNo
                              LEFT JOIN Factory F ON PS.FactoryID = F.FactoryID
                              WHERE (PS.FabricID = @FabricID AND PSCM.ColorNo IS NULL) OR PSCM.ColorNo = @ColorNo";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@FabricID", SqlDbType.Int) { Value = fabricID },
                new SqlParameter("@ColorNo", SqlDbType.Int) { Value = colorNo }
            };
            var result = DapperHelper.QueryCollection<ProcessSequenceDetail>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameters);
            return result;
        }

        /// <summary>
        /// 以布種編號取得所有加工程序
        /// </summary>
        /// <param name="fabricID"></param>
        /// <returns></returns>
        public IEnumerable<ProcessSequenceDetail> GetProcessSequencesByFabricID(int fabricID)
        {
            string sqlCmd = @"SELECT PS.*,F.Name FROM ProcessSequence PS
                              INNER JOIN Factory F ON PS.FactoryID = F.FactoryID
                              WHERE FabricID = @FabricID";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@FabricID", SqlDbType.Int) { Value = fabricID }
            };
            var result = DapperHelper.QueryCollection<ProcessSequenceDetail>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameters);
            return result;
        }
        /// <summary>
        /// 取得所有紗商的紗價
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MerchantYarnPrice> GetMerchantYarnPriceList()
        {
            string sqlCmd = @"SELECT * FROM YarnPrice";
            var result = DapperHelper.QueryCollection<MerchantYarnPrice>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd);
            return result;
        }

        /// <summary>
        /// 以紗規格編號取得紗價
        /// </summary>
        /// <param name="yarnSpecificationNo"></param>
        /// <returns></returns>
        public IEnumerable<MerchantYarnPrice> GetYarnPriceByYarnSpecificationNo(int yarnSpecificationNo)
        {
            string sqlCmd = @"SELECT F.Name,FB.Name AS BrandName,YP.YarnMerchant,YP.YarnPriceNo,YP.YarnSpecificationNo,YP.Price,YP.PiecePrice,YP.CreateDate 
                              FROM YarnPrice YP
							  INNER JOIN Factory F ON YP.YarnMerchant = F.FactoryID
							  INNER JOIN Factory FB ON YP.Brand = FB.FactoryID
                              WHERE YarnSpecificationNo = @YarnSpecificationNo";
            SqlParameter[] parameters = new SqlParameter[]
          {
                new SqlParameter("@YarnSpecificationNo", SqlDbType.Int) { Value = yarnSpecificationNo }
          };
            var result = DapperHelper.QueryCollection<MerchantYarnPrice>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameters);
            return result;
        }
        /// <summary>
        /// 新增紗價
        /// </summary>
        /// <param name="yarnPrice"></param>
        /// <returns></returns>
        public int InsertYarnPrice(YarnPrice yarnPrice)
        {
            string sqlCmd = @"INSERT INTO [dbo].[YarnPrice]
                              ([YarnSpecificationNo]
                              ,[YarnMerchant]
                              ,[Brand]
                              ,[Price]
                              ,[PiecePrice])
                              VALUES
                              (@YarnSpecificationNo
                              ,@YarnMerchant
                              ,@Brand
                              ,@Price
                              ,@PiecePrice)";
            int count = DapperHelper.Execute(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, yarnPrice);
            return count;
        }
        /// <summary>
        /// 更新紗價
        /// </summary>
        /// <param name="yarnPrice"></param>
        /// <returns></returns>
        public int EditYarnPrice(YarnPrice yarnPrice)
        {
            string sqlCmd = @"UPDATE [dbo].[YarnPrice]
                            SET [Ingredient] = @Ingredient
                            ,[YarnCount] = @YarnCount
                            ,[Color] = @Color
                            ,[Price] = @Price
                            ,[PiecePrice] = @PiecePrice
                            ,[YarnMerchant] = @YarnMerchant
                            ,[UpdateDate] = GETDATE()
                            WHERE [YarnPriceNo] = @YarnPriceNo";
            int count = DapperHelper.Execute(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, yarnPrice);
            return count;
        }
        /// <summary>
        /// 刪除紗價
        /// </summary>
        /// <param name="yarnPriceNo"></param>
        /// <returns></returns>
        public int DeleteYarnPrice(int yarnPriceNo)
        {
            string sqlCmd = @"DELETE [dbo].[YarnPrice]
                              WHERE [YarnPriceNo] = @YarnPriceNo";
            SqlParameter[] parameters = new SqlParameter[]
            {
                 new SqlParameter("@YarnPriceNo", SqlDbType.Int) { Value = yarnPriceNo },
            };
            int count = DapperHelper.ExecuteParameter(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameters);
            return count;
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
                              ,[Proportion] = @Proportion
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
                           VALUES
                           (@FabricID, @Color, GETDATE());";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@FabricID", SqlDbType.Int) { Value = fabricID },
                new SqlParameter("@Color", SqlDbType.NVarChar) { Value = text }
            };
            var result = DapperHelper.ExecuteParameter(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameters);
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
        /// 刪除布種成分比例
        /// </summary>
        /// <param name="fabricColorNo"></param>
        /// <param name="groupNo"></param>
        /// <returns></returns>
        public int DeleteFabricIngredientProportions(int fabricColorNo, int groupNo)
        {
            string sqlCmd = @"DELETE [dbo].[FabricProportion]
                              WHERE [ColorNo] = @ColorNo AND [Group] = @Group";
            SqlParameter[] parameters = new SqlParameter[]
            {
                 new SqlParameter("@ColorNo", SqlDbType.Int) { Value = fabricColorNo },
                 new SqlParameter("@Group", SqlDbType.Int) { Value = groupNo }
            };
            int count = DapperHelper.ExecuteParameter(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameters);
            return count;
        }
        /// <summary>
        /// 取得布種的成分群組資訊
        /// </summary>
        /// <param name="fabricID"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public IngredientGroupInfo GetIngredientGroupInfo(int fabricID, int fabricColorNo)
        {
            var sqlCmd = @"SELECT TOP 1 FC.ColorNo,FP.[GROUP] FROM FabricColor FC
                          LEFT JOIN FabricProportion FP ON FC.ColorNo=FP.ColorNo
                          WHERE FabricID = @FabricID AND FC.ColorNo = @ColorNo
                          ORDER BY FP.[GROUP] DESC";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@FabricID", SqlDbType.Int) { Value = fabricID },
                new SqlParameter("@ColorNo", SqlDbType.Int) { Value = fabricColorNo }
            };
            var result = DapperHelper.Query<IngredientGroupInfo>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameters);
            return result;
        }
        /// <summary>
        /// 新增加工程序
        /// </summary>
        /// <param name="processSequenceDetails"></param>
        /// <returns></returns>
        public List<int> InsertProcessSequence(List<ProcessSequenceDetail> processSequenceDetails)
        {
            string sqlCmd = @"
            INSERT INTO [dbo].[ProcessSequence]
           ([FabricID]
           ,[FactoryID]
           ,[ProcessItem]
           ,[Loss]
           ,[WorkPay]
           ,[Order]
           ,[Group])
             OUTPUT INSERTED.SequenceNo
            VALUES
           (@FabricID
           ,@FactoryID
           ,@ProcessItem
           ,@Loss
           ,@WorkPay
           ,@Order
           ,@Group)";

            List<int> sequenceNoList = new List<int>();
            foreach (var item in processSequenceDetails)
            {
                SqlParameter[] parameters = new SqlParameter[]
               {
                new SqlParameter("@FabricID", SqlDbType.Int) { Value = item.FabricID },
                new SqlParameter("@FactoryID", SqlDbType.Int) { Value = item.FactoryID },
                new SqlParameter("@ProcessItem", SqlDbType.TinyInt) { Value = item.ProcessItem },
                new SqlParameter("@Loss", SqlDbType.Decimal) { Value = item.Loss },
                new SqlParameter("@WorkPay", SqlDbType.SmallInt) { Value = item.WorkPay },
                new SqlParameter("@Order", SqlDbType.TinyInt) { Value = item.Order },
                new SqlParameter("@Group", SqlDbType.TinyInt) { Value = item.Group }
               };
                sequenceNoList.Add(DapperHelper.Query<int>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameters));
            }
            return sequenceNoList;
        }
        /// <summary>
        /// 取得布種加工程序群組標示
        /// </summary>
        /// <param name="fabricID"></param>
        /// <returns></returns>
        public int GetGroupIndex(int fabricID)
        {
            string sqlCmd = @"
                  SELECT TOP 1 [Group] + 1 FROM ProcessSequence
                  WHERE FabricID = @FabricID
                  ORDER BY [Group] DESC;";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@FabricID", SqlDbType.Int) { Value = fabricID },
            };
            var result = DapperHelper.QueryCollection<int>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameters);
            return result.Count() == 0 ? 1 : result.First();
        }
        /// <summary>
        /// 新增加工程序顏色對照
        /// </summary>
        /// <param name="processSequenceColorMapping"></param>
        /// <returns></returns>
        public int InsertProcessSequenceColorMapping(IEnumerable<ProcessSequenceColorMapping> processSequenceColorMapping)
        {
            string sqlCmd = @"
            INSERT INTO [dbo].[ProcessSequenceColorMapping]
            ([ColorNo],[SequenceNo])
            VALUES
            (@ColorNo,@SequenceNo)";
            int count = DapperHelper.Execute(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, processSequenceColorMapping);
            return count;
        }
        /// <summary>
        /// 刪除加工程序
        /// </summary>
        /// <param name="colorNo"></param>
        /// <param name="group"></param>
        /// <param name="sequenceNoList"></param>
        /// <returns></returns>
        public int DeleteProcessSequence(int colorNo, int group, IEnumerable<int> sequenceNoList)
        {
            string sqlCmd = string.Format(@"
               DECLARE @SequenceList TABLE(SequenceNo int)
         --將此顏色的加工程序暫存於此
               INSERT INTO @SequenceList
               SELECT PSCM.SequenceNo FROM ProcessSequenceColorMapping PSCM
               INNER JOIN ProcessSequence PS ON PSCM.SequenceNo = PS.SequenceNo
               WHERE PSCM.ColorNo = @ColorNo AND PS.[Group] = @Group
         --如果存在@SequenceList代表此色有Mapping要先刪除Mapping
         --然後再判斷Mapping裡是否還有,否則刪除加工程序內的資料
               IF EXISTS(SELECT * FROM @SequenceList)
               BEGIN
	             DELETE ProcessSequenceColorMapping WHERE ColorNo = @ColorNo AND SequenceNo IN (SELECT SequenceNo FROM @SequenceList)
	             IF NOT EXISTS(SELECT * FROM ProcessSequenceColorMapping WHERE SequenceNo IN (SELECT SequenceNo FROM @SequenceList))
		            DELETE ProcessSequence WHERE SequenceNo IN (SELECT SequenceNo FROM @SequenceList)
               END
               ELSE 
               BEGIN
                 DELETE ProcessSequence WHERE SequenceNo IN ({0})
               END", string.Join(",", sequenceNoList));

            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@ColorNo", SqlDbType.Int) { Value = colorNo },
                new SqlParameter("@Group", SqlDbType.Int) { Value = group }
              };
            int count = DapperHelper.ExecuteParameter(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameters);
            return count;
        }

        /// <summary>
        /// 檢查此加工程序是否有在加工程序顏色對應中
        /// </summary>
        /// <param name="processSequenceDetails"></param>
        /// <returns></returns>
        public int CheckIsInProcessSequenceColorMapping(int colorNo, IEnumerable<int> processSequences)
        {
            string sqlCmd = @"SELECT COUNT(*) FROM [ProcessSequenceColorMapping]
                              WHERE ColorNo = @ColorNo and SequenceNo IN @SequenceNo";
            var parameter =
               new
               {
                   ColorNo = colorNo,
                   SequenceNo = processSequences
               };
            var result = DapperHelper.QueryCollection<int, object>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameter).First();
            return result;
        }
        /// <summary>
        /// 修改加工程序,可修改工繳,損耗,順序
        /// </summary>
        /// <param name="sequenceNo"></param>
        /// <param name="loss"></param>
        /// <param name="workPay"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public int EditProcessSequence(int sequenceNo, decimal loss, int workPay, int order)
        {
            var sqlCmd = @"UPDATE [dbo].[ProcessSequence]
                           SET 
                            [Loss] = @Loss
                           ,[WorkPay] = @WorkPay
                           ,[Order] = @Order
                           WHERE SequenceNo=@SequenceNo";
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@SequenceNo", SqlDbType.Int) { Value = sequenceNo },
                new SqlParameter("@Loss", SqlDbType.Decimal) { Value = loss },
                new SqlParameter("@WorkPay", SqlDbType.Int) { Value = workPay },
                new SqlParameter("@Order", SqlDbType.Int) { Value = order }
              };
            var result = DapperHelper.ExecuteParameter(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameters);
            return result;
        }
        /// <summary>
        /// 新增紗規格
        /// </summary>
        /// <param name="yarnSpecification"></param>
        /// <returns></returns>
        public int AddYarnSpecification(YarnSpecification yarnSpecification)
        {
            string sqlCmd = @"INSERT INTO [dbo].[YarnSpecification]
            ([Ingredient],[YarnCount],[Color])
            VALUES
            (@Ingredient,@YarnCount,@Color)";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Ingredient", SqlDbType.NVarChar) { Value = yarnSpecification.Ingredient },
                new SqlParameter("@YarnCount", SqlDbType.NVarChar) { Value = yarnSpecification.YarnCount },
                new SqlParameter("@Color", SqlDbType.NVarChar) { Value = yarnSpecification.Color }
            };
            var result = DapperHelper.ExecuteParameter(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameters);
            return result;
        }
        /// <summary>
        /// 取得紗規格清單
        /// </summary>
        /// <returns></returns>
        public IEnumerable<YarnSpecification> GetYarnSpecificationList()
        {
            string sqlCmd = @"SELECT [YarnSpecificationNo]
                              ,[Ingredient]
                              ,[YarnCount]
                              ,[Color]
                              ,[CreateDate] 
                              FROM [YarnSpecification]";
            var result = DapperHelper.QueryCollection<YarnSpecification>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd);
            return result;
        }
        /// <summary>
        /// 以紗商取得紗規格清單
        /// </summary>
        /// <returns></returns>
        public IEnumerable<YarnSpecification> GetYarnSpecificationListByYarnMerchant(int yarnMerchant)
        {
            string sqlCmd = @"SELECT DISTINCT YS.* FROM [hungyidb].[dbo].[YarnPrice] YP
                              INNER JOIN YarnSpecification YS ON YP.YarnSpecificationNo = YS.YarnSpecificationNo
                              WHERE YarnMerchant = @YarnMerchant";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@YarnMerchant", SqlDbType.Int) { Value = yarnMerchant }
            };
            var result = DapperHelper.QueryCollection<YarnSpecification>(AppSettingConfig.ConnectionString(), CommandType.Text, sqlCmd, parameters);
            return result;
        }
    }
}
