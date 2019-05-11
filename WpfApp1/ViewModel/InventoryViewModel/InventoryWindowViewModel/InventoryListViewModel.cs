using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.ExcelDataClass;

namespace WpfApp1.ViewModel.InventoryViewModel.InventoryWindowViewModel
{
    public class InventoryListViewModel : ViewModelBase
    {
        public string FileName { get; set; }
        public IEnumerable<TextileColorInventory> TextileColorList { get; set; }
        public TextileInventoryHeader TextileInventoryHeader { get; set; }
        public InventoryListViewModel()
        {

        }

        public void Change()
        {
            RaisePropertyChanged("TextileColorList");
        }
    }
}
