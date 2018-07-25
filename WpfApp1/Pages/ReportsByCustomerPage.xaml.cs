﻿using System;
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

namespace WpfApp1.Pages
{
    /// <summary>
    /// ReportsByCustomerPage.xaml 的互動邏輯
    /// </summary>
    public partial class ReportsByCustomerPage : Page
    {
        public ReportsByCustomerPage()
        {
            InitializeComponent();
            string databaseDirectory = AppSettingConfig.DbfFilePath();

            string invo = "INVO.dbf";
            string invoSql = "SELECT * FROM " + invo;
            DataTable invoDt = GetOleDbDbfDataTable(databaseDirectory, invoSql);
            DataGridInvo.ItemsSource = invoDt.DefaultView;

            string invosub = "INVOSUB.dbf";
            string invosubSql = "SELECT * FROM " + invosub;
            DataTable invosubDt = GetOleDbDbfDataTable(databaseDirectory, invosubSql);
            DataGridInvoSub.ItemsSource = invosubDt.DefaultView;

            string item = "ITEM.dbf";
            string itemSql = "SELECT * FROM " + item;
            DataTable itemDt = GetOleDbDbfDataTable(databaseDirectory, itemSql);
            DataGridItem.ItemsSource = itemDt.DefaultView;

            string cust = "CUST.dbf";
            string custoSql = "SELECT * FROM " + cust;
            DataTable custDt = GetOleDbDbfDataTable(databaseDirectory, custoSql);
            DataGridCust.ItemsSource = custDt.DefaultView;
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
