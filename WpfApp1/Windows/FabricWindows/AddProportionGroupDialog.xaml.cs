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
using WpfApp1.Utility;
using WpfApp1.Windows.CommonWindows;

namespace WpfApp1.Windows.FabricWindows
{
    /// <summary>
    /// AddProportionGroupDialog.xaml 的互動邏輯
    /// </summary>
    public partial class AddProportionGroupDialog : Window
    {
        private void OnCloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        protected IFabricModule FabricModule { get; } = new FabricModule();

        private Fabric _fabric;
        private Dictionary<int, ObservableCollection<FabricIngredientProportion>> _dictionaryFabricIngredientProportion;
        public AddProportionGroupDialog(Fabric fabric, FabricColor FabricColor, ObservableCollection<FabricColor> fabricColorList)
        {
            InitializeComponent();

            _fabric = fabric;
            LabelFabricName.Content = fabric.FabricName;
            ComboBoxFabricColor.ItemsSource = fabricColorList;
            int selectedIndex = fabricColorList.Select(s => s.ColorNo).ToList().IndexOf(FabricColor.ColorNo);
            ComboBoxFabricColor.SelectedIndex = selectedIndex;
        }

        private void ButtonAddIngredientGroup_Click(object sender, RoutedEventArgs e)
        {
            FabricColor selectedFabricColor = ComboBoxFabricColor.SelectedItem as FabricColor;
            IngredientGroupInfo ingredientGroupInfo = FabricModule.GetIngredientGroupInfo(_fabric.FabricID, selectedFabricColor.ColorNo);
            List<FabricIngredientProportion> fabricIngredientProportion = (DataGridFabricIngredientProportion.ItemsSource as IEnumerable<FabricIngredientProportion>).ToList();
            fabricIngredientProportion.ForEach(f => f.Group = ingredientGroupInfo.Group + 1);
            bool success = FabricModule.InsertFabricIngredientProportions(ingredientGroupInfo.ColorNo, fabricIngredientProportion);

            success.CheckSuccessMessageBox("新增成功!!", "好像有錯誤喔!!");
        }

        private void ButtonChangeYarn_Click(object sender, RoutedEventArgs e)
        {
            int groupNo = 1;
            YarnSelectDialog yarnSelectDialog = new YarnSelectDialog(groupNo)
            {
                Owner = this,
                Left = this.Left + this.Width,
                Top = this.Top,
                DataContext = this
            };
            yarnSelectDialog.ChangeYarnExecute += new YarnSelectDialog.ChangeYarnAction(ChangeYarn);
            yarnSelectDialog.Show();
        }

        private void ChangeYarn(SpecificationYarnPrice specificationYarnPrice, int groupNo)
        {
            int selectedIndex = DataGridFabricIngredientProportion.SelectedIndex;
            //如果紗成分比例沒有選擇一個色的話則會新增一個比例
            //否則會將選擇到的比例

            TextBoxMessageDialog textBoxMessageDialog = new TextBoxMessageDialog
            {
                Left = this.Left + 100,
                Top = this.Top + 130
            };
            decimal proportion = 0;
            if (textBoxMessageDialog.ShowDialog() == true)
            {
                proportion = decimal.Parse(textBoxMessageDialog.TextBoxProportion.Text);
            }
            else
            {
                return;
            }
            FabricIngredientProportion fabricIngredientProportion = FabricModule.GetFabricIngredientProportion(0, proportion, specificationYarnPrice);

            _dictionaryFabricIngredientProportion[groupNo].Add(fabricIngredientProportion);
        }

        private void ComboBoxFabricColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            if (!(comboBox.SelectedItem is FabricColor fabricColor)) return;//未知發生的錯誤，有時間再來查看看什麼問題

            _dictionaryFabricIngredientProportion = new Dictionary<int, ObservableCollection<FabricIngredientProportion>> { { 1, new ObservableCollection<FabricIngredientProportion>() } };

            DataGridFabricIngredientProportion.ItemsSource = _dictionaryFabricIngredientProportion[_dictionaryFabricIngredientProportion.First().Key];
        }
    }
}
