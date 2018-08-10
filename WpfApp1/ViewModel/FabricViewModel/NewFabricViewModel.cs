using NPOI.SS.UserModel;
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
using WpfApp1.DataClass.StoreSearch;
using WpfApp1.Modules.FabricModule;
using WpfApp1.Modules.FabricModule.Implement;
using WpfApp1.Utility;

namespace WpfApp1.ViewModel.FabricViewModel
{
    public class NewFabricViewModel : ViewModelBase
    {

        protected IFabricModule FabricModule { get; } = new FabricModule();
        public ICommand CheckFabricClick { get { return new RelayCommand(CheckFabricClickExecute, CanExecute); } }
        public ICommand NewFabricDoubleClick { get { return new RelayCommand(NewFabricExecute, CanExecute); } }

        private ObservableCollection<string> _fabricNameList { get; set; }

        public ObservableCollection<string> FabricDataList
        {
            get { return _fabricNameList; }
            set { _fabricNameList = value; }
        }

        private ObservableCollection<Fabric> _fabricList { get; set; }

        public ObservableCollection<Fabric> FabricList
        {
            get { return _fabricList; }
            set { _fabricList = value; }
        }

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

        public NewFabricViewModel()
        {
            _fabricNameList = new ObservableCollection<string>();
            _fabricList = new ObservableCollection<Fabric>(FabricModule.GetFabricList());
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
                FabricDataList.Add(name);
            }
        }

        /// <summary>
        /// 依據是否出貨過濾資料
        /// </summary>
        /// <param name="list"></param>
        /// <param name="row"></param>
        /// <param name="timeRange"></param>
        /// <returns></returns>
        public List<StoreSearchData<InventoryCheck>> GetCountInventoryAction(List<StoreSearchData<InventoryCheck>> list, IRow row, int timeRange)
        {
            var columnIndex = ExcelEnum.ExcelInventoryColumnIndexEnum.CountInventory.ToInt();
            if (row.GetCell(columnIndex) != null && row.GetCell(columnIndex).CellType != CellType.String && row.GetCell(columnIndex).NumericCellValue != 0)
            {
                var countInventory = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.CountInventory);
                if (countInventory == null || string.IsNullOrEmpty(countInventory.ToString()) || (countInventory.CellType == CellType.Formula && countInventory.CachedFormulaResultType == CellType.Error))
                {
                    //continue;
                }
                var cellValue = countInventory.NumericCellValue; //獲取i行j列數據
                list.Last().StoreSearchColorDetails.Add(new InventoryCheck
                {
                    ColorName = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ColorName) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ColorName).ToString(),
                    StorageSpaces = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.StorageSpaces) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.StorageSpaces).ToString(),
                    CountInventory = cellValue.ToString(),
                });
            }


            return list;
        }

        private bool CanExecute()
        {
            return true;
        }
    }
}
