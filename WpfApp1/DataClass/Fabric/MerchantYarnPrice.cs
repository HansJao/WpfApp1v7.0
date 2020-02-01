using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.Entity;

namespace WpfApp1.DataClass.Fabric
{
    public class MerchantYarnPrice : YarnPrice
    {

        /// <summary>
        /// 紗商
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 廠牌
        /// </summary>
        public string BrandName { get; set; }
    }
}
