using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.StoreSearch;

namespace WpfApp1.Modules.ExcelModule
{
    public interface IExcelModule
    {

        /// <summary>
        /// 取得Excel每日出貨清單
        /// </summary>
        /// <returns></returns>
        List<StoreSearchData<StoreSearchColorDetail>> GetExcelDailyShippedList(DateTime? shippedDate);
    }
}
