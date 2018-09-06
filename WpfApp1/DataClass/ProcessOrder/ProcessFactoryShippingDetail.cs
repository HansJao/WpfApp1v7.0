using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.Entity;

namespace WpfApp1.DataClass.ProcessOrder
{

    /// <summary>
    /// 加工廠出貨明細
    /// </summary>
    public class ProcessOrderColorFactoryShippingDetail : ProcessOrderColorDetail
    {
        public int ShippingQuantity { get; set; }
    }
}
