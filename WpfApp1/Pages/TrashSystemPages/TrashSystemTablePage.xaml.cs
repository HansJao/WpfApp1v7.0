using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
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

namespace WpfApp1.Pages.TrashSystemPages
{
    /// <summary>
    /// ReportsByCustomerPage.xaml 的互動邏輯
    /// </summary>
    public partial class TrashSystemTablePage : Page
    {
        public TrashSystemTablePage()
        {
            InitializeComponent();
            string databaseDirectory = AppSettingConfig.DbfFilePath();

            string invo = "INVO.dbf";
            string invoSql = "SELECT TOP 100 * FROM " + invo;
            DataTable invoDt = GetOleDbDbfDataTable(databaseDirectory, invoSql);
            DataGridInvo.ItemsSource = invoDt.DefaultView;

            string invosub = "INVOSUB.dbf";
            string invosubSql = "SELECT TOP 100 * FROM " + invosub;//+ " where IN_DATE Between cDate('" + DateTime.Now.AddDays(-1).ToString() + "') and cDate('" + DateTime.Now.ToString() + "') ";
            DataTable invosubDt = GetOleDbDbfDataTable(databaseDirectory, invosubSql);
            DataGridInvoSub.ItemsSource = invosubDt.DefaultView;

            string item = "ITEM.dbf";
            string itemSql = "SELECT TOP 100 * FROM " + item;
            DataTable itemDt = GetOleDbDbfDataTable(databaseDirectory, itemSql);
            DataGridItem.ItemsSource = itemDt.DefaultView;

            string cust = "CUST.dbf";
            string custoSql = "SELECT TOP 100 * FROM " + cust;
            DataTable custDt = GetOleDbDbfDataTable(databaseDirectory, custoSql);
            DataGridCust.ItemsSource = custDt.DefaultView;

            string purc = "PURC.dbf";
            string purcSql = "SELECT TOP 100 * FROM " + purc;
            DataTable purcDt = GetOleDbDbfDataTable(databaseDirectory, purcSql);
            DataGridPurc.ItemsSource = purcDt.DefaultView;

            string purcSub = "PURCSUB.dbf";
            string purcSubSql = "SELECT TOP 100 * FROM " + purcSub;
            DataTable purcSubDt = GetOleDbDbfDataTable(databaseDirectory, purcSubSql);
            DataGridPurcsub.ItemsSource = purcSubDt.DefaultView;

            string inCash = "IN_CASH.dbf";
            string inCashSql = "SELECT * FROM " + inCash;
            DataTable inCashDt = GetOleDbDbfDataTable(databaseDirectory, inCashSql);
            DataGridInCash.ItemsSource = inCashDt.DefaultView;
        }

        private void ButtonExport_Click(object sender, RoutedEventArgs e)
        {

        }

        public static OleDbConnection OleDbDbfOpenConn(string DatabaseDirectory)
        {
            string cnstr = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DatabaseDirectory + "; Extended Properties=dBASE IV;");
            OleDbConnection icn = new OleDbConnection();
            icn.ConnectionString = cnstr;
            if (icn.State == ConnectionState.Open) icn.Close();
            icn.Open();
            return icn;
        }

        public static DataTable GetOleDbDbfDataTable(string DatabaseDirectory, string OleDbString)
        {
            DataTable myDataTable = new DataTable();
            OleDbConnection icn = OleDbDbfOpenConn(DatabaseDirectory);
            OleDbDataAdapter da = new OleDbDataAdapter(OleDbString, icn);
            DataSet ds = new DataSet();
            ds.Clear();
            da.Fill(ds);
            myDataTable = ds.Tables[0];
            if (icn.State == ConnectionState.Open) icn.Close();
            return myDataTable;
        }
    }
}
