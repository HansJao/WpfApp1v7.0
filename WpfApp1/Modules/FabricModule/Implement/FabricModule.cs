using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Adapter;
using WpfApp1.Adapter.MSSQL;
using WpfApp1.DataClass.Entity;
using WpfApp1.DataClass.Fabric;
using WpfApp1.Utility;

namespace WpfApp1.Modules.FabricModule.Implement
{
    public class FabricModule : IFabricModule
    {
        private IFabricAdapter _FabricAdapter;
        protected IFabricAdapter FabricAdapter
        {
            get
            {
                if (this._FabricAdapter == null)
                {
                    this._FabricAdapter = new FabricAdapter();
                }
                return this._FabricAdapter;
            }
            set
            {
                this._FabricAdapter = value;
            }
        }
        public IEnumerable<Fabric> GetFabricList()
        {
            var result = FabricAdapter.GetFabricList();
            return result;
        }

        public IEnumerable<string> CheckExcelAndSqlFabricName()
        {
            var sqlFabricName = FabricAdapter.GetFabricList().Select(s => s.FabricName).ToList();
            var excelFabricName = new ExcelHelper().GetExcelSheetName();
            var notInSqlFabric = excelFabricName.Except(sqlFabricName);
            return notInSqlFabric;
        }
        /// <summary>
        /// 新增布種
        /// </summary>
        /// <param name="fabric"></param>
        /// <returns></returns>
        public int AddFabric(Fabric fabric)
        {
            int count = FabricAdapter.AddFabric(fabric);
            return count;
        }

        /// <summary>
        /// 更新布種
        /// </summary>
        /// <param name="fabric"></param>
        /// <returns></returns>
        public int EditFabric(Fabric fabric)
        {
            fabric.UpdateDate = DateTime.Now;
            int count = FabricAdapter.EditFabric(fabric);
            return count;
        }
        /// <summary>
        /// 以布種編號取得布種顏色
        /// </summary>
        /// <param name="fabricIDList"></param>
        /// <returns></returns>
        public IEnumerable<FabricColor> GetFabricColorListByFabricID(IEnumerable<int> fabricIDList)
        {
            IEnumerable<FabricColor> fabricColors = FabricAdapter.GetFabricColorListByFabricID(fabricIDList);
            return fabricColors;
        }

        /// <summary>
        /// 新增布種顏色
        /// </summary>
        /// <param name="fabricColors"></param>
        public bool AddFabricColorList(List<FabricColor> fabricColors)
        {
            int count = FabricAdapter.AddFabricColorList(fabricColors);
            return fabricColors.Count() == count;
        }
        /// <summary>
        /// 以布種顏色編號取得顏色比例
        /// </summary>
        /// <param name="fabricColorNoList"></param>
        /// <returns></returns>
        public IEnumerable<FabricProportion> GetFabricProportionByColorNo(List<int> fabricColorNoList)
        {
            IEnumerable<FabricProportion> fabricProportions = FabricAdapter.GetFabricProportionByColorNo(fabricColorNoList);
            return fabricProportions;
        }
        /// <summary>
        /// 以布種顏色編號取得布種顏色比例
        /// </summary>
        /// <param name="fabricColorNoList"></param>
        /// <returns></returns>
        public IEnumerable<FabricIngredientProportion> GetFabricIngredientProportionByColorNo(List<int> fabricColorNoList)
        {
            IEnumerable<FabricIngredientProportion> fabricIngredientProportions = FabricAdapter.GetFabricIngredientProportionByColorNo(fabricColorNoList);
            return fabricIngredientProportions;
        }
        /// <summary>
        /// 取得加工順序
        /// </summary>
        /// <param name="fabricID"></param>
        /// <param name="fabricID"></param>
        /// <returns></colorNo>
        public IEnumerable<ProcessSequenceDetail> GetProcessSequences(int fabricID, int colorNo)
        {
            IEnumerable<ProcessSequenceDetail> processSequences = FabricAdapter.GetProcessSequences(fabricID, colorNo);
            return processSequences;
        }
        /// <summary>
        /// 以布種編號取得所有加工程序
        /// </summary>
        /// <param name="fabricID"></param>
        /// <returns></returns>
        public List<ProcessSequenceDetail> GetProcessSequencesByFabricID(int fabricID)
        {
            IEnumerable<ProcessSequenceDetail> processSequenceDetails = FabricAdapter.GetProcessSequencesByFabricID(fabricID);
            return processSequenceDetails.ToList();
        }

