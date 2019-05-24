using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.ExcelDataClass;
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
        /// <summary>
        /// 取得Excel 出貨日期
        /// </summary>
        /// <returns></returns>
        TextileInventoryHeader GetShippingDate(ISheet sheet);
        /// <summary>
        /// 取得Excel Workbook
        /// </summary>
        /// <returns></returns>
        Tuple<List<string>, IWorkbook> GetExcelWorkbook(string fileName);
    }
}
