using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Adapter;
using WpfApp1.Adapter.MSSQL;
using WpfApp1.DataClass.Entity;
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
    }
}
