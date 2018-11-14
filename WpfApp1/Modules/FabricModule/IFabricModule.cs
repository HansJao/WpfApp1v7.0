﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.Entity;
using WpfApp1.DataClass.Fabric;

namespace WpfApp1.Modules.FabricModule
{
    public interface IFabricModule
    {
        IEnumerable<Fabric> GetFabricList();
        IEnumerable<string> CheckExcelAndSqlFabricName();

        /// <summary>
        /// 新增布種
        /// </summary>
        /// <param name="fabric"></param>
        /// <returns></returns>
        int AddFabric(Fabric fabric);
        /// <summary>
        /// 更新布種
        /// </summary>
        /// <param name="fabric"></param>
        /// <returns></returns>
        int EditFabric(Fabric fabric);
        /// <summary>
        /// 以布種編號取得布種顏色
        /// </summary>
        /// <param name="fabricIDList"></param>
        /// <returns></returns>
        IEnumerable<FabricColor> GetFabricColorListByFabricID(IEnumerable<int> fabricIDList);

        /// <summary>
        /// 新增布種顏色
        /// </summary>
        /// <param name="fabricColors"></param>
        bool AddFabricColorList(List<FabricColor> fabricColors);

        /// <summary>
        /// 以布種顏色編號取得顏色比例
        /// </summary>
        /// <param name="fabricColorNoList"></param>
        /// <returns></returns>
        IEnumerable<FabricProportion> GetFabricProportionByColorNo(List<int> fabricColorNoList);
        /// <summary>
        /// 以布種顏色編號取得布種顏色比例
        /// </summary>
        /// <param name="fabricColorNoList"></param>
        /// <returns></returns>
        IEnumerable<FabricIngredientProportion> GetFabricIngredientProportionByColorNo(List<int> fabricColorNoList);

        /// <summary>
        /// 取得加工順序
        /// </summary>
        /// <param name="fabricID"></param>
        /// <returns></returns>
        IEnumerable<ProcessSequenceDetail> GetProcessSequences(int fabricID,int colorNo);
        /// <summary>
        /// 取得所有紗商的紗價
        /// </summary>
        /// <returns></returns>
        IEnumerable<MerchantYarnPrice> GetMerchantYarnPriceList();

        /// <summary>
        /// 更新布種比例成分
        /// </summary>
        /// <param name="fabricIngredientProportions"></param>
        /// <returns></returns>
        bool UpdateFabricProportion(List<FabricIngredientProportion> fabricIngredientProportions);

        /// <summary>
        /// 新增布種顏色
        /// </summary>
        /// <param name="fabricID"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        int InsertFabricColor(int fabricID, string text);

        /// <summary>
        /// 新增布種顏色的成分比例
        /// </summary>
        /// <param name="colorNo"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        bool InsertFabricIngredientProportions(int colorNo, List<FabricIngredientProportion> list);

        /// <summary>
        /// 取得布種的成分群組資訊
        /// </summary>
        /// <param name="fabricID"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        IngredientGroupInfo GetIngredientGroupInfo(int fabricID, string color);
    }
}
