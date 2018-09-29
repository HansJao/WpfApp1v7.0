using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfApp1.Command;
using WpfApp1.DataClass.Entity;
using WpfApp1.DataClass.Fabric;
using WpfApp1.Modules.FabricModule;
using WpfApp1.Modules.FabricModule.Implement;
using WpfApp1.Windows.FabricWindows;

namespace WpfApp1.ViewModel.FabricViewModel
{
    public class FabricCostQueryViewModel : ViewModelBase
    {
        protected IFabricModule FabricModule { get; } = new FabricModule();

        public ICommand AddFabricColorClick { get { return new RelayCommand(AddFabricColorExecute, CanExecute); } }

        private void AddFabricColorExecute()
        {
            AddFabricColorDialog addFabricColorDialog = new AddFabricColorDialog(Fabric);
            addFabricColorDialog.Show();
        }

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
                FabricColorList.Clear();
                ProcessSequenceList.Clear();
                if (value == null) return;
                List<int> fabricIDList = new List<int> { _fabric.FabricID };

                var fabricColorList = FabricModule.GetFabricColorListByFabricID(fabricIDList);
                foreach (var item in fabricColorList)
                {
                    FabricColorList.Add(item);
                }

                IEnumerable<ProcessSequence> processSequences = FabricModule.GetProcessSequences(new List<int> { _fabric.FabricID });
                foreach (var item in processSequences)
                {
                    ProcessSequenceList.Add(item);
                }
            }
        }
        public ObservableCollection<FabricColor> FabricColorList { get; set; }
        private FabricColor _fabricColor { get; set; }
        public FabricColor FabricColor
        {
            get
            {
                return _fabricColor;
            }
            set
            {
                _fabricColor = value;
                FabricIngredientProportionList.Clear();
                if (value == null) return;
                IEnumerable<FabricIngredientProportion> fabricIngredientProportions = FabricModule.GetFabricIngredientProportionByColorNo(new List<int> { value.ColorNo });

                foreach (var item in fabricIngredientProportions)
                {
                    FabricIngredientProportionList.Add(item);
                }
            }
        }
        public ObservableCollection<FabricIngredientProportion> FabricIngredientProportionList { get; set; }

        public ObservableCollection<ProcessSequence> ProcessSequenceList { get; set; }

        public FabricCostQueryViewModel()
        {
            FabricList = new ObservableCollection<Fabric>(FabricModule.GetFabricList());
            FabricColorList = new ObservableCollection<FabricColor>();
            FabricIngredientProportionList = new ObservableCollection<FabricIngredientProportion>();
            ProcessSequenceList = new ObservableCollection<ProcessSequence>();
        }
    }
}
