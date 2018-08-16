using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfApp1.Command;
using WpfApp1.DataClass.Enumeration;
using WpfApp1.DataClass.StoreSearch;

namespace WpfApp1.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public ICommand InventoryNumberRangeClick { get { return new RelayCommand(ExportCheckDateExcel, CanExecute); } }

        private void ExportCheckDateExcel()
        {
            
        }       
    }
}
