using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.TrashSystem;

namespace WpfApp1.Adapter
{
    public interface ITrashAdapter
    {
        IEnumerable<TrashItem> GetTrashItems();
        IEnumerable<TrashItem> GetTrashItemsByFeature(string feature);
        IEnumerable<TrashShipped> GetTrashShippedList(DateTime datePickerBegin, DateTime datePickerEnd);
    }
}
