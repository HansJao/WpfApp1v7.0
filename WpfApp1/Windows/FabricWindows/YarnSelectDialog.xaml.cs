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
        protected IFabricModule FabricModule { get; } = new FabricModule();
        private Dictionary<int, ObservableCollection<FabricIngredientProportion>> _fabricIngredientProportion;
        private int _groupNo { get; set; }

        public delegate void ChangeButtonEditFabricColorAction();

        public event ChangeButtonEditFabricColorAction ChangeButtonEditFabricColorExecute;

        public YarnSelectDialog(int groupNo, ref Dictionary<int, ObservableCollection<FabricIngredientProportion>> fabricIngredientProportion)
        {
            InitializeComponent();
            _groupNo = groupNo;
            _fabricIngredientProportion = fabricIngredientProportion;
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
            AddFabricColorDialog addFabricColorDialog = (AddFabricColorDialog)this.DataContext;
            MerchantYarnPrice merchantYarnPrice = DataGridMerchantYarnPrice.SelectedItem as MerchantYarnPrice;
            int selectedIndex = addFabricColorDialog.DataGridFabricIngredientProportion.SelectedIndex;
            //如果紗成分比例沒有選擇一個色的話則會新增一個比例
            //否則會將選擇到的比例
            if (selectedIndex == -1)
            {
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
                //當新增一個比例時,不能用修改
                ChangeButtonEditFabricColorExecute();
                FabricIngredientProportion fabricIngredientProportion = GetFabricIngredientProportion(0, proportion, merchantYarnPrice);
                if (_fabricIngredientProportion.Count == 0)
                {
                    List<FabricIngredientProportion> fabricIngredientProportions = new List<FabricIngredientProportion> { fabricIngredientProportion };
                    _fabricIngredientProportion.Add(_groupNo, new ObservableCollection<FabricIngredientProportion>(fabricIngredientProportions));
                }
                else
                {
                    _fabricIngredientProportion[_groupNo].Add(fabricIngredientProportion);
                }
            }
            else
            {
                var selectedItem = addFabricColorDialog.DataGridFabricIngredientProportion.SelectedItem as FabricIngredientProportion;
                FabricIngredientProportion fabricIngredientProportion = GetFabricIngredientProportion(selectedItem.ProportionNo, selectedItem.Proportion, merchantYarnPrice);
                _fabricIngredientProportion[_groupNo].RemoveAt(selectedIndex);
                _fabricIngredientProportion[_groupNo].Insert(selectedIndex, fabricIngredientProportion);
                addFabricColorDialog.DataGridFabricIngredientProportion.SelectedIndex = selectedIndex += 1;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="proportionNo">布種比例編號,如修改布種成分時Update使用</param>
        /// <param name="proportion">成分比例,同一布種比例應相同</param>
        /// <param name="merchantYarnPrice"></param>
        /// <returns></returns>
        private FabricIngredientProportion GetFabricIngredientProportion(int proportionNo, decimal proportion, MerchantYarnPrice merchantYarnPrice)
        {
            FabricIngredientProportion fabricIngredientProportion = new FabricIngredientProportion
            {
                ProportionNo = proportionNo,
                YarnPriceNo = merchantYarnPrice.YarnPriceNo,
                Name = merchantYarnPrice.Name,
                Color = merchantYarnPrice.Color,
                Ingredient = merchantYarnPrice.Ingredient,
                Price = merchantYarnPrice.Price,
                Proportion = proportion,
                YarnCount = merchantYarnPrice.YarnCount,
                //Group = 3
            };
            return fabricIngredientProportion;
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
            int yarnMerchant = ((MerchantYarnPrice)ComboBoxYarnMerchant.SelectedItem).YarnMerchant;
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
