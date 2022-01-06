using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Input;
using WpfApp1.DataClass.Chart;
using WpfApp1.DataClass.TrashSystem;
using WpfApp1.Modules.TrashModule;
using WpfApp1.Modules.TrashModule.Implement;
using WpfApp1.Utility;

namespace WpfApp1.Pages.TrashSystemPages
{
    /// <summary>
    /// ShippingRankChart.xaml 的互動邏輯
    /// </summary>
    public partial class ShippingRankChartPage : Page
    {
        protected ITrashModule TrashModule { get; } = new TrashModule();
        public ShippingRankChartPage()
        {
            InitializeComponent();

            InitializeTextileShippingChart();
            IEnumerable<TrashCustomer> trashCustomers = TrashModule.GetCustomerList();
            ComboBoxCustomer.ItemsSource = trashCustomers;
            DatePickerStartDate.SelectedDate = DateTime.Now.AddMonths(-1);
        }

        private void ButtonTimeIntervalTextileShippingChart_Click(object sender, RoutedEventArgs e)
        {
            if (!(((ComboBox)ComboBoxCustomer).SelectedItem is TrashCustomer trashCustomer))
            {
                mainChart.Series.Clear();
                this.mainChart.Name = "Textile";
                IEnumerable<TrashShipped> trashShippeds = TrashModule.GetTrashShippedQuantitySum(DatePickerStartDate.SelectedDate ?? DateTime.Now, DatePickerEndDate.SelectedDate ?? DateTime.Now);
                IEnumerable<ChartTable> chartTables = trashShippeds.Select(s => new ChartTable { AxisXValue = s.IN_DATE.ToOADate(), AxisYName = s.I_03, AxisYValue = s.Quantity, });
                CreateChartData(chartTables, true);
            }
            else
            {
                IEnumerable<TrashCustomerShipped> trashCustomerShippeds = TrashModule.GetCustomerShippedList(trashCustomer.C_NAME, DatePickerStartDate.SelectedDate ?? DateTime.Now, DatePickerEndDate.SelectedDate ?? DateTime.Now);
                IEnumerable<ChartTable> chartTables = trashCustomerShippeds.Select(s => new ChartTable { AxisXValue = s.IN_DATE.ToOADate(), AxisYName = s.I_03, AxisYValue = s.Quantity, });
                mainChart.Series.Clear();
                CreateChartData(chartTables, true);
                mainChart.ChartAreas[0].RecalculateAxesScale();
                mainChart.Invalidate();
                this.mainChart.Name = "CustomerShipped";
            }
        }

        private void InitializeTextileShippingChart()
        {
            ChartArea ca = new ChartArea("ChartArea1");
            ca.CursorX.IsUserEnabled = true;
            ca.CursorX.IsUserSelectionEnabled = true;
            ca.AxisX.ScaleView.Zoomable = true;
            ca.CursorY.IsUserEnabled = true;
            ca.CursorY.IsUserSelectionEnabled = true;
            ca.AxisY.ScaleView.Zoomable = true;

            this.mainChart.ChartAreas.Add(ca);
            this.mainChart.Name = "Textile";
            Legend legend = new Legend("Legend1")
            {
                IsTextAutoFit = true,
                BackColor = Color.Transparent,
                Docking = Docking.Right,
                TitleFont = new Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold)
            };
            this.mainChart.Legends.Add(legend);

            Legend legend2 = new Legend("Legend2")
            {
                IsTextAutoFit = true,
                BackColor = Color.Transparent,
                Docking = Docking.Right,
                TitleFont = new Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold)
            };
            this.mainChart.Legends.Add(legend2);

            // Add first cell column
            LegendCellColumn firstColumn = new LegendCellColumn
            {
                ColumnType = LegendCellColumnType.SeriesSymbol,
                HeaderText = "顏色",
                HeaderBackColor = Color.WhiteSmoke
            };
            this.mainChart.Legends["Legend2"].CellColumns.Add(firstColumn);

            // Add second cell column
            LegendCellColumn secondColumn = new LegendCellColumn
            {
                ColumnType = LegendCellColumnType.Text,
                HeaderText = "布種名稱",
                Text = "#LEGENDTEXT",
                HeaderBackColor = Color.WhiteSmoke
            };
            this.mainChart.Legends["Legend2"].CellColumns.Add(secondColumn);
            // Add second cell column
            LegendCellColumn totalColumn = new LegendCellColumn
            {
                ColumnType = LegendCellColumnType.Text,
                HeaderText = "Sum",
                HeaderBackColor = Color.WhiteSmoke
            };
            this.mainChart.Legends["Legend1"].CellColumns.Add(totalColumn);

