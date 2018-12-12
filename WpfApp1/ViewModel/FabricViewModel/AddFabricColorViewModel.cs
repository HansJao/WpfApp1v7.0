using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfApp1.Command;
using WpfApp1.DataClass.Entity;
using WpfApp1.Modules.FabricModule;
using WpfApp1.Modules.FabricModule.Implement;
using WpfApp1.Utility;

namespace WpfApp1.ViewModel.FabricViewModel
{
    public class AddFabricColorViewModel : ViewModelBase
    {
        protected IFabricModule FabricModule { get; } = new FabricModule();
        public ICommand AddFabricColorClick { get { return new RelayCommand(AddFabricColorExecute, CanExecute); } }


        private Fabric _fabric { get; set; }
        public AddFabricColorViewModel(Fabric fabric)
        {
            _fabric = fabric;
            FabricName = fabric.FabricName;
        }
        public string FabricName { get; set; }
        public string FabricColor { get; set; }
        private void AddFabricColorExecute()
        {
            bool success = FabricModule.InsertFabricColor(_fabric.FabricID, FabricColor) == 1;
            success.CheckSuccessMessageBox("新增成功!!", "新增失敗");
        }
    }
}
