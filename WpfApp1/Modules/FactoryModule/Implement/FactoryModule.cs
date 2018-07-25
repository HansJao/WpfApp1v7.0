using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Adapter;
using WpfApp1.Adapter.MSSQL;
using WpfApp1.DataClass.Entity;
using WpfApp1.DataClass.FactoryClass;

namespace WpfApp1.Modules.FactoryModule.Implement
{
    public class FactoryModule : IFactoryModule
    {
        private IFacroryAdapter _factoryAdapter;
        protected IFacroryAdapter FactoryAdapter
        {
            get
            {
                if (this._factoryAdapter == null)
                {
                    this._factoryAdapter = new FactoryAdapter();
                }
                return this._factoryAdapter;
            }
            set
            {
                this._factoryAdapter = value;
            }
        }


        /// <summary>
        /// 取得工廠編號
        /// </summary>
        /// <param name="FactoryName"></param>
        /// <returns></returns>
        public IEnumerable<FactoryIdentity> GetFactoryIdentiys(List<string> FactoryName)
        {
            var factoryIdentitys = FactoryAdapter.GetFactoryIdentiys(FactoryName.ToArray());
            var result = factoryIdentitys.OrderBy(o => FactoryName.IndexOf(o.Name));
            return result;
        }
        /// <summary>
        /// 取得工廠清單
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Factory> GetFactoryList()
        {
            var result = FactoryAdapter.GetFactoryList().OrderBy(o => o.Sort == 0).ThenBy(o => o.Sort);
            return result;
        }

        /// <summary>
        /// 新增工廠清單
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        public int InsertFactory(Factory factory)
        {
            var count = FactoryAdapter.InsertFactory(factory);
            return count;
        }
        /// <summary>
        /// 修改工廠明細
        /// </summary>
        /// <returns></returns>
        public int UpdateFactory(Factory factory)
        {
            int count = FactoryAdapter.UpdateFactory(factory);
            return count;
        }
    }
}
