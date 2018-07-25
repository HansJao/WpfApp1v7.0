using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.Enumeration;

namespace WpfApp1.DataClass.Entity
{
    public class Factory
    {
        /// <summary>
        /// 工廠編號
        /// </summary>
        public int FactoryID { get; set; }
        /// <summary>
        /// 工廠名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 市內電話
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 行動電話
        /// </summary>
        public string CellPhone { get; set; }
        /// <summary>
        /// 傳真
        /// </summary>
        public string Fax { get; set; }
        /// <summary>
        /// 加工項目
        /// </summary>
        public ProcessItem Process { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
    }
}