        /// <summary>
        /// 取得所有紗商的紗價
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MerchantYarnPrice> GetMerchantYarnPriceList()
        {
            IEnumerable<MerchantYarnPrice> merchantYarnPrices = FabricAdapter.GetMerchantYarnPriceList();
            return merchantYarnPrices;

        }
        /// <summary>
        /// 更新布種比例成分
        /// </summary>
        /// <param name="fabricIngredientProportions"></param>
        /// <returns></returns>
        public bool UpdateFabricProportion(List<FabricIngredientProportion> fabricIngredientProportions)
        {
            int count = FabricAdapter.UpdateFabricProportion(fabricIngredientProportions);
            return count == fabricIngredientProportions.Count;
        }
        /// <summary>
        /// 新增布種顏色
        /// </summary>
        /// <param name="fabricID"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public int InsertFabricColor(int fabricID, string text)
        {
            int colorNo = FabricAdapter.InsertFabricColor(fabricID, text);
            return colorNo;
        }
        /// <summary>
        /// 新增布種顏色的成分比例
        /// </summary>
        /// <param name="colorNo"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool InsertFabricIngredientProportions(int colorNo, List<FabricIngredientProportion> list)
        {
            IEnumerable<FabricProportion> fabricProportions = list.Select(s => new FabricProportion
            {
                ColorNo = colorNo,
                YarnPriceNo = s.YarnPriceNo,
                Proportion = s.Proportion,
                Group = s.Group == 0 ? 1 : s.Group,
                CreateDate = DateTime.Now
            });
            int count = FabricAdapter.InsertFabricIngredientProportions(fabricProportions);
            return count == list.Count;
        }
        /// <summary>
        /// 取得布種的成分群組資訊
        /// </summary>
        /// <param name="fabricID"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public IngredientGroupInfo GetIngredientGroupInfo(int fabricID, string color)
        {
            IngredientGroupInfo ingredientGroupInfo = FabricAdapter.GetIngredientGroupInfo(fabricID, color);
            return ingredientGroupInfo;
        }
        /// <summary>
        /// 新增加工程序
        /// </summary>
        /// <param name="processSequenceDetails"></param>
        /// <returns></returns>
        public List<int> InsertProcessSequence(List<ProcessSequenceDetail> processSequenceDetails)
        {
            int groupIndex = FabricAdapter.GetGroupIndex(processSequenceDetails.First().FabricID);
            int order = 0;
            processSequenceDetails.ForEach(f =>
            {
                order++;
                f.Order = order;
                f.Group = groupIndex;
            });
            List<int> sequenceNoList = FabricAdapter.InsertProcessSequence(processSequenceDetails);
            return sequenceNoList;
        }
        /// <summary>
        /// 新增加工程序顏色對照
        /// </summary>
        /// <param name="processSequenceColorMapping"></param>
        /// <returns></returns>
        public bool InsertProcessSequenceColorMapping(IEnumerable<ProcessSequenceColorMapping> processSequenceColorMapping)
        {
            int count = FabricAdapter.InsertProcessSequenceColorMapping(processSequenceColorMapping);
            return count == processSequenceColorMapping.Count();
        }
        /// <summary>
        /// 刪除加工程序
        /// </summary>
        /// <param name="colorNo"></param>
        /// <param name="group"></param>
        /// <param name="sequenceNoList"></param>
        /// <returns></returns>
        public bool DeleteProcessSequence(int colorNo, int group, IEnumerable<int> sequenceNoList)
        {
            int count = FabricAdapter.DeleteProcessSequence(colorNo, group, sequenceNoList);
            return true;
        }
    }
}
