using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using WpfApp1.Command;
using WpfApp1.DataClass.Enumeration;
using WpfApp1.DataClass.ExcelDataClass;
using WpfApp1.Modules.ExcelModule;
using WpfApp1.Modules.ExcelModule.Implement;
using WpfApp1.Utility;
using WpfApp1.Windows.InventoryWindows;

namespace WpfApp1.ViewModel.InventoryViewModel
{
    public class InventoryRecordCompareViewModel : ViewModelBase
    {
        protected IExcelModule ExcelModule { get; } = new ExcelModule();

        public ICommand ComboBoxSelectionChanged { get { return new RelayCommand(ComboBoxSelectionChangedExecute, CanExecute); } }
        public ICommand ComboBoxTextileKeyUp { get { return new RelayCommand(ComboBoxTextileKeyUpExecute, CanExecute); } }
        public ICommand ComboBoxTextileSelectionChanged { get { return new RelayCommand(ComboBoxTextileSelectionChangedExecute, CanExecute); } }
        public ICommand InventoryDataGridDoubleClick { get { return new RelayCommand(InventoryDataGridDoubleClickExecute, CanExecute); } }

        public IEnumerable<string> InventoryRecordFileList { get; set; }
        public InventoryRecordCompareViewModel()
        {
            IEnumerable<string> existsFileName = Directory.GetFiles(AppSettingConfig.InventoryHistoryRecordFilePath(), "*.xlsx").Select(System.IO.Path.GetFileName).OrderByDescending(o => o);
            InventoryRecordFileList = existsFileName;
        }

        public TextileColorInventory TextileColor { get; set; }
        private void InventoryDataGridDoubleClickExecute()
        {
            List<TextileColorInventory> textileColorInventorys = new List<TextileColorInventory>
            {
                TextileColor
            };
            InventoryListDialog inventoryListDialog = new InventoryListDialog(FileName, TextileInventoryHeader, textileColorInventorys);
            inventoryListDialog.Show();
        }

        public IEnumerable<TextileColorInventory> TextileColorList { get; set; }
        private void ComboBoxTextileSelectionChangedExecute()
        {
            if (string.IsNullOrEmpty(SelectedTextile)) return;
            if (!_workbookDictionary.TryGetValue(FileName, out IWorkbook workbook))
                return;
            TextileInventoryHeader.Textile = SelectedTextile;
            RaisePropertyChanged("TextileInventoryHeader");
            ExcelHelper excelHelper = new ExcelHelper();
            List<TextileColorInventory> selectedTextiles = new List<TextileColorInventory>();
            selectedTextiles = excelHelper.GetInventoryData(workbook, SelectedTextile);

            TextileColorList = selectedTextiles;
            RaisePropertyChanged("TextileColorList");
        }



        private void ComboBoxTextileKeyUpExecute()
        {
            CollectionView itemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView(TextileList);

            itemsViewOriginal.Filter = ((o) =>
            {
                if (string.IsNullOrEmpty(TextileText)) return false;
                else
                {
                    if (((string)o).Contains(TextileText)) return true;
                    else return false;
                }
            });

            //cmb.IsDropDownOpen = true;
            itemsViewOriginal.Refresh();
        }

        private Dictionary<string, IWorkbook> _workbookDictionary { get; set; } = new Dictionary<string, IWorkbook>();
        public IEnumerable<string> TextileList { get; set; }
        public string TextileText { get; set; }
        public string FileName { get; set; }

        private void ComboBoxSelectionChangedExecute()
        {
            string priviousTextile = SelectedTextile;
            TextileColorList = null;
            TextileList = null;
            RaisePropertyChanged("TextileColorList");
            RaisePropertyChanged("TextileList");
            List<string> textileList = new List<string>();
            if (!_workbookDictionary.TryGetValue(FileName, out IWorkbook dictionaryWorkbook))
            {
                string fileNamePath = string.Concat(AppSettingConfig.InventoryHistoryRecordFilePath(), "/", FileName);
                Tuple<List<string>, IWorkbook> tuple = ExcelModule.GetExcelWorkbook(fileNamePath);
                TextileList = tuple.Item1;
                GetShippingDate(tuple.Item2.GetSheetAt(1));
                _workbookDictionary.Add(FileName, tuple.Item2);
            }
            else
            {
                for (int sheetCount = 1; sheetCount < dictionaryWorkbook.NumberOfSheets; sheetCount++)
                {
                    ISheet sheet = dictionaryWorkbook.GetSheetAt(sheetCount);  //獲取第i個工作表  
                    textileList.Add(sheet.SheetName);
                }
                TextileList = textileList;
                GetShippingDate(dictionaryWorkbook.GetSheetAt(1));
            }
            RaisePropertyChanged("TextileList");
            SelectedTextile = priviousTextile;
            RaisePropertyChanged("SelectedTextile");
        }
        public TextileInventoryHeader TextileInventoryHeader { get; set; }
        public string SelectedTextile { get; set; }
        public void GetShippingDate(ISheet sheet)
        {
            TextileInventoryHeader = ExcelModule.GetShippingDate(sheet);
            RaisePropertyChanged("TextileInventoryHeader");
        }
    }
}
