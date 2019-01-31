using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.DataClass.Entity;
using WpfApp1.ViewModel.FabricViewModel;

namespace WpfApp1.Pages.FabricPages
{
    /// <summary>
    /// MalFunctionPage.xaml 的互動邏輯
    /// </summary>
    public partial class MalFunctionPage : Page
    {
        public MalFunctionPage()
        {
            InitializeComponent();
            this.DataContext = new NewMalFunctionViewModel();
        }

        private void CustomerList_KeyUp(object sender, KeyEventArgs e)
        {
            CollectionView itemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView(CustomerList.ItemsSource);

            itemsViewOriginal.Filter = ((o) =>
            {
                if (String.IsNullOrEmpty(CustomerList.Text)) return true;
                else
                {
                    if ((((Customer)o).Name).Contains(CustomerList.Text)) return true;
                    else return false;
                }
            });

            itemsViewOriginal.Refresh();

            // if datasource is a DataView, then apply RowFilter as below and replace above logic with below one
            /* 
             DataView view = (DataView) Cmb.ItemsSource; 
             view.RowFilter = ("Name like '*" + Cmb.Text + "*'"); 
            */
        }

        private void FabricList_KeyUp(object sender, KeyEventArgs e)
        {
            CollectionView itemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView(FabricList.ItemsSource);

            itemsViewOriginal.Filter = ((o) =>
            {
                if (String.IsNullOrEmpty(FabricList.Text)) return true;
                else
                {
                    if ((((Fabric)o).FabricName).Contains(FabricList.Text)) return true;
                    else return false;
                }
            });

            itemsViewOriginal.Refresh();

            // if datasource is a DataView, then apply RowFilter as below and replace above logic with below one
            /* 
             DataView view = (DataView) Cmb.ItemsSource; 
             view.RowFilter = ("Name like '*" + Cmb.Text + "*'"); 
            */
        }
    }
}
