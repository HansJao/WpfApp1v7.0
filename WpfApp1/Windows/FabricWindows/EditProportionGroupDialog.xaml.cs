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
using WpfApp1.Windows.CommonWindows;

namespace WpfApp1.Windows.FabricWindows
{
    /// <summary>
    /// EditProportionGroupDialog.xaml 的互動邏輯
    /// </summary>
    public partial class EditProportionGroupDialog : Window
    {
        private void OnCloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        protected IFabricModule FabricModule { get; } = new FabricModule();
        private Fabric _fabric;
        private Dictionary<int, ObservableCollection<FabricIngredientProportion>> _dictionaryFabricIngredientProportion;
        private int _fabricColorNo;
        public EditProportionGroupDialog(Fabric fabric, FabricColor FabricColor, ObservableCollection<FabricColor> fabricColorList)
        {
            InitializeComponent();

            _fabric = fabric;
            _fabricColorNo = FabricColor.ColorNo;
            LabelFabricName.Content = fabric.FabricName;
            ComboBoxFabricColor.ItemsSource = fabricColorList;

            int selectedIndex = fabricColorList.Select(s => s.ColorNo).ToList().IndexOf(FabricColor.ColorNo);
            ComboBoxFabricColor.SelectedIndex = selectedIndex;
        }

        private YarnSelectDialog _yarnSelectDialog { get; set; }

        private void ButtonChangeYarn_Click(object sender, RoutedEventArgs e)
        {
            int groupNo = Convert.ToInt16(ComboBoxGroup.SelectedItem);
            _yarnSelectDialog = new YarnSelectDialog(groupNo)
            {
                Owner = this,
                Left = this.Left + this.Width,
                Top = this.Top,
                DataContext = this
            };
            _yarnSelectDialog.ChangeYarnExecute += new YarnSelectDialog.ChangeYarnAction(ChangeYarn);
            _yarnSelectDialog.Show();
        }
        private void ChangeYarn(SpecificationYarnPrice specificationYarnPrice, int groupNo)
        {
            int selectedIndex = DataGridFabricIngredientProportion.SelectedIndex;
            //如果紗成分比例沒有選擇一個色的話則會新增一個比例
            //否則會將選擇到的比例
            if (selectedIndex == -1)
            {
                MessageBox.Show("請選擇一筆要修改的資料！");
            }
            else
            {
                var selectedItem = DataGridFabricIngredientProportion.SelectedItem as FabricIngredientProportion;
                FabricIngredientProportion fabricIngredientProportion = FabricModule.GetFabricIngredientProportion(selectedItem.ProportionNo, selectedItem.Proportion, specificationYarnPrice);
                _dictionaryFabricIngredientProportion[groupNo].RemoveAt(selectedIndex);
                _dictionaryFabricIngredientProportion[groupNo].Insert(selectedIndex, fabricIngredientProportion);
                DataGridFabricIngredientProportion.SelectedIndex = selectedIndex += 1;
            }
        }

        private void ButtonEditFabricColor_Click(object sender, RoutedEventArgs e)
        {
            bool success = FabricModule.UpdateFabricProportion((DataGridFabricIngredientProportion.ItemsSource as IEnumerable<FabricIngredientProportion>).ToList());
            success.CheckSuccessMessageBox("更新成功!!", "好像有錯誤喔!!");
        }

        private void ComboBoxGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            if (comboBox.SelectedIndex == -1)
            {
                DataGridFabricIngredientProportion.ItemsSource = _dictionaryFabricIngredientProportion[_dictionaryFabricIngredientProportion.First().Key];
                return;
            }
            int groupNo = Convert.ToInt16(comboBox.SelectedItem);
            if (_yarnSelectDialog != null) _yarnSelectDialog.ChangeGroupNo(groupNo);
            DataGridFabricIngredientProportion.ItemsSource = _dictionaryFabricIngredientProportion[groupNo];
        }

        private void ComboBoxFabricColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxGroup.SelectedIndex = -1;
            ComboBox comboBox = (ComboBox)sender;
            FabricColor fabricColor = comboBox.SelectedItem as FabricColor;
            if (fabricColor == null) return;//未知發生的錯誤，有時間再來查看看什麼問題

            var dictionaryFabricIngredientProportion = FabricModule.GetDictionaryFabricIngredientProportion(new List<int> { fabricColor.ColorNo });

            _dictionaryFabricIngredientProportion = dictionaryFabricIngredientProportion.Count == 0
                                                    ? new Dictionary<int, ObservableCollection<FabricIngredientProportion>> { { 1, new ObservableCollection<FabricIngredientProportion>() } }
                                                    : dictionaryFabricIngredientProportion;

            DataGridFabricIngredientProportion.ItemsSource = _dictionaryFabricIngredientProportion[_dictionaryFabricIngredientProportion.First().Key];
            ComboBoxGroup.ItemsSource = _dictionaryFabricIngredientProportion.Select(s => s.Key);
            ComboBoxGroup.SelectedIndex = 0;
        }

        private void ButtonDeleteFabricIngredientProportions_Click(object sender, RoutedEventArgs e)
        {
            FabricColor fabricColor = ComboBoxFabricColor.SelectedItem as FabricColor;
            int groupNo = Convert.ToInt16(ComboBoxGroup.SelectedItem);
            bool success = FabricModule.DeleteFabricIngredientProportions(fabricColor.ColorNo, groupNo);
            success.CheckSuccessMessageBox("刪除成功!!", "好像有錯誤喔!!");
        }
    }
}
