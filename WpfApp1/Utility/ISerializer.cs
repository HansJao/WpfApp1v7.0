using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Utility
{
    public interface ISerializer
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="T">type for serialize</typeparam>
        /// <param name="obj">欲被序列化的物件</param>
        /// <returns>
        /// 序列化後的字串
        /// </returns>
        string Serialize<T>(T obj);

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T">type for deserialize</typeparam>
        /// <param name="data">序列化字串</param>
        /// <returns>
        /// 反序列化後的物件
        /// </returns>
        T Deserialize<T>(string data);
    }
}
