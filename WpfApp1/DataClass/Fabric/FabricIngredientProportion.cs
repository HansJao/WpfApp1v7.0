using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.DataClass.Fabric
{
    public class FabricIngredientProportion
    {
        /// <summary>
        /// 比例編號
        /// </summary>
        public int ProportionNo { get; set; }
        /// <summary>
        /// 紗商名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 紗商名稱
        /// </summary>
        public int YarnPriceNo { get; set; }
        /// <summary>
        /// 紗成份
        /// </summary>
        public string Ingredient { get; set; }
        /// <summary>
        /// 紗色
        /// </summary>
        public string Color { get; set; }
        /// <summary>
        /// 紗支數
        /// </summary>
        public string YarnCount { get; set; }
        /// <summary>
        /// 紗價
        /// </summary>
        public int Price { get; set; }
        /// <summary>
        /// 布種顏色比例
        /// </summary>
        public decimal Proportion { get; set; }
        /// <summary>
        /// 群組
        /// </summary>
        public int Group { get; set; }
    }
}
