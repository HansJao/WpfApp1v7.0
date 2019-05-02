using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.DataClass.Enumeration
{
    public class ExcelEnum
    {
        public enum ShippingSheetEnum
        {
            WeightCellNumber = 6,
            CellOfStringLength = 5
        }

        public enum ExcelInventoryColumnIndexEnum
        {
            Index = 0,
            ColorName = 1,
            StorageSpaces = 2,
            Inventory = 3,
            DifferentCylinder = 4,
            ShippingDate1 = 5,
            ShippingDate2 = 6,
            ShippingDate3 = 7,
            ShippingDate4 = 8,
            ShippingDate5 = 9,
            ShippingDate6 = 10,
            ShippingDate7 = 11,
            ShippingDate8 = 12,
            ShippingDate9 = 13,
            FabricFactory = 14,
            ClearFactory = 15,
            CountInventory = 16,
            IsChecked = 17,
            CheckDate = 18,
            Memo = 19
        }
        public enum ProcessFactoryEnum
        {
            //FabricFactory = "FabricFactory"

        }

        /// <summary>
        /// 加工訂單Cell Index
        /// </summary>
        public enum ProcessOrderColumnIndexEnum
        {

            /// <summary>
            /// 布種
            /// </summary>
            Fabric = 3,
            /// <summary>
            /// 工廠字串
            /// </summary>
            Factory = 3,
            /// <summary>
            /// 定型種類(漿內.針內.全幅)
            /// </summary>
            ClearType = 6,
            /// <summary>
            /// 幅寬
            /// </summary>
            Width = 7,
            /// <summary>
            /// 碼重
            /// </summary>
            Weight = 6,
            /// <summary>
            /// 加工項目
            /// </summary>
            ProcessItem = 3,
            /// <summary>
            /// 注意事項
            /// </summary>
            Precations = 7,
            /// <summary>
            /// 備註
            /// </summary>
            Memo = 7,
            /// <summary>
            /// 顏色
            /// </summary>
            Color = 2,
            /// <summary>
            /// 色號
            /// </summary>
            ColorNumber = 4,
            /// <summary>
            /// 疋數
            /// </summary>
            ColorQuantity = 6,
            /// <summary>
            /// 手感
            /// </summary>
            HandFeel = 9
        }
    }
}
