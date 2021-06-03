using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfApp1.DataClass.ExcelDataClass;
using WpfApp1.ViewModel.InventoryViewModel.InventoryWindowViewModel;

namespace WpfApp1.Windows.InventoryWindows
{
    /// <summary>
    /// InventoryListDialog.xaml 的互動邏輯
    /// </summary>
    public partial class InventoryListDialog : Window
    {
        public InventoryListViewModel InventoryListViewModel;
        public InventoryListDialog(string fileName, TextileInventoryHeader textileInventoryHeader, IEnumerable<TextileColorInventory> textileColorInventoryList)
        {
            InitializeComponent();
            InventoryListViewModel = new InventoryListViewModel()
             {
                 FileName = fileName,
                 TextileColorList = textileColorInventoryList,
                 TextileInventoryHeader = textileInventoryHeader
             };
            this.DataContext = InventoryListViewModel;
        }
        public void ChangeDataContext(TextileInventoryHeader textileInventoryHeader, IEnumerable<TextileColorInventory> textileColorInventoryList)
        {
            InventoryListViewModel inventoryListViewModel =  this.DataContext as InventoryListViewModel;
            inventoryListViewModel.TextileColorList = textileColorInventoryList;
            inventoryListViewModel.TextileInventoryHeader = textileInventoryHeader;
            inventoryListViewModel.Change();
        }
    }
}
