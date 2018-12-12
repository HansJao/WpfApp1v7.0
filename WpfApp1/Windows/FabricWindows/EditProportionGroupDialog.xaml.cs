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
using WpfApp1.ViewModel.FabricViewModel;

namespace WpfApp1.Windows.FabricWindows
{
    /// <summary>
    /// AddFabricColor.xaml 的互動邏輯
    /// </summary>
    public partial class EditProportionGroupDialog : Window
    {

        protected IFabricModule FabricModule { get; } = new FabricModule();

        private Fabric _fabric;
        private Dictionary<int, ObservableCollection<FabricIngredientProportion>> _dictionaryFabricIngredientProportion;
        private ObservableCollection<FabricColor> _fabricColorList;
        private int _fabricColorNo;
        public EditProportionGroupDialog(Fabric fabric, FabricColor FabricColor, Dictionary<int, ObservableCollection<FabricIngredientProportion>> dictionaryFabricIngredientProportion, ObservableCollection<FabricColor> FabricColorList)
        {
            InitializeComponent();
            _fabric = fabric;
            _dictionaryFabricIngredientProportion = dictionaryFabricIngredientProportion;
            _fabricColorList = FabricColorList;
            _fabricColorNo = FabricColor.ColorNo;

            ComboBoxGroup.ItemsSource = dictionaryFabricIngredientProportion.Select(s => s.Key);
            LabelFabricName.Content = fabric.FabricName;
            DataGridFabricIngredientProportion.ItemsSource = _dictionaryFabricIngredientProportion.Count != 0
                                                            ? _dictionaryFabricIngredientProportion[1]
                                                            : null;
            TextBoxColorName.Text = FabricColor == null
                                    ? string.Empty
                                    : FabricColor.Color;
            ButtonControl(TextBoxColorName);
        }


        private void ButtonAddIngredientGroup_Click(object sender, RoutedEventArgs e)
        {
            IngredientGroupInfo ingredientGroupInfo = FabricModule.GetIngredientGroupInfo(_fabric.FabricID, TextBoxColorName.Text);
            List<FabricIngredientProportion> fabricIngredientProportion = GetFabricIngredientProportions();
            fabricIngredientProportion.ForEach(f => f.Group = ingredientGroupInfo.Group + 1);
            bool success = FabricModule.InsertFabricIngredientProportions(ingredientGroupInfo.ColorNo, fabricIngredientProportion);

            success.CheckSuccessMessageBox("新增成功!!", "好像有錯誤喔!!");
        }

        private YarnSelectDialog _yarnSelectDialog { get; set; }

        private void ButtonChangeYarn_Click(object sender, RoutedEventArgs e)
        {
            int groupNo = Convert.ToInt16(ComboBoxGroup.SelectedItem);
            _yarnSelectDialog = new YarnSelectDialog(groupNo, ref _dictionaryFabricIngredientProportion)
            {
                Owner = this,
                Left = this.Left + this.Width,
                Top = this.Top,
                DataContext = this
            };
            _yarnSelectDialog.ChangeButtonEditFabricColorExecute += new YarnSelectDialog.ChangeButtonEditFabricColorAction(DisableChangeButtonEditFabricColor);
            _yarnSelectDialog.Show();
        }

        private void DisableChangeButtonEditFabricColor()
        {
            ButtonEditFabricColor.IsEnabled = false;
        }
        private void TextBoxColorName_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            ButtonControl(textBox);
        }

        private void ButtonControl(TextBox textBox)
        {
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

            if (DataGridFabricIngredientProportion.Items.Count == 0)
                ButtonEditFabricColor.IsEnabled = false;
        }

        private void ButtonEditFabricColor_Click(object sender, RoutedEventArgs e)
        {
            bool success = FabricModule.UpdateFabricProportion(GetFabricIngredientProportions());

            success.CheckSuccessMessageBox("更新成功!!", "好像有錯誤喔!!");
        }

        private List<FabricIngredientProportion> GetFabricIngredientProportions()
        {
            List<FabricIngredientProportion> fabricIngredientProportions = new List<FabricIngredientProportion>();
            foreach (FabricIngredientProportion item in DataGridFabricIngredientProportion.ItemsSource)
            {
                fabricIngredientProportions.Add(item);
            }
            return fabricIngredientProportions;
        }

        private void ButtonAddFabricColor_Click(object sender, RoutedEventArgs e)
        {
            int colorNo = FabricModule.InsertFabricColor(_fabric.FabricID, TextBoxColorName.Text);
            bool success = FabricModule.InsertFabricIngredientProportions(colorNo, GetFabricIngredientProportions());
            success.CheckSuccessMessageBox("新增成功!!", "好像有錯誤喔!!");
        }

        private void ComboBoxGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            int groupNo = Convert.ToInt16(comboBox.SelectedItem);
            if (_yarnSelectDialog != null) _yarnSelectDialog.ChangeGroupNo(groupNo);
            DataGridFabricIngredientProportion.ItemsSource = _dictionaryFabricIngredientProportion[groupNo];
        }

        private void ButtonDeleteFabricIngredientProportions_Click(object sender, RoutedEventArgs e)
        {
            int groupNo = Convert.ToInt16(ComboBoxGroup.SelectedItem);
            bool success = FabricModule.DeleteFabricIngredientProportions(_fabricColorNo, groupNo);
            success.CheckSuccessMessageBox("刪除成功!!", "好像有錯誤喔!!");
        }
    }
}
