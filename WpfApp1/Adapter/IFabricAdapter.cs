﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.Entity;
using WpfApp1.DataClass.Fabric;

namespace WpfApp1.Adapter
{
    public interface IFabricAdapter
    {

        /// <summary>
        /// 取得布種清單
        /// </summary>
        /// <returns></returns>
        IEnumerable<Fabric> GetFabricList();
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
        int AddFabricColorList(List<FabricColor> fabricColors);
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
        /// <param name="colorNo"></param>
        /// <returns></returns>
        IEnumerable<ProcessSequenceDetail> GetProcessSequences(int fabricID, int colorNo);
        /// <summary>
        /// 以布種編號取得所有加工程序
        /// </summary>
        /// <param name="fabricID"></param>
        /// <returns></returns>
        IEnumerable<ProcessSequenceDetail> GetProcessSequencesByFabricID(int fabricID);
        /// <summary>
        /// 取得所有紗商的紗價
        /// </summary>
        /// <returns></returns>
        IEnumerable<MerchantYarnPrice> GetMerchantYarnPriceList();
        /// <summary>
        /// 新增紗價
        /// </summary>
        /// <param name="yarnPrice"></param>
        /// <returns></returns>
        int InsertYarnPrice(YarnPrice yarnPrice);
        /// <summary>
        /// 更新紗價
        /// </summary>
        /// <param name="yarnPrice"></param>
        /// <returns></returns>
        int EditYarnPrice(YarnPrice yarnPrice);
        /// <summary>
        /// 刪除紗價
        /// </summary>
        /// <param name="yarnPriceNo"></param>
        /// <returns></returns>
        int DeleteYarnPrice(int yarnPriceNo);
        /// <summary>
        /// 更新布種比例成分
        /// </summary>
        /// <param name="fabricIngredientProportions"></param>
        /// <returns></returns>
        int UpdateFabricProportion(List<FabricIngredientProportion> fabricIngredientProportions);
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
        /// <param name="list"></param>
        /// <returns></returns>
        int InsertFabricIngredientProportions(IEnumerable<FabricProportion> list);
        /// <summary>
        /// 取得布種的成分群組資訊
        /// </summary>
        /// <param name="fabricID"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        IngredientGroupInfo GetIngredientGroupInfo(int fabricID, string color);
        /// <summary>
        /// 新增加工程序
        /// </summary>
        /// <param name="processSequenceDetails"></param>
        /// <returns></returns>
        List<int> InsertProcessSequence(List<ProcessSequenceDetail> processSequenceDetails);

        /// <summary>
        /// 取得布種加工程序群組標示
        /// </summary>
        /// <param name="fabricID"></param>
        /// <returns></returns>
        int GetGroupIndex(int fabricID);
        /// <summary>
        /// 新增加工程序顏色對照
        /// </summary>
        /// <param name="processSequenceColorMapping"></param>
        /// <returns></returns>
        int InsertProcessSequenceColorMapping(IEnumerable<ProcessSequenceColorMapping> processSequenceColorMapping);
        /// <summary>
        /// 刪除加工程序
        /// </summary>
        /// <param name="colorNo"></param>
        /// <param name="group"></param>
        /// <param name="sequenceNoList"></param>
        /// <returns></returns>
        int DeleteProcessSequence(int colorNo, int group, IEnumerable<int> sequenceNoList);
       
    }
}
