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
            TimeIntervalTextileShippingChart();
            DatePickerStartDate.SelectedDate = DateTime.Now.AddMonths(-1);
        }

        private void ButtonTimeIntervalTextileShippingChart_Click(object sender, RoutedEventArgs e)
        {
            while (this.mainChart.Series.Count > 0)
            {
                this.mainChart.Series.RemoveAt(0);
            };
            CreateChartData();
        }
        //以時間區間顯示所有布種顏色出貨紀錄圖表
        private void TimeIntervalTextileShippingChart()
        {
            ChartArea ca = new ChartArea("ChartArea1");
            ca.Area3DStyle.Enable3D = false;//開啟3D
            this.mainChart.ChartAreas.Add(ca);
            Legend lgCPU = new Legend("Legend1")
            {
                IsTextAutoFit = true,
                Docking = Docking.Right,
            };
            this.mainChart.Legends.Add(lgCPU);
            mainChart.MouseMove += new System.Windows.Forms.MouseEventHandler(MainChart_MouseMove);
            mainChart.MouseDown += new System.Windows.Forms.MouseEventHandler(MainChart_MouseDown);
        }

        private void CreateChartData()
        {
            IEnumerable<TrashShipped> shippingRankCharts = TrashModule.GetTrashShippedList(DatePickerStartDate.SelectedDate ?? DateTime.Now, DatePickerEndDate.SelectedDate ?? DateTime.Now);
            var dateArray = shippingRankCharts.Select(s => s.IN_DATE).Distinct().ToArray();

            List<ShippingRecord> shippingRecordList = new List<ShippingRecord>();
            var test = shippingRankCharts.GroupBy(g => g.I_03).Select(s => new ShippingRecord
            {
                TextileName = s.Key,
                MaxQuantity = s.Select(ss => ss.Quantity).Sum(),
                ShippingRecordDetails = s.Select(ss => new ShippingRecordDetail { Quantity = ss.Quantity, ShippedDate = ss.IN_DATE }).ToList()
            })
            .OrderByDescending(o => o.MaxQuantity)
            .Where(w => w.TextileName.Contains(TextBoxTextileName.Text))
            .Skip(RankValueStart.Text.ToInt())
            .Take(RankValueEnd.Text.ToInt() - RankValueStart.Text.ToInt())
            .Select(s => new ShippingRecord
            {
                TextileName = s.TextileName,
                MaxQuantity = s.MaxQuantity,
                ShippingRecordDetails = dateArray.Select(ss => new ShippingRecordDetail
                {
                    ShippedDate = ss.Date,
                    Quantity = s.ShippingRecordDetails.Where(w => w.ShippedDate == ss.Date).Select(sss => sss.Quantity).FirstOrDefault()
                }).ToList()
            });
            foreach (var item in test)
            {
                ShippingRecord shippingRecord = new ShippingRecord
                {
                    TextileName = item.TextileName,
                    ShippingRecordDetails = new List<ShippingRecordDetail>()
                };
                double priviousValue = 0;
                foreach (var eachDate in item.ShippingRecordDetails.OrderBy(o => o.ShippedDate))
                {
                    double currentValue = eachDate.Quantity + priviousValue;
                    shippingRecord.ShippingRecordDetails.Add(new ShippingRecordDetail { ShippedDate = eachDate.ShippedDate, Quantity = currentValue });
                    priviousValue = currentValue;
                }
                shippingRecord.MaxQuantity = priviousValue;
                shippingRecordList.Add(shippingRecord);
            }

            foreach (var item in shippingRecordList)
            {
                Series series = new Series(item.TextileName, 10)
                {
                    ChartArea = "ChartArea1",
                    ChartType = SeriesChartType.Line,
                    IsVisibleInLegend = true,
                    Legend = "Legend1",
                    LegendText = item.TextileName,
                    ToolTip = item.TextileName,
                    LegendToolTip = "test",
                    LabelToolTip = "test123",
                    XValueType = ChartValueType.Date
                };
                this.mainChart.Series.Add(series);
                mainChart.Series[item.TextileName].Points.DataBindXY(item.ShippingRecordDetails.Select(s => s.ShippedDate.ToOADate()).ToArray(), item.ShippingRecordDetails.Select(s => s.Quantity).ToArray());
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
            if (result != null && result.Object != null)
            {
                // When user hits the LegendItem
                if (result.Object is LegendItem)
                {
                    // Legend item result
                    LegendItem legendItem = (LegendItem)result.Object;

                    // series item selected
                    Series selectedSeries = mainChart.Series[legendItem.SeriesName];

                    if (selectedSeries != null)
                    {


                        if (selectedSeries.Enabled)
                        {
                            selectedSeries.Enabled = false;
                            legendItem.Cells[0].ImageTransparentColor = Color.Red;
                        }

                        else
                        {
                            selectedSeries.Enabled = true;
                            legendItem.Cells[0].ImageTransparentColor = Color.Red;
                        }
                    }
                }
            }

        }
    }
}
