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
using System.Windows.Shapes;
using WpfApp1.DataClass.Entity;
using WpfApp1.Modules.FactoryModule;
using WpfApp1.Modules.FactoryModule.Implement;
using WpfApp1.Utility;

namespace WpfApp1.Windows
{
    /// <summary>
    /// FactoryEditDialog.xaml 的互動邏輯
    /// </summary>
    public partial class FactoryEditDialog : Window
    {
        protected IFactoryModule FactoryModule { get; } = new FactoryModule();
        private int _factoryID;
        public FactoryEditDialog(Factory factory)
        {
            InitializeComponent();
            _factoryID = factory.FactoryID;
            LabelFactoryName.Content = factory.Name;
            TextBoxPhoneNumber.Text = factory.PhoneNumber;
            TextBoxCellPhone.Text = factory.CellPhone;
            TextBoxFax.Text = factory.Fax;
            TextBoxAddress.Text = factory.Address;
            TextBoxSort.Text = factory.Sort.ToString();
        }
        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            Factory factory = new Factory()
            {
                FactoryID = _factoryID,
                Name = LabelFactoryName.Content.ToString(),
                PhoneNumber = TextBoxPhoneNumber.Text,
                CellPhone = TextBoxCellPhone.Text,
                Fax = TextBoxFax.Text,
                Address = TextBoxAddress.Text,
                Sort = TextBoxSort.Text.ToInt()
            };
            int count = FactoryModule.UpdateFactory(factory);
            Close();
        }
    }
}
