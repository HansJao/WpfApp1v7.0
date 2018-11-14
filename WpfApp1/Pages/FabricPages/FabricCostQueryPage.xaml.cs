using System.Windows.Controls;
using WpfApp1.ViewModel.FabricViewModel;

namespace WpfApp1.Pages.FabricPages
{
    /// <summary>
    /// FabricCostPage.xaml 的互動邏輯
    /// </summary>
    public partial class FabricCostQueryPage : Page
    {
        public FabricCostQueryPage()
        {
            InitializeComponent();
            this.DataContext = new FabricCostQueryViewModel(StackPanelArea, StackPanelProcessSequence);
        }
    }
}
