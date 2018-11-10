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
        private ObservableCollection<FabricIngredientProportion> _fabricIngredientProportion;
        public YarnSelectDialog(ref ObservableCollection<FabricIngredientProportion> fabricIngredientProportion)
        {
            InitializeComponent();
            _fabricIngredientProportion = fabricIngredientProportion;
            IEnumerable<MerchantYarnPrice> merchantYarnPrices = FabricModule.GetMerchantYarnPriceList();
            DataGridMerchantYarnPrice.ItemsSource = merchantYarnPrices;
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
                FabricIngredientProportion fabricIngredientProportion = GetFabricIngredientProportion(proportion, merchantYarnPrice);
                _fabricIngredientProportion.Add(fabricIngredientProportion);
            }
            else
            {
                var selectedItem = addFabricColorDialog.DataGridFabricIngredientProportion.SelectedItem as FabricIngredientProportion;
                FabricIngredientProportion fabricIngredientProportion = GetFabricIngredientProportion(selectedItem.Proportion, merchantYarnPrice);
                _fabricIngredientProportion.RemoveAt(selectedIndex);
                _fabricIngredientProportion.Insert(selectedIndex, fabricIngredientProportion);
                addFabricColorDialog.DataGridFabricIngredientProportion.SelectedIndex = selectedIndex += 1;
            }

        }

        private FabricIngredientProportion GetFabricIngredientProportion(float proportion, MerchantYarnPrice merchantYarnPrice)
        {
            FabricIngredientProportion fabricIngredientProportion = new FabricIngredientProportion
            {
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
