using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Threading;

namespace WpfApp1.Pages.TrashSystemPages
{
    /// <summary>
    /// ShippingHistoryChartPage.xaml 的互動邏輯
    /// </summary>
    public partial class ShippingHistoryChartPage : Page
    {
        public ShippingHistoryChartPage()
        {
            InitializeComponent();
            //DispatcherTimer dispatcherTimer = new DispatcherTimer();
            //dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            //dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            //dispatcherTimer.Start();

            //設定DataTable的欄位
            SetDataTable();
            //設定Chart Control
            SetChart();

            //this.mainChart.DataSource = dt;
            //this.mainChart.DataBind();//這時候先DataBind()是為了顯示空白的圖表
        }

        DataTable dt = new DataTable();
        /// <summary>
        /// 設定DataTable的欄位
        /// </summary>
        private void SetDataTable()
        {
            dt.Columns.Add("Processor");
            dt.Columns.Add("Memory");

            dt.Columns.Add("Processor2");

            ////這個動作只是為了能夠在一開始顯示圖表,比例就是30筆
            //for (int i = 0; i < 30; i++)
            //{
            //    DataRow dr = dt.NewRow();
            //    dr["Processor"] = 0;
            //    dt.Rows.Add(dr);
            //}
        }

        /// <summary>
        /// 設定Chart Control
        /// </summary>
        private void SetChart()
        {
            ChartArea ca = new ChartArea("ChartArea1");
            ca.Area3DStyle.Enable3D = true;//開啟3D
            this.mainChart.ChartAreas.Add(ca);

            //Processor
            Legend lgCPU = new Legend("Legend1")
            {
                IsTextAutoFit = true,
                Docking = Docking.Bottom
            };
            this.mainChart.Legends.Add(lgCPU);

            Series seCPU = new Series("Series1", 10)
            {
                ChartArea = "ChartArea1",
                ChartType = SeriesChartType.Line,
                IsVisibleInLegend = true,
                Legend = "Legend1",
                LegendText = "CPU123",
                YValueMembers = "Processor"
            };
            this.mainChart.Series.Add(seCPU);

            Series seCPU2 = new Series("Series2", 10)
            {
                ChartArea = "ChartArea1",
                ChartType = SeriesChartType.Line,
                IsVisibleInLegend = true,
                Legend = "Legend1",
                LegendText = "CPU2",
                YValueMembers = "Processor2"
            };
            this.mainChart.Series.Add(seCPU2);
            double[] yValues = { 65.62, 75.54, 60.45, 34.73, 85.42, 55.9, 63.6, 55.1, 77.2 };
            double[] yValues2 = { 76.45, 23.78, 86.45, 30.76, 23.79, 35.67, 89.56, 67.45, 38.98 };
            string[] xValues = { "France", "Canada", "Germany", "USA", "Italy", "Spain", "Russia", "Sweden", "Japan" };
            mainChart.Series["Series1"].Points.DataBindXY(xValues, yValues);
            mainChart.Series["Series2"].Points.DataBindXY(xValues, yValues2);
        }

        PerformanceCounter pcCPU = new PerformanceCounter("Processor", "% Processor Time", "_Total", true);

        //  System.Windows.Threading.DispatcherTimer.Tick handler
        //
        //  Updates the current seconds display and calls
        //  InvalidateRequerySuggested on the CommandManager to force 
        //  the Command to raise the CanExecuteChanged event.
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            //if (dt.Rows.Count > 30)//這動作只是保留顯示30筆即可,不用一直再增加下去
            //{
            //    dt.Rows.RemoveAt(0);
            //}
            DataRow dr = dt.NewRow();

            dr["Processor"] = pcCPU.NextValue();//比例1:1
            dr["Processor2"] = 10;
            dt.Rows.Add(dr);
            //因為DataSource在Form Load就設了,所以這裡只要重新DataBind()就可以更新顯示資料,沒重DataBind之前,新資料不會顯示上去
            this.mainChart.DataBind();

            // Forcing the CommandManager to raise the RequerySuggested event
            CommandManager.InvalidateRequerySuggested();
        }
    }

   
}
