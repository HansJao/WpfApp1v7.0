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
            ISheet sheet = workbook.GetSheet(SelectedTextile);  //獲取工作表
            List<TextileColorInventory> selectedTextiles = new List<TextileColorInventory>();
            IRow row;
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                row = sheet.GetRow(i);
                if (row == null)
                {
                    break;
                }
                var differentCylinder = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.DifferentCylinder) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.DifferentCylinder).CellType == CellType.Blank ? "" : "有不同缸應注意";
                var cellValue = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.CountInventory) == null || (row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.CountInventory).CellType == CellType.Formula ? row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.CountInventory).CachedFormulaResultType == CellType.Error : false)
                    ? ""
                    : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.CountInventory).NumericCellValue.ToString();

                double inventory = ExcelModule.CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.Inventory));
                selectedTextiles.Add(new TextileColorInventory
                {
                    Index = ExcelModule.CheckExcelCellType<string>(CellType.String, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.Index)),
                    ColorName = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ColorName) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ColorName).ToString(),
                    StorageSpaces = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.StorageSpaces) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.StorageSpaces).ToString(),
                    Inventory = inventory,
                    DifferentCylinder = ExcelModule.CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.DifferentCylinder)),
                    ShippingDate1 = ExcelModule.CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate1)),
                    ShippingDate2 = ExcelModule.CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate2)),
                    ShippingDate3 = ExcelModule.CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate3)),
                    ShippingDate4 = ExcelModule.CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate4)),
                    ShippingDate5 = ExcelModule.CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate5)),
                    ShippingDate6 = ExcelModule.CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate6)),
                    ShippingDate7 = ExcelModule.CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate7)),
                    ShippingDate8 = ExcelModule.CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate8)),
                    ShippingDate9 = ExcelModule.CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate9)),
                    CountInventory = cellValue,
                    IsChecked = ExcelModule.CheckExcelCellType<string>(CellType.String, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.IsChecked)),
                    CheckDate = ExcelModule.CheckExcelCellType<string>(CellType.String, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.CheckDate)),
                    ClearFactory = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ClearFactory) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ClearFactory).ToString(),
                    Memo = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.Memo) == null ? differentCylinder : string.Concat(row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.Memo).ToString(), ",", differentCylinder)
                });
            }
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
