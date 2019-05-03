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
using WpfApp1.Windows.InventoryWindows;

namespace WpfApp1.ViewModel.InventoryViewModel
{
    public class InventoryRecordCompareViewModel : ViewModelBase
    {
        public ICommand ComboBoxSelectionChanged { get { return new RelayCommand(ComboBoxSelectionChangedExecute, CanExecute); } }
        public ICommand ComboBoxTextileKeyUp { get { return new RelayCommand(ComboBoxTextileKeyUpExecute, CanExecute); } }
        public ICommand ComboBoxTextileSelectionChanged { get { return new RelayCommand(ComboBoxTextileSelectionChangedExecute, CanExecute); } }
        public ICommand InventoryDataGridDoubleClick { get { return new RelayCommand(InventoryDataGridDoubleClickExecute, CanExecute); } }

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
        public TextileColorInventory TextileColor { get; set; }
        private void ComboBoxTextileSelectionChangedExecute()
        {
            if (string.IsNullOrEmpty(TextileInventoryHeader.Textile)) return;
            if (!_workbookDictionary.TryGetValue(FileName, out IWorkbook workbook))
                return;

            ISheet sheet = workbook.GetSheet(TextileInventoryHeader.Textile);  //獲取工作表
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

                double inventory = CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.Inventory));
                selectedTextiles.Add(new TextileColorInventory
                {
                    Index = CheckExcelCellType<string>(CellType.String, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.Index)),
                    ColorName = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ColorName) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ColorName).ToString(),
                    StorageSpaces = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.StorageSpaces) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.StorageSpaces).ToString(),
                    Inventory = inventory,
                    DifferentCylinder = CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.DifferentCylinder)),
                    ShippingDate1 = CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate1)),
                    ShippingDate2 = CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate2)),
                    ShippingDate3 = CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate3)),
                    ShippingDate4 = CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate4)),
                    ShippingDate5 = CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate5)),
                    ShippingDate6 = CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate6)),
                    ShippingDate7 = CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate7)),
                    ShippingDate8 = CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate8)),
                    ShippingDate9 = CheckExcelCellType<double>(CellType.Numeric, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate9)),
                    CountInventory = cellValue,
                    IsChecked = CheckExcelCellType<string>(CellType.String, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.IsChecked)),
                    CheckDate = CheckExcelCellType<string>(CellType.String, row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.CheckDate)),
                    ClearFactory = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ClearFactory) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ClearFactory).ToString(),
                    Memo = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.Memo) == null ? differentCylinder : string.Concat(row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.Memo).ToString(), ",", differentCylinder)
                });
            }
            TextileColorList = selectedTextiles;
            RaisePropertyChanged("TextileColorList");
        }

        private T CheckExcelCellType<T>(CellType cellType, ICell cell)
        {
            switch (cellType)
            {
                case CellType.Unknown:
                    return default(T);
                case CellType.Numeric:
                    if (cell == null)
                    {
                        return (T)Convert.ChangeType(-1, typeof(T));
                    }
                    else if (cell.CellType == cellType)
                    {
                        return (T)Convert.ChangeType(cell.NumericCellValue, typeof(T));
                    }
                    else if (cell.CellType == CellType.Blank)
                    {
                        return default(T);
                    }
                    else
                    {
                        return (T)Convert.ChangeType(999, typeof(T));
                    }
                case CellType.String:
                    if (cell == null)
                    {
                        return (T)Convert.ChangeType("null", typeof(T));
                    }
                    else if (cell.CellType == cellType)
                    {
                        return (T)Convert.ChangeType(cell.StringCellValue, typeof(T)); ;
                    }
                    else if (cell.CellType == CellType.Blank)
                    {
                        return default(T);
                    }
                    else if (cell.CellType == CellType.Numeric)
                    {
                        return (T)Convert.ChangeType(cell.NumericCellValue.ToString(), typeof(T));
                    }
                    else
                    {
                        return (T)Convert.ChangeType("Unknown", typeof(T));
                    }
                case CellType.Formula:
                    return default(T);
                case CellType.Blank:
                    return default(T);
                case CellType.Boolean:
                    return default(T);
                case CellType.Error:
                    return default(T);
                default:
                    return default(T);
            }
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
                string fileName = string.Concat(AppSettingConfig.InventoryHistoryRecordFilePath(), "/", FileName);
                FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                IWorkbook Workbook = new XSSFWorkbook(fileStream);  //xlsx數據讀入workbook
                for (int sheetCount = 1; sheetCount < Workbook.NumberOfSheets; sheetCount++)
                {
                    ISheet sheet = Workbook.GetSheetAt(sheetCount);  //獲取第i個工作表  
                    textileList.Add(sheet.SheetName);
                }
                GetShippingDate(Workbook.GetSheetAt(1));
                _workbookDictionary.Add(FileName, Workbook);
            }
            else
            {
                for (int sheetCount = 1; sheetCount < dictionaryWorkbook.NumberOfSheets; sheetCount++)
                {
                    ISheet sheet = dictionaryWorkbook.GetSheetAt(sheetCount);  //獲取第i個工作表  
                    textileList.Add(sheet.SheetName);
                }
                GetShippingDate(dictionaryWorkbook.GetSheetAt(1));
            }
            TextileList = textileList;
            RaisePropertyChanged("TextileList");
        }
        public TextileInventoryHeader TextileInventoryHeader { get; set; }
        public void GetShippingDate(ISheet sheet)
        {
            TextileInventoryHeader = new TextileInventoryHeader
            {
                ShippingDate1 = CheckExcelCellType<string>(CellType.String, sheet.GetRow(0).GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate1)),
                ShippingDate2 = CheckExcelCellType<string>(CellType.String, sheet.GetRow(0).GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate2)),
                ShippingDate3 = CheckExcelCellType<string>(CellType.String, sheet.GetRow(0).GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate3)),
                ShippingDate4 = CheckExcelCellType<string>(CellType.String, sheet.GetRow(0).GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate4)),
                ShippingDate5 = CheckExcelCellType<string>(CellType.String, sheet.GetRow(0).GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate5)),
                ShippingDate6 = CheckExcelCellType<string>(CellType.String, sheet.GetRow(0).GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate6)),
                ShippingDate7 = CheckExcelCellType<string>(CellType.String, sheet.GetRow(0).GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate7)),
                ShippingDate8 = CheckExcelCellType<string>(CellType.String, sheet.GetRow(0).GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate8)),
                ShippingDate9 = CheckExcelCellType<string>(CellType.String, sheet.GetRow(0).GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ShippingDate9))
            };
            RaisePropertyChanged("TextileInventoryHeader");
        }

        public IEnumerable<string> InventoryRecordFileList { get; set; }
        public InventoryRecordCompareViewModel()
        {
            IEnumerable<string> existsFileName = Directory.GetFiles(AppSettingConfig.InventoryHistoryRecordFilePath(), "*.xlsx").Select(System.IO.Path.GetFileName).OrderByDescending(o => o);
            InventoryRecordFileList = existsFileName;
        }
    }
}
