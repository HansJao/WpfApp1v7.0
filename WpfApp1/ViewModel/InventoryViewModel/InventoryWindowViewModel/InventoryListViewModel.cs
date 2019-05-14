using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
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
            RaisePropertyChanged("TextileInventoryHeader");
        }

        public void FilterColorName(string selectedColorName)
        {
            if (TextileColorList == null) return;
            Regex regex = new Regex("[" + selectedColorName + "]");
            ICollectionView cv = CollectionViewSource.GetDefaultView(TextileColorList);
            if (!string.IsNullOrEmpty(selectedColorName))
            {
                cv.Filter = o =>
                {
                    /* change to get data row value */
                    TextileColorInventory p = o as TextileColorInventory;
                    return (regex.IsMatch(p.ColorName));
                    /* end change to get data row value */
                };
            }
            else
            {
                cv.Filter = o =>
                {
                    return (true);
                };
            }
        }
    }
}
