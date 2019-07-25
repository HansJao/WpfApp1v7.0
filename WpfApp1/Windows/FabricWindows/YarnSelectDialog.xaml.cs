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

        public delegate void ChangeYarnAction(SpecificationYarnPrice specificationYarnPrice);
        public event ChangeYarnAction ChangeYarnExecute;

        public YarnSelectDialog(int groupNo)
        {
            InitializeComponent();
            IEnumerable<YarnSpecification> specificationYarnPrices = FabricModule.GetYarnSpecificationList();
            DataGridYarnSpecification.ItemsSource = specificationYarnPrices;
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
            ChangeYarnExecute(specificationYarnPrice);
        }

        private void DataGridYarnSpecification_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            YarnSpecification yarnSpecification = dataGrid.SelectedItem as YarnSpecification;
            IEnumerable<MerchantYarnPrice> merchantYarnPrices = FabricModule.GetYarnPriceByYarnSpecificationNo(yarnSpecification.YarnSpecificationNo);
            DataGridMerchantYarnPrice.ItemsSource = merchantYarnPrices;
        }
    }
}
