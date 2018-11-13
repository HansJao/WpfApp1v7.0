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
using WpfApp1.DataClass.Fabric;
using WpfApp1.Modules.FabricModule;
using WpfApp1.Modules.FabricModule.Implement;
using WpfApp1.Windows.CommonWindows;

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
        public YarnSelectDialog(int groupNo, ref Dictionary<int, ObservableCollection<FabricIngredientProportion>> fabricIngredientProportion)
        {
            InitializeComponent();
            _groupNo = groupNo;
            _fabricIngredientProportion = fabricIngredientProportion;
            IEnumerable<MerchantYarnPrice> merchantYarnPrices = FabricModule.GetMerchantYarnPriceList();
            DataGridMerchantYarnPrice.ItemsSource = merchantYarnPrices;
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
            if (selectedIndex == -1)
            {
                TextBoxMessageDialog textBoxMessageDialog = new TextBoxMessageDialog();
                float proportion = 0;
                if (textBoxMessageDialog.ShowDialog() == true)
                {
                    proportion = float.Parse(textBoxMessageDialog.TextBoxProportion.Text);
                }
                else
                {
                    return;
                }
                FabricIngredientProportion fabricIngredientProportion = GetFabricIngredientProportion(0, proportion, merchantYarnPrice);
                _fabricIngredientProportion[_groupNo].Add(fabricIngredientProportion);
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
        private FabricIngredientProportion GetFabricIngredientProportion(int proportionNo, float proportion, MerchantYarnPrice merchantYarnPrice)
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
    }
}
