using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.AccountSystem;

namespace WpfApp1.Utility.EqualityComparer
{
    public class AccountTextileComparer : IEqualityComparer<AccountTextile>
    {
        public bool Equals(AccountTextile x, AccountTextile y)
        {
            return x.FactoryID == y.FactoryID && x.ItemID == y.ItemID;
        }

        public int GetHashCode(AccountTextile obj)
        {
            //確認物件是否為空值
            if (Object.ReferenceEquals(obj, null)) return 0;

            //取得uId欄位的HashCode
            int itemID = obj.ItemID == null ? 0 : obj.ItemID.GetHashCode();

            //取得uName欄位的HashCode
            int factoryID = obj.FactoryID == null ? 0 : obj.FactoryID.GetHashCode();


            //計算HashCode，因為是XOR所以要全部都是1才會回傳1，否則都會回傳0
            return itemID ^ factoryID;
        }
    }
}
