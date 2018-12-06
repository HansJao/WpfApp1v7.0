using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        /// 以布種編號取得所有加工程序
        /// </summary>
        /// <param name="fabricID"></param>
        /// <returns></returns>
        List<ProcessSequenceDetail> GetProcessSequencesByFabricID(int fabricID);

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
        bool InsertYarnPrice(YarnPrice yarnPrice);
        /// <summary>
        /// 更新紗價
        /// </summary>
        /// <param name="yarnPrice"></param>
        /// <returns></returns>
        bool EditYarnPrice(YarnPrice yarnPrice);
        /// <summary>
        /// 刪除紗價
        /// </summary>
        /// <param name="yarnPriceNo"></param>
        /// <returns></returns>
        bool DeleteYarnPrice(int yarnPriceNo);
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

        /// <summary>
        /// 新增加工程序
        /// </summary>
        /// <param name="processSequenceDetails"></param>
        /// <returns></returns>
        List<int> InsertProcessSequence(List<ProcessSequenceDetail> processSequenceDetails);

        /// <summary>
        /// 新增加工程序顏色對照
        /// </summary>
        /// <param name="processSequenceColorMapping"></param>
        /// <returns></returns>
        bool InsertProcessSequenceColorMapping(IEnumerable<ProcessSequenceColorMapping> processSequenceColorMapping);

        /// <summary>
        /// 刪除加工程序
        /// </summary>
        /// <param name="colorNo"></param>
        /// <param name="group"></param>
        /// <param name="sequenceNoList"></param>
        /// <returns></returns>
        bool DeleteProcessSequence(int colorNo, int group,IEnumerable<int> sequenceNoList);

        /// <summary>
        /// 檢查此加工程序是否有在加工程序顏色對應中
        /// </summary>
        /// <param name="processSequenceDetails"></param>
        /// <returns></returns>
        bool CheckIsInProcessSequenceColorMapping(IEnumerable<ProcessSequenceDetail> processSequenceDetails);
    }
}
