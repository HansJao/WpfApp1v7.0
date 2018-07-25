using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.Entity;
using WpfApp1.DataClass.FactoryClass;

namespace WpfApp1.Modules.FactoryModule
{
    public interface IFactoryModule
    {
        /// <summary>
        /// 取得工廠編號
        /// </summary>
        /// <param name="FactoryName"></param>
        /// <returns></returns>
        IEnumerable<FactoryIdentity> GetFactoryIdentiys(List<string> FactoryName);

        /// <summary>
        /// 新增工廠清單
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        int InsertFactory(Factory factory);
        /// <summary>
        /// 取得工廠清單
        /// </summary>
        /// <returns></returns>
        IEnumerable<Factory> GetFactoryList();
        /// <summary>
        /// 修改工廠明細
        /// </summary>
        /// <returns></returns>
        int UpdateFactory(Factory factory);
    }
}
