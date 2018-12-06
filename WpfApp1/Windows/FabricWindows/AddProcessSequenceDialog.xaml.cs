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
        private ObservableCollection<ProcessSequenceDetail> _processSequenceDetails { get; set; } = new ObservableCollection<ProcessSequenceDetail>();
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
            var processSequenceDetails = FabricModule.GetProcessSequencesByFabricID(_fabric.FabricID)
                                                     .GroupBy(g => g.Group)
                                                     .ToDictionary(g => g.Key, g => new ObservableCollection<ProcessSequenceDetail>(g.OrderBy(o => o.Order).ToList()));
            foreach (var item in processSequenceDetails)
            {
                if (!_processSequenceListGroup.Keys.Contains(item.Key))
                    _processSequenceListGroup.Add(item.Key, item.Value);
            }

            ComboBoxProcessGroup.ItemsSource = _processSequenceListGroup.Keys.ToList();
            ComboBoxFactoryList.ItemsSource = FactoryModule.GetFactoryList();
            DataGridProcessSequence.ItemsSource = _processSequenceDetails;
        }

        private void ComboBoxProcessGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            _processSequenceDetails = new ObservableCollection<ProcessSequenceDetail>(_processSequenceListGroup[Convert.ToInt16(comboBox.SelectedItem)]);
            DataGridProcessSequence.ItemsSource = _processSequenceDetails;
            CheckBoxIsThisColor.IsChecked = true;
            CheckBoxIsThisColor.IsEnabled = false;
        }

        private void ButtonInsertProcessSequence_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxProcessGroup.SelectedIndex != -1)
            {
                int selectedGroup = Convert.ToInt16(ComboBoxProcessGroup.SelectedItem);
                Factory factory = ComboBoxFactoryList.SelectedItem as Factory;
                _processSequenceDetails.Add(new ProcessSequenceDetail
                {
                    FactoryID = factory.FactoryID,
                    FabricID = _fabric.FabricID,
                    Name = factory.Name,
                    ColorNo = _fabricColor.ColorNo,
                    ProcessItem = (DataClass.Enumeration.ProcessItem)ComboBoxProcessItem.SelectedItem,
                    WorkPay = TextBoxWorkPay.Text.ToInt(),
                    Loss = Convert.ToDecimal(TextBoxLoss.Text),
                });
            }
            else
            {
                Factory factory = ComboBoxFactoryList.SelectedItem as Factory;
                _processSequenceDetails.Add(new ProcessSequenceDetail
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
            List<ProcessSequenceDetail> processSequenceDetails = _processSequenceDetails.ToList();
            bool isInProcessSequenceColorMapping = FabricModule.CheckIsInProcessSequenceColorMapping(_processSequenceDetails);
            if (isInProcessSequenceColorMapping)
            {
                MessageBox.Show("已存於在資料庫!!");
                return;
            }
            bool success = false;
            if (CheckBoxIsThisColor.IsChecked == true)
            {
                if (ComboBoxProcessGroup.SelectedIndex == -1)
                {
                    List<int> sequenceNoList = FabricModule.InsertProcessSequence(processSequenceDetails);
                    var processSequenceColorMapping = sequenceNoList.Select(s => new ProcessSequenceColorMapping { ColorNo = _fabricColor.ColorNo, SequenceNo = s });
                    success = FabricModule.InsertProcessSequenceColorMapping(processSequenceColorMapping);
                }
                else
                {
                    var processSequenceColorMapping = processSequenceDetails.Select(s => new ProcessSequenceColorMapping { ColorNo = _fabricColor.ColorNo, SequenceNo = s.SequenceNo });
                    success = FabricModule.InsertProcessSequenceColorMapping(processSequenceColorMapping);
                }
            }
            else
            {
                List<int> sequenceNoList = FabricModule.InsertProcessSequence(processSequenceDetails);
                success = sequenceNoList.Count() == processSequenceDetails.Count();
            }

            success.CheckSuccessMessageBox("新增成功!!", "新增失敗!!");
        }

        private void ButtonDeleteProcessSequence_Click(object sender, RoutedEventArgs e)
        {
            List<ProcessSequenceDetail> processSequenceDetails = _processSequenceDetails.ToList();

            int group = processSequenceDetails.First().Group;
            bool success = FabricModule.DeleteProcessSequence(_fabricColor.ColorNo, group, processSequenceDetails.Select(s => s.SequenceNo));

            success.CheckSuccessMessageBox("刪除成功!!", "刪除失敗!!");
        }

        private void ButtonEditProcessSequence_Click(object sender, RoutedEventArgs e)
        {
            ProcessSequenceDetail processSequenceDetail = DataGridProcessSequence.SelectedItem as ProcessSequenceDetail;
            bool success = FabricModule.EditProcessSequence(processSequenceDetail);
            success.CheckSuccessMessageBox("修改成功!!", "修改失敗!!");
        }
    }
}
