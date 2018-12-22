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

        public delegate void ChangeYarnAction(MerchantYarnPrice merchantYarnPrice, int groupNo);
        public event ChangeYarnAction ChangeYarnExecute;

        public YarnSelectDialog(int groupNo)
        {
            InitializeComponent();
            _groupNo = groupNo;
            IEnumerable<MerchantYarnPrice> merchantYarnPrices = FabricModule.GetMerchantYarnPriceList();
            DataGridMerchantYarnPrice.ItemsSource = merchantYarnPrices;
            var comboBoxItems = merchantYarnPrices.Select(s => new MerchantYarnPrice { YarnMerchant = s.YarnMerchant, Name = s.Name }).Distinct(d => d.YarnMerchant).ToList();
            comboBoxItems.Insert(0, new MerchantYarnPrice { YarnMerchant = 0, Name = "全部" });
            ComboBoxYarnMerchant.ItemsSource = comboBoxItems;
            cv = CollectionViewSource.GetDefaultView(DataGridMerchantYarnPrice.ItemsSource);
        }

        public void ChangeGroupNo(int groupNo)
        {
            _groupNo = groupNo;
        }
        private void ButtonChangeYarn_Click(object sender, RoutedEventArgs e)
        {
            MerchantYarnPrice merchantYarnPrice = DataGridMerchantYarnPrice.SelectedItem as MerchantYarnPrice;
            ChangeYarnExecute(merchantYarnPrice, _groupNo);
        }
       
        private ICollectionView cv;
        private void ComboBoxYarnMerchant_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckDataGridFilterCondition();
        }

        private void TextBoxIngredient_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckDataGridFilterCondition();
        }

        private void CheckDataGridFilterCondition()
        {
            string ingredient = TextBoxIngredient.Text.ToUpper();
            string color = TextBoxColor.Text.ToUpper();
            int yarnMerchant = ComboBoxYarnMerchant.SelectedIndex == -1 ? 0 : ((MerchantYarnPrice)ComboBoxYarnMerchant.SelectedItem).YarnMerchant;
            bool checkNoneFilterCondition = string.IsNullOrEmpty(ingredient) && yarnMerchant == 0 && string.IsNullOrEmpty(color);

            if (!checkNoneFilterCondition)
            {
                cv.Filter = o =>
                {
                    /* change to get data row value */
                    MerchantYarnPrice p = o as MerchantYarnPrice;
                    bool checkYarnMerchant = yarnMerchant == 0 ? true : yarnMerchant == p.YarnMerchant;
                    bool checkColor = string.IsNullOrEmpty(color) ? true : p.Color.ToUpper().Contains(color);
                    bool checkIngredient = string.IsNullOrEmpty(ingredient) ? true : p.Ingredient.ToUpper().Contains(ingredient);

                    return (checkYarnMerchant && checkColor && checkIngredient);
                    /* end change to get data row value */
                };
            }
            else
            {
                cv.Filter = o =>
                {
                    return (true);
                };
            }
        }

        private void TextBoxColor_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckDataGridFilterCondition();
        }
    }
}
