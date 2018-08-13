using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.Entity;

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
    }
}
