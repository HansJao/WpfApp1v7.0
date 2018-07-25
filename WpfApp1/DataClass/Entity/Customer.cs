using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.DataClass.Entity
{
    public class Customer
    {
        /// <summary>
        /// 客戶編號
        /// </summary>
        public int CustomerID { get; set; }
        /// <summary>
        /// 客戶名稱
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
        /// 傳真號碼
        /// </summary>
        public string Fax { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public string Memo { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
    }
}
