using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using WpfApp1.Command;
using WpfApp1.DataClass.Entity;
using WpfApp1.DataClass.Enumeration;
using WpfApp1.DataClass.ExcelDataClass;
using WpfApp1.DataClass.StoreSearch;
using WpfApp1.Modules.FabricModule;
using WpfApp1.Modules.FabricModule.Implement;
using WpfApp1.Utility;

namespace WpfApp1.ViewModel.FabricViewModel
{
    public class FabricViewModel : ViewModelBase
    {

        protected IFabricModule FabricModule { get; } = new FabricModule();
        public ICommand CheckFabricClick { get { return new RelayCommand(CheckFabricClickExecute, CanExecute); } }
        public ICommand NewFabricDoubleClick { get { return new RelayCommand(NewFabricExecute, CanExecute); } }
        public ICommand ExportInventoryPriceClick { get { return new RelayCommand(ExportInventoryPriceToExcel, CanExecute); } }
        public ICommand FabricEditClick { get { return new RelayCommand(FabricEdit, CanExecute); } }

        private void FabricEdit()
        {
            int count = FabricModule.EditFabric(Fabric);
        }

        public ObservableCollection<string> FabricNameList { get; set; }

        public Fabric Fabric { get; set; }

        public ObservableCollection<Fabric> FabricList { get; set; }

        public int AverageUnitPrice { get; set; }
        public int AverageCost { get; set; }

        private string _fabricSearch { get; set; }
        public string FabricSearch
        {
            get { return _fabricSearch; }
            set
            {
                string filterText = value;
                ICollectionView cv = CollectionViewSource.GetDefaultView(FabricList);
                if (!string.IsNullOrEmpty(filterText))
                {
                    cv.Filter = o =>
                    {
                        /* change to get data row value */
                        Fabric p = o as Fabric;
                        return (p.FabricName.ToUpper().Contains(filterText.ToUpper()));
                        /* end change to get data row value */
                    };
                }
                else
                {
                    cv.Filter = o =>
                    {
                        return (true);
                    };
                };
                _fabricSearch = value;
            }
        }

        public string SelectedFabricName { get; set; }

        public FabricViewModel()
        {
            FabricNameList = new ObservableCollection<string>();
            FabricList = new ObservableCollection<Fabric>(FabricModule.GetFabricList());
            AverageUnitPrice = 200;
            AverageCost = 170;
        }
        private void NewFabricExecute()
        {
            var result = MessageBox.Show(string.Concat(@"是否要新增'", SelectedFabricName, "'?"), "新增布種", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Fabric fabric = new Fabric()
                {
                    FabricName = SelectedFabricName,
                    AverageUnitPrice = AverageUnitPrice,
                    AverageCost = AverageCost,
                    UpdateDate = DateTime.Now
                };

                int count = FabricModule.AddFabric(fabric);
            }
        }

        private void CheckFabricClickExecute()
        {
            var names = FabricModule.CheckExcelAndSqlFabricName();
            foreach (var name in names)
            {
                FabricNameList.Add(name);
            }
        }

        /// <summary>
        /// 取得所有布種庫存量資料
        /// </summary>
        /// <param name="list"></param>
        /// <param name="row"></param>
        /// <param name="timeRange"></param>
        /// <returns></returns>
        public List<StoreSearchData<InventoryPrice>> GetAllInventoryAction(List<StoreSearchData<InventoryPrice>> list, IRow row, string sheetName, int timeRange)
        {
            var columnIndex = ExcelEnum.ExcelInventoryColumnIndexEnum.CountInventory.ToInt();

            var countInventory = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.CountInventory);
            //如果沒有這個布種名稱則新增
            if (list.Where(w => w.TextileName == sheetName).Count() == 0)
            {
                list.Add(new StoreSearchData<InventoryPrice>
                {
                    TextileName = sheetName,
                    StoreSearchColorDetails = new List<InventoryPrice>()
                });
            }
            if (countInventory == null || string.IsNullOrEmpty(countInventory.ToString()) || (countInventory.CellType == CellType.Formula && countInventory.CachedFormulaResultType == CellType.Error) || countInventory.NumericCellValue < 0)
            {
                list.Last().StoreSearchColorDetails.Add(new InventoryPrice
                {
                    ColorName = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ColorName) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ColorName).ToString(),
                    CountInventory = 0,
                });
                return list;
            }
            var cellValue = countInventory.NumericCellValue; //獲取i行j列數據
            list.Last().StoreSearchColorDetails.Add(new InventoryPrice
            {
                ColorName = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ColorName) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ColorName).ToString(),
                CountInventory = Convert.ToInt32(cellValue),
            });

            return list;
        }

        private void CreateInventoryPriceExcelAction(IWorkbook wb, ISheet ws, ICellStyle positionStyle, ref int rowIndex, StoreSearchData<InventoryPrice> storeData)
        {
            XSSFRow rowTextile = (XSSFRow)ws.CreateRow(rowIndex);
            ExcelHelper.CreateCell(rowTextile, 0, storeData.TextileName, positionStyle);
            int countInventoryTotal = 0;
            foreach (var item in storeData.StoreSearchColorDetails)
            {
                countInventoryTotal += item.CountInventory;
            }
            var textile = FabricList.Where(w => w.FabricName == storeData.TextileName).FirstOrDefault();
            //未匯入Sql的布種不予計算
            if (textile != null)
            {
                ExcelHelper.CreateCell(rowTextile, 1, countInventoryTotal.ToString(), positionStyle);
                ExcelHelper.CreateCell(rowTextile, 2, (countInventoryTotal * 20 * textile.AverageUnitPrice).ToString(), positionStyle);
                ExcelHelper.CreateCell(rowTextile, 3, (countInventoryTotal * 20 * textile.AverageCost).ToString(), positionStyle);
            }
            rowIndex++;
        }

        private void ExportInventoryPriceToExcel()
        {
            ExcelHelper excelHelper = new ExcelHelper();
            ExcelFormat columnFormats = new ExcelFormat()
            {
                FileName = "庫存成本清單",
                ColumnFormats = new List<ColumnFormat>
                {
                    new ColumnFormat
                    {
                        CoiumnWidth = 3000,
                        ColumnTitle = "布種名稱",
                    },
                    new ColumnFormat
                    {
                        CoiumnWidth = 2800,
                        ColumnTitle = "庫存總數",
                    },
                    new ColumnFormat
                    {
                        CoiumnWidth = 1850,
                        ColumnTitle = "庫存總價格",
                    },
                    new ColumnFormat
                    {
                        CoiumnWidth = 1850,
                        ColumnTitle = "庫存總成本",
                    }
                }
            };
            excelHelper.InventoryCheckSheet<StoreSearchData<InventoryPrice>>(GetAllInventoryAction, CreateInventoryPriceExcelAction, 0, columnFormats);
        }

        //private bool CanExecute()
        //{
        //    return true;
        //}
    }
}
