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
using WpfApp1.ViewModel.FabricViewModel;

namespace WpfApp1.Windows.FabricWindows
{
    /// <summary>
    /// AddFabricColorDialog.xaml 的互動邏輯
    /// </summary>
    public partial class AddFabricColorDialog : Window
    {
        public AddFabricColorDialog(Fabric fabric)
        {
            InitializeComponent();
            this.DataContext = new AddFabricColorViewModel(fabric);
        }
    }
}
