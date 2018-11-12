﻿using System;
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
            ButtonControl(TextBoxColorName);
        }

        private void ButtonAddIngredientGroup_Click(object sender, RoutedEventArgs e)
        {
            IngredientGroupInfo ingredientGroupInfo = FabricModule.GetIngredientGroupInfo(_fabric.FabricID, TextBoxColorName.Text);
            List<FabricIngredientProportion> fabricIngredientProportion = GetFabricIngredientProportions();
            fabricIngredientProportion.ForEach(f => f.Group = ingredientGroupInfo.Group + 1);
            bool success = FabricModule.InsertFabricIngredientProportions(ingredientGroupInfo.ColorNo, fabricIngredientProportion);
            if (success == false)
            {
                MessageBox.Show("好像有錯誤喔!!");
            }
            else
            {
                MessageBox.Show("新增成功!!");
            }
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
            if (success == false)
            {
                MessageBox.Show("好像有錯誤喔!!");
            }
            else
            {
                MessageBox.Show("更新成功!!");
            }
        }

        private List<FabricIngredientProportion> GetFabricIngredientProportions()
        {
            List<FabricIngredientProportion> fabricIngredientProportions = new List<FabricIngredientProportion>();
            foreach (FabricIngredientProportion item in DataGridFabricIngredientProportion.Items)
            {
                fabricIngredientProportions.Add(item);
            }
            return fabricIngredientProportions;
        }

        private void ButtonAddFabricColor_Click(object sender, RoutedEventArgs e)
        {
            int colorNo = FabricModule.InsertFabricColor(_fabric.FabricID, TextBoxColorName.Text);
            bool success = FabricModule.InsertFabricIngredientProportions(colorNo, GetFabricIngredientProportions());
            if (success == false)
            {
                MessageBox.Show("好像有錯誤喔!!");
            }
            else
            {
                MessageBox.Show("新增成功!!");
            }

        }
    }
}
