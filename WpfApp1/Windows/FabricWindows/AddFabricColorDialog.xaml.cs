using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using WpfApp1.DataClass.Fabric;
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
        private ObservableCollection<FabricIngredientProportion> _fabricIngredientProportion;
        private ObservableCollection<FabricColor> _fabricColorList;
        public AddFabricColorDialog(Fabric fabric, FabricColor FabricColor, ObservableCollection<FabricIngredientProportion> fabricIngredientProportion, ObservableCollection<FabricColor> FabricColorList)
        {
            InitializeComponent();
            _fabric = fabric;
            _fabricIngredientProportion = fabricIngredientProportion;
            _fabricColorList = FabricColorList;

            LabelFabricName.Content = fabric.FabricName;
            DataGridFabricIngredientProportion.ItemsSource = _fabricIngredientProportion;
            TextBoxColorName.Text = FabricColor == null ? string.Empty : FabricColor.Color;
        }

        private void ButtonAddIngredientGroup_Click(object sender, RoutedEventArgs e)
        {
            List<FabricColor> fabricColors = new List<FabricColor>
            {
                new FabricColor { FabricID = _fabric.FabricID, Color = TextBoxColorName.Text }
            };
            bool success = FabricModule.AddFabricColorList(fabricColors);
        }

        private void ButtonChangeYarn_Click(object sender, RoutedEventArgs e)
        {
            YarnSelectDialog yarnSelectDialog = new YarnSelectDialog(ref _fabricIngredientProportion);
            yarnSelectDialog.DataContext = this;
            yarnSelectDialog.Show();
        }

        private void TextBoxColorName_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            int isInFabricColorList = _fabricColorList.Where(w => w.Color == textBox.Text).Count();
            if (isInFabricColorList > 0)
            {
                ButtonAddIngredientGroup.IsEnabled = true;
                ButtonAddFabricColor.IsEnabled = false;
                ButtonEditFabricColor.IsEnabled = true;
            }
            else if (textBox.Text == string.Empty)
            {
                ButtonAddIngredientGroup.IsEnabled = false;
                ButtonAddFabricColor.IsEnabled = false;
                ButtonEditFabricColor.IsEnabled = false;
            }
            else
            {
                ButtonAddIngredientGroup.IsEnabled = false;
                ButtonAddFabricColor.IsEnabled = true;
                ButtonEditFabricColor.IsEnabled = false;
            }
        }

        private void ButtonEditFabricColor_Click(object sender, RoutedEventArgs e)
        {
            List<FabricIngredientProportion> fabricIngredientProportions = new List<FabricIngredientProportion>();
            foreach (FabricIngredientProportion item in DataGridFabricIngredientProportion.Items)
            {
                fabricIngredientProportions.Add(item);
            }
            bool success = FabricModule.UpdateFabricProportion(fabricIngredientProportions);
            if (success == false)
            {
                MessageBox.Show("好像有錯誤喔!!");
            }
            else
            {
                MessageBox.Show("更新成功!!");
            }
        }
    }
}
