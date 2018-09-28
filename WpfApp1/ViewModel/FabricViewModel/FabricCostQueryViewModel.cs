using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.Entity;
using WpfApp1.Modules.FabricModule;
using WpfApp1.Modules.FabricModule.Implement;

namespace WpfApp1.ViewModel.FabricViewModel
{
    public class FabricCostQueryViewModel : ViewModelBase
    {
        protected IFabricModule FabricModule { get; } = new FabricModule();

        public ObservableCollection<Fabric> FabricList { get; set; }
        private Fabric _fabric { get; set; }
        public Fabric Fabric
        {
            get
            {
                return _fabric;
            }
            set
            {
                _fabric = value;
                List<int> fabricIDList = new List<int> { _fabric.FabricID };
                FabricColorList.Clear();
                foreach (var item in FabricModule.GetFabricColorListByFabricID(fabricIDList))
                {
                    FabricColorList.Add(item);
                }

            }
        }
        public ObservableCollection<FabricColor> FabricColorList { get; set; }

        public FabricCostQueryViewModel()
        {
            FabricList = new ObservableCollection<Fabric>(FabricModule.GetFabricList());
            FabricColorList = new ObservableCollection<FabricColor>();
        }
    }
}
