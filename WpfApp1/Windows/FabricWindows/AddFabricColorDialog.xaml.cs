using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfApp1.DataClass.Entity;
using WpfApp1.Modules.FabricModule;
using WpfApp1.Modules.FabricModule.Implement;
using WpfApp1.ViewModel.FabricViewModel;

namespace WpfApp1.Windows.FabricWindows
{
    /// <summary>
    /// AddFabricColor.xaml 的互動邏輯
    /// </summary>
    public partial class AddFabricColorDialog : Window
    {

        protected IFabricModule FabricModule { get; } = new FabricModule();

        private Fabric _fabric;
        public AddFabricColorDialog(Fabric fabric)
        {
            InitializeComponent();
            _fabric = fabric;
            LabelFabricName.Content = fabric.FabricName;
        }

        private void ButtonAddFabricColor_Click(object sender, RoutedEventArgs e)
        {
            List<FabricColor> fabricColors = new List<FabricColor>
            {
                new FabricColor { FabricID = _fabric.FabricID, Color = TextBoxColorName.Text }
            };
            bool success = FabricModule.AddFabricColorList(fabricColors);
        }
    }
}
