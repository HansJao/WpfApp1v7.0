using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using WpfApp1.Command;
using WpfApp1.DataClass.ExcelDataClass;

namespace WpfApp1.ViewModel.InventoryViewModel.InventoryWindowViewModel
{
    public class InventoryListViewModel : ViewModelBase
    {
        public string FileName { get; set; }
        public IEnumerable<TextileColorInventory> TextileColorList { get; set; }
        public TextileInventoryHeader TextileInventoryHeader { get; set; }
        public TextileColorInventory TextileColor { get; set; }
        public ICommand InventoryDataGridSelectionChanged { get { return new RelayCommand(InventoryDataGridSelectionChangedExecute, CanExecute); } }

        private void InventoryDataGridSelectionChangedExecute()
        {
            //throw new NotImplementedException();
        }

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
            if (TextileColorList == null) return;//([芥黃])|([白])
            Regex regex = new Regex("(" + selectedColorName.Replace("/", ")+|(") + ")+");
            ICollectionView cv = CollectionViewSource.GetDefaultView(TextileColorList);
            if (!string.IsNullOrEmpty(selectedColorName))
            {
                bool isContainTrue = false;
                cv.Filter = o =>
                {
                    /* change to get data row value */
                    TextileColorInventory p = o as TextileColorInventory;
                    var check = regex.Matches(p.ColorName.Split('-')[0]).Count == selectedColorName.Split('/').Count();
                    if (check == true) isContainTrue = true;
                    return (check);
                    /* end change to get data row value */
                };
                if(!isContainTrue)
                {
                    cv.Filter = o =>
                    {
                        return (true);
                    };
                }
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
