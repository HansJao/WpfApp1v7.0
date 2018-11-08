using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.TrashSystem;

namespace WpfApp1.Modules.TrashModule
{
    public interface ITrashModule
    {

        /// <summary>
        /// 取得布種名稱
        /// </summary>
        /// <returns></returns>
        IEnumerable<TrashItem> GetTrashItems();
        IEnumerable<TrashItem> GetTrashItemsByFeature(string feature);
        IEnumerable<TrashShipped> GetTrashShippedList(DateTime datePickerBegin, DateTime datePickerEnd);
        void UpdateProductName(string v);
    }
}
