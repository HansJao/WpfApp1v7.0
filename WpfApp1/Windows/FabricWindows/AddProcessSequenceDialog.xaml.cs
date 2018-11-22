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
using WpfApp1.Modules.FactoryModule;
using WpfApp1.Modules.FactoryModule.Implement;
using WpfApp1.Utility;

namespace WpfApp1.Windows.FabricWindows
{
    /// <summary>
    /// AddProcessSequenceDialog.xaml 的互動邏輯
    /// </summary>
    public partial class AddProcessSequenceDialog : Window
    {
        protected IFactoryModule FactoryModule { get; } = new FactoryModule();
        protected IFabricModule FabricModule { get; } = new FabricModule();
        private Dictionary<int, ObservableCollection<ProcessSequenceDetail>> _processSequenceListGroup { get; set; }
        private Fabric _fabric { get; set; }
        private FabricColor _fabricColor { get; set; }
        public AddProcessSequenceDialog(Fabric fabric, FabricColor fabricColor, Dictionary<int, ObservableCollection<ProcessSequenceDetail>> processSequenceListGroup)
        {
            InitializeComponent();
            _fabric = fabric;
            _fabricColor = fabricColor;
            LabelTextileID.Content = fabric.FabricID;
            LabelTextileName.Content = fabric.FabricName;

            LabelFabricColor.Content = fabricColor.Color;
            _processSequenceListGroup = processSequenceListGroup;
            ComboBoxProcessGroup.ItemsSource = processSequenceListGroup.Keys.ToList();
            ComboBoxFactoryList.ItemsSource = FactoryModule.GetFactoryList();
        }

        private void ComboBoxProcessGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            DataGridProcessSequence.Items.Clear();
            DataGridProcessSequence.ItemsSource = _processSequenceListGroup[Convert.ToInt16(comboBox.SelectedItem)];
        }

        private void ButtonInsertProcessSequence_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxProcessGroup.SelectedIndex != -1)
            {
                int selectedGroup = Convert.ToInt16(ComboBoxProcessGroup.SelectedItem);
                _processSequenceListGroup[selectedGroup].Add(new ProcessSequenceDetail
                {
                    FabricID = _fabric.FabricID,
                    ColorNo = _fabricColor.ColorNo,
                    ProcessItem = (DataClass.Enumeration.ProcessItem)ComboBoxProcessItem.SelectedItem,
                    WorkPay = TextBoxWorkPay.Text.ToInt(),
                    Loss = Convert.ToDecimal(TextBoxLoss.Text),
                });
            }
            else
            {
                Factory factory = ComboBoxFactoryList.SelectedItem as Factory;
                DataGridProcessSequence.Items.Add(new ProcessSequenceDetail
                {
                    FactoryID = factory.FactoryID,
                    Name = factory.Name,
                    FabricID = _fabric.FabricID,
                    ColorNo = _fabricColor.ColorNo,
                    ProcessItem = (DataClass.Enumeration.ProcessItem)ComboBoxProcessItem.SelectedItem,
                    WorkPay = TextBoxWorkPay.Text.ToInt(),
                    Loss = Convert.ToDecimal(TextBoxLoss.Text),
                });
            }
        }

        private void ButtonNewProcessSequence_Click(object sender, RoutedEventArgs e)
        {
            List<ProcessSequenceDetail> processSequenceDetails = new List<ProcessSequenceDetail>();
            foreach (ProcessSequenceDetail processSequenceDetail in DataGridProcessSequence.Items)
            {
                processSequenceDetails.Add(processSequenceDetail);
            }
            if (CheckBoxIsThisColor.IsChecked == true)
            {
                if (ComboBoxProcessGroup.SelectedIndex == -1)
                {
                    List<int> sequenceNoList = FabricModule.InsertProcessSequence(processSequenceDetails);
                    var processSequenceColorMapping = sequenceNoList.Select(s => new ProcessSequenceColorMapping { ColorNo = _fabricColor.ColorNo, SequenceNo = s });
                    bool success = FabricModule.InsertProcessSequenceColorMapping(processSequenceColorMapping);
                }
                else
                {
                    var processSequenceColorMapping = processSequenceDetails.Select(s => new ProcessSequenceColorMapping { ColorNo = _fabricColor.ColorNo, SequenceNo = s.SequenceNo });
                    bool success = FabricModule.InsertProcessSequenceColorMapping(processSequenceColorMapping);
                }
            }
            else
            {
                List<int> sequenceNoList = FabricModule.InsertProcessSequence(processSequenceDetails);
                bool success = sequenceNoList.Count() == processSequenceDetails.Count();
            }
        }

        private void ButtonDeleteProcessSequence_Click(object sender, RoutedEventArgs e)
        {
            List<ProcessSequenceDetail> processSequenceDetails = new List<ProcessSequenceDetail>();
            foreach (ProcessSequenceDetail processSequenceDetail in DataGridProcessSequence.Items)
            {
                processSequenceDetails.Add(processSequenceDetail);
            }
            int group = processSequenceDetails.First().Group;
            bool success =  FabricModule.DeleteProcessSequence(_fabricColor.ColorNo, group);
        }
    }
}