            LegendCellColumn avgColumn = new LegendCellColumn
            {
                HeaderText = "Avg",
                Name = "AvgColumn",
                HeaderBackColor = Color.WhiteSmoke
            };
            this.mainChart.Legends["Legend1"].CellColumns.Add(avgColumn);

            mainChart.MouseMove += new System.Windows.Forms.MouseEventHandler(MainChart_MouseMove);
            mainChart.MouseDown += new System.Windows.Forms.MouseEventHandler(MainChart_MouseDown);
        }

        private void CreateChartData(IEnumerable<ChartTable> chartTables, bool skipCheck)
        {
            var dateArray = chartTables.Select(s => s.AxisXValue).Distinct().ToArray();

            List<ChartData> chartDataList = new List<ChartData>();
            var filterShippingRecord = chartTables.GroupBy(g => g.AxisYName).Select(s => new ChartData
            {
                LegendText = s.Key,
                MaxQuantity = s.Select(ss => ss.AxisYValue).Sum(),
                ChartDetail = s.Select(ss => new ChartDetail { Quantity = ss.AxisYValue, ShippedDate = ss.AxisXValue }).ToList()
            })
            .OrderByDescending(o => o.MaxQuantity)
            .CheckFilter(skipCheck, RankValueStart.Text.ToInt(), RankValueEnd.Text.ToInt() - RankValueStart.Text.ToInt(), w => w.LegendText.Contains(TextBoxTextileName.Text))
            .Select(s => new ChartData
            {
                LegendText = s.LegendText,
                MaxQuantity = s.MaxQuantity,
                ChartDetail = dateArray.Select(ss => new ChartDetail
                {
                    ShippedDate = ss,
                    Quantity = s.ChartDetail.Where(w => w.ShippedDate == ss).Select(sss => sss.Quantity).Sum()
                }).ToList()
            });
            LabelTotalCount.Content = Math.Round(filterShippingRecord.Sum(s => s.ChartDetail.Sum(ss => ss.Quantity)), 1);
            foreach (var item in filterShippingRecord)
            {
                ChartData chartData = new ChartData
                {
                    LegendText = item.LegendText,
                    ChartDetail = new List<ChartDetail>()
                };
                double priviousValue = 0;
                foreach (var eachDate in item.ChartDetail.OrderBy(o => o.ShippedDate))
                {
                    double currentValue = eachDate.Quantity + priviousValue;
                    if (currentValue == priviousValue) continue;
                    chartData.ChartDetail.Add(new ChartDetail { ShippedDate = eachDate.ShippedDate, Quantity = currentValue });
                    priviousValue = currentValue;
                }
                chartData.MaxQuantity = priviousValue;
                chartDataList.Add(chartData);
            }
            mainChart.Legends["Legend1"].CustomItems.Clear();
            foreach (var item in chartDataList)
            {
                Series series = new Series(item.LegendText, 10)
                {
                    ChartArea = "ChartArea1",
                    ChartType = SeriesChartType.Line,
                    IsVisibleInLegend = true,
                    Legend = "Legend2",
                    LegendText = item.LegendText,
                    ToolTip = item.LegendText,
                    LegendToolTip = "test",
                    LabelToolTip = "test123",
                    XValueType = ChartValueType.Date
                };
                LegendItem legendItem = new LegendItem
                {
                    SeriesName = item.LegendText,
                    Name = "Avg",//(Math.Round(item.MaxQuantity / ((DatePickerEndDate.SelectedDate ?? DateTime.Now).ToOADate() - (DatePickerStartDate.SelectedDate ?? DateTime.Now).ToOADate()), 2)).ToString(),
                    MarkerStyle = MarkerStyle.None,
                    ImageStyle = LegendImageStyle.Line
                };
                mainChart.Legends["Legend1"].CustomItems.Add(legendItem);
                mainChart.Legends["Legend1"].CustomItems[chartDataList.IndexOf(item)].Cells.Add(LegendCellType.Text, item.MaxQuantity.ToString(), ContentAlignment.MiddleLeft);
                mainChart.Legends["Legend1"].CustomItems[chartDataList.IndexOf(item)].Cells.Add(LegendCellType.Text, (Math.Round(item.MaxQuantity / ((DatePickerEndDate.SelectedDate ?? DateTime.Now).ToOADate() - (DatePickerStartDate.SelectedDate ?? DateTime.Now).ToOADate()), 2)).ToString(), ContentAlignment.MiddleLeft);
                this.mainChart.Series.Add(series);
                mainChart.Series[item.LegendText].Points.DataBindXY(item.ChartDetail.Select(s => s.ShippedDate).ToArray(), item.ChartDetail.Select(s => s.Quantity).ToArray());
            }
        }

        private void MainChart_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // Call Hit Test Method
            HitTestResult result = mainChart.HitTest(e.X, e.Y);

            // Reset Data Point Attributes
            foreach (var series in mainChart.Series)
            {
                //point.BackSecondaryColor = Color.Black;
                series.BackHatchStyle = ChartHatchStyle.None;
                series.BorderWidth = 1;
                series.ShadowColor = Color.Empty;
                series.ShadowOffset = 0;
                series.IsValueShownAsLabel = false;
            }

            // If a Data Point or a Legend item is selected.
            if (result.ChartElementType == ChartElementType.DataPoint || result.ChartElementType == ChartElementType.LegendItem)
            {
                // Set cursor type 
                this.Cursor = Cursors.Hand;

                // Find selected data point
                Series series = mainChart.Series[result.Series.Name];

                // Set End Gradient Color to White
                series.BackSecondaryColor = Color.White;


                // Increase border width
                series.BorderWidth = 5;
                series.ShadowColor = Color.LightGray;
                series.ShadowOffset = 4;
                series.IsValueShownAsLabel = true;
            }
            else
            {
                // Set default cursor
                this.Cursor = Cursors.Arrow;
            }
        }

        private void MainChart_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            HitTestResult result = mainChart.HitTest(e.X, e.Y);
            #region 點擊右側legendItem 可讓該線條消失
            //if (result != null && result.Object != null)
            //{
            //    // When user hits the LegendItem
            //    if (result.Object is LegendItem)
            //    {
            //        // Legend item result
            //        LegendItem legendItem = (LegendItem)result.Object;

            //        // series item selected
            //        Series selectedSeries = mainChart.Series[legendItem.SeriesName];

            //        if (selectedSeries != null)
            //        {
            //            if (selectedSeries.Enabled)
            //            {
            //                selectedSeries.Enabled = false;
            //                legendItem.Cells[0].ImageTransparentColor = Color.Red;
            //            }
            //            else
            //            {
            //                selectedSeries.Enabled = true;
            //                legendItem.Cells[0].ImageTransparentColor = Color.Red;
            //            }
            //        }
            //    }
            //}
            #endregion

            if (result.ChartElementType != ChartElementType.DataPoint && result.ChartElementType != ChartElementType.LegendItem)
                return;
            if (mainChart.Name != "CustomerShipped")
                mainChart.Series.Clear();
            // If Pie chart is selected
            switch (mainChart.Name)
            {
                case "Textile":
                    IEnumerable<TrashCustomerShipped> trashCustomerShippeds = TrashModule.GetCustomerShippedListByTextileName(result.Series.Name, DatePickerStartDate.SelectedDate ?? DateTime.Now, DatePickerEndDate.SelectedDate ?? DateTime.Now);
                    IEnumerable<ChartTable> chartTables = trashCustomerShippeds.Select(s => new ChartTable { AxisXValue = s.IN_DATE.ToOADate(), AxisYName = s.C_Name, AxisYValue = s.Quantity, });
                    CreateChartData(chartTables, false);
                    this.mainChart.Name = "TextileCustomerDetail";
                    mainChart.ChartAreas[0].RecalculateAxesScale();
                    mainChart.Invalidate();
                    break;
                case "TextileCustomerDetail":
                    IEnumerable<TrashShipped> trashShippeds = TrashModule.GetTrashShippedQuantitySum(DatePickerStartDate.SelectedDate ?? DateTime.Now, DatePickerEndDate.SelectedDate ?? DateTime.Now);
                    IEnumerable<ChartTable> trashShippedsTables = trashShippeds.Select(s => new ChartTable { AxisXValue = s.IN_DATE.ToOADate(), AxisYName = s.I_03, AxisYValue = s.Quantity, });
                    CreateChartData(trashShippedsTables, true);
                    mainChart.ChartAreas[0].RecalculateAxesScale();
                    mainChart.Invalidate();
                    this.mainChart.Name = "Textile";
                    break;
                default:
                    break;
            }
        }

        private void ComboBoxCustomer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        private void ComboBoxCustomer_KeyUp(object sender, KeyEventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            CollectionView itemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView(cmb.ItemsSource);

            itemsViewOriginal.Filter = ((o) =>
            {
                if (string.IsNullOrEmpty(cmb.Text)) return false;
                else
                {
                    if (((TrashCustomer)o).C_NAME.Contains(cmb.Text)) return true;
                    else return false;
                }
            });

            cmb.IsDropDownOpen = true;
            itemsViewOriginal.Refresh();
        }
    }
}
