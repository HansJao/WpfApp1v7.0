using System;
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
        /// 以布種編號取得加工順序
        /// </summary>
        /// <param name="fabricIDList"></param>
        /// <returns></returns>
        IEnumerable<ProcessSequence> GetProcessSequences(IEnumerable<int> fabricIDList);
        /// <summary>
        /// 取得所有紗商的紗價
        /// </summary>
        /// <returns></returns>
        IEnumerable<MerchantYarnPrice> GetMerchantYarnPriceList();
    }
}
