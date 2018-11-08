using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Adapter;
using WpfApp1.Adapter.DBF;
using WpfApp1.DataClass.TrashSystem;

namespace WpfApp1.Modules.TrashModule.Implement
{
    public class TrashModule : ITrashModule
    {
        private ITrashAdapter _trashAdapter;
        protected ITrashAdapter TrashAdapter
        {
            get
            {
                if (this._trashAdapter == null)
                {
                    this._trashAdapter = new TrashAdapter();
                }
                return this._trashAdapter;
            }
            set
            {
                this._trashAdapter = value;
            }
        }

        public IEnumerable<TrashItem> GetTrashItems()
        {
            var trashItems = TrashAdapter.GetTrashItems();
            return trashItems;
        }

        public IEnumerable<TrashItem> GetTrashItemsByFeature(string feature)
        {
            IEnumerable<TrashItem> trashItems = TrashAdapter.GetTrashItemsByFeature(feature);
            return trashItems;
        }

        public IEnumerable<TrashShipped> GetTrashShippedList(DateTime datePickerBegin, DateTime datePickerEnd)
        {
            IEnumerable<TrashShipped> trashList = TrashAdapter.GetTrashShippedList(datePickerBegin, datePickerEnd);
            return trashList;
        }

        public void UpdateProductName(string v)
        {
            TrashAdapter.UpdateProductName(v);
        }
    }
}
