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
            IEnumerable<TrashShipped> shippingRankCharts = TrashModule.GetTrashShippedList(DateTime.Now.AddDays(-20), DateTime.Now);

            ChartArea ca = new ChartArea("ChartArea1");
            ca.Area3DStyle.Enable3D = false;//開啟3D
            this.mainChart.ChartAreas.Add(ca);
            Legend lgCPU = new Legend("Legend1")
            {
                IsTextAutoFit = true,
                Docking = Docking.Bottom
            };
            this.mainChart.Legends.Add(lgCPU);

            var dateArray = shippingRankCharts.Select(s => s.IN_DATE).Distinct().ToArray();
            foreach (var item in shippingRankCharts.Select(s => s.I_03).Distinct())
            {
                Series seCPU = new Series(item, 10)
                {
                    ChartArea = "ChartArea1",
                    ChartType = SeriesChartType.Line,
                    IsVisibleInLegend = true,
                    Legend = "Legend1",
                    LegendText = item,
                    YValueMembers = "Processor"
                };
                this.mainChart.Series.Add(seCPU);
                List<double> quantityList = new List<double>();
                foreach (var eachDate in dateArray)
                {
                    quantityList.Add(shippingRankCharts.Where(w => w.I_03 == item && w.IN_DATE == eachDate).Select(s => s.Quantity).FirstOrDefault());
                }
                mainChart.Series[item].Points.DataBindXY(dateArray, quantityList);
            }
        }

        private void mainChart_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // Call Hit Test Method
            HitTestResult result = mainChart.HitTest(e.X, e.Y);

            // Reset Data Point Attributes
            foreach (var point in mainChart.Series)
            {
                //point.BackSecondaryColor = Color.Black;
                point.BackHatchStyle = ChartHatchStyle.None;
                point.BorderWidth = 1;
                point.ShadowColor = Color.Empty;
                point.ShadowOffset = 0;
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
            }
            else
            {
                // Set default cursor
                this.Cursor = Cursors.Arrow;
            }
        }
    }
}
