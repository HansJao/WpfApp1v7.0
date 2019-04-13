using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using WpfApp1.DataClass.Fabric;
using WpfApp1.Modules.FabricModule;
using WpfApp1.Modules.FabricModule.Implement;
using WpfApp1.Windows.CommonWindows;
using WpfApp1.Utility;
using WpfApp1.DataClass.Entity.FabricEntity;

namespace WpfApp1.Windows.FabricWindows
{
    /// <summary>
    /// YarnSelectDialog.xaml 的互動邏輯
    /// </summary>
    public partial class YarnSelectDialog : Window
    {
        private void OnCloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        protected IFabricModule FabricModule { get; } = new FabricModule();
        private int _groupNo { get; set; }

        public delegate void ChangeYarnAction(SpecificationYarnPrice specificationYarnPrice, int groupNo);
        public event ChangeYarnAction ChangeYarnExecute;

        public YarnSelectDialog(int groupNo)
        {
            InitializeComponent();
            _groupNo = groupNo;
            IEnumerable<YarnSpecification> specificationYarnPrices = FabricModule.GetYarnSpecificationList();
            DataGridYarnSpecification.ItemsSource = specificationYarnPrices;


        }

        public void ChangeGroupNo(int groupNo)
        {
            _groupNo = groupNo;
        }
        private void ButtonChangeYarn_Click(object sender, RoutedEventArgs e)
        {
            YarnSpecification yarnSpecification = DataGridYarnSpecification.SelectedItem as YarnSpecification;
            MerchantYarnPrice merchantYarnPrice = DataGridMerchantYarnPrice.SelectedItem as MerchantYarnPrice;
            SpecificationYarnPrice specificationYarnPrice = new SpecificationYarnPrice
            {
                YarnPriceNo = merchantYarnPrice.YarnPriceNo,
                Ingredient = yarnSpecification.Ingredient,
                Name = merchantYarnPrice.Name,
                Color = yarnSpecification.Color,
                YarnCount = yarnSpecification.YarnCount,
                YarnMerchant = merchantYarnPrice.YarnMerchant,
                Price = merchantYarnPrice.Price
            };
            ChangeYarnExecute(specificationYarnPrice, _groupNo);
        }

        private ICollectionView cv;
        private void ComboBoxYarnMerchant_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //CheckDataGridFilterCondition();
        }

        private void TextBoxIngredient_TextChanged(object sender, TextChangedEventArgs e)
        {
            //CheckDataGridFilterCondition();
        }

        //private void CheckDataGridFilterCondition()
        //{
        //    string ingredient = TextBoxIngredient.Text.ToUpper();
        //    string color = TextBoxColor.Text.ToUpper();
        //    string yarnCount = TextBoxYarnCount.Text.ToUpper();
        //    int yarnMerchant = ComboBoxYarnMerchant.SelectedIndex == -1 ? 0 : ((MerchantYarnPrice)ComboBoxYarnMerchant.SelectedItem).YarnMerchant;
        //    bool checkNoneFilterCondition = string.IsNullOrEmpty(ingredient) && yarnMerchant == 0 && string.IsNullOrEmpty(color) && string.IsNullOrEmpty(yarnCount);

        //    if (!checkNoneFilterCondition)
        //    {
        //        cv.Filter = o =>
        //        {
        //            /* change to get data row value */
        //            MerchantYarnPrice p = o as MerchantYarnPrice;
        //            bool checkYarnMerchant = yarnMerchant == 0 ? true : yarnMerchant == p.YarnMerchant;
        //            bool checkColor = string.IsNullOrEmpty(color) ? true : p.Color.ToUpper().Contains(color);
        //            bool checkIngredient = string.IsNullOrEmpty(ingredient) ? true : p.Ingredient.ToUpper().Contains(ingredient);
        //            bool checkYarnCount = string.IsNullOrEmpty(yarnCount) ? true : p.YarnCount.ToUpper().Contains(yarnCount);
        //            return (checkYarnMerchant && checkColor && checkIngredient && checkYarnCount);
        //            /* end change to get data row value */
        //        };
        //    }
        //    else
        //    {
        //        cv.Filter = o =>
        //        {
        //            return (true);
        //        };
        //    }
        //}

        private void TextBoxColor_TextChanged(object sender, TextChangedEventArgs e)
        {
            //CheckDataGridFilterCondition();
        }

        private void TextBoxYarnCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            //CheckDataGridFilterCondition();
        }

        private void DataGridYarnSpecification_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            YarnSpecification yarnSpecification = dataGrid.SelectedItem as YarnSpecification;
            IEnumerable<MerchantYarnPrice> merchantYarnPrices = FabricModule.GetYarnPriceByYarnSpecificationNo(yarnSpecification.YarnSpecificationNo);
            DataGridMerchantYarnPrice.ItemsSource = merchantYarnPrices;


            //var comboBoxItems = merchantYarnPrices.Select(s => new MerchantYarnPrice { Name = s.Name }).ToList();
            //comboBoxItems.Insert(0, new MerchantYarnPrice { Name = "全部" });
            //ComboBoxYarnMerchant.ItemsSource = comboBoxItems;
            //cv = CollectionViewSource.GetDefaultView(DataGridMerchantYarnPrice.ItemsSource);
        }
    }
}
