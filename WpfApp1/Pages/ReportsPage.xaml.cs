using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
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
using WpfApp1.Adapter.MSSQL;
using WpfApp1.DataClass.ExcelDataClass;
using WpfApp1.DataClass.Reports;
using WpfApp1.DataClass.StoreSearch;
using WpfApp1.DataClass.TrashSystem;
using WpfApp1.Modules.ExcelModule;
using WpfApp1.Modules.ExcelModule.Implement;
using WpfApp1.Modules.TrashModule;
using WpfApp1.Modules.TrashModule.Implement;
using WpfApp1.Utility;
using WpfApp1.ViewModel.InventoryViewModel;

namespace WpfApp1.Pages
{
    /// <summary>
    /// ReportsPage.xaml 的互動邏輯
    /// </summary>
    public partial class ReportsPage : Page
    {
        public ReportsPage()
        {
            InitializeComponent();
            //string databaseDirectory = AppSettingConfig.DbfFilePath();
            //string database2 = "ITEM.dbf";
            //string sql2 = "SELECT * FROM " + database2;
            //DataTable dt2 = GetOleDbDbfDataTable(databaseDirectory, sql2);
        }

        //private void ButtonExport_Click(object sender, RoutedEventArgs e)
        //{
        //    string databaseDirectory = AppSettingConfig.DbfFilePath();
        //    string sql4 = "SELECT invosub.IN_DATE,invosub.I_01,item.I_03,SUM(invosub.QUANTITY) FROM INVOSUB.dbf invosub " +
        //                    "INNER JOIN ITEM.dbf item ON invosub.I_01 = item.I_01 AND invosub.F_01 = item.F_01 " +
        //                    "WHERE invosub.IN_DATE Between cDate('" + DatePickerBegin.ToString() + "') and cDate('" + DatePickerEnd.ToString() + "') " +
        //                    "GROUP BY invosub.IN_DATE,invosub.I_01,item.I_03 " +
        //                    "ORDER BY invosub.IN_DATE,invosub.I_01";
        //    DataTable dt4 = GetOleDbDbfDataTable(databaseDirectory, sql4);
        //    DataGridInCash_Copy3.ItemsSource = dt4.DefaultView;
        //    var originalSources = new List<OriginalSource>();
        //    foreach (DataRow dataRow in dt4.Rows)
        //    {
        //        originalSources.Add(new OriginalSource
        //        {
        //            DateTime = DateTime.Parse(dataRow[0].ToString()),
        //            TextileNo = Convert.ToInt32(dataRow[1]),
        //            TextileColorName = dataRow[2].ToString(),
        //            Weight = Convert.ToInt64(dataRow[3])
        //        });
        //    }

        //    var filterText = TextBoxFilter.Text;
        //    var filterOriginalSources = string.IsNullOrEmpty(filterText) == true ? originalSources.Select(s => s.TextileNo).Distinct() : originalSources.Where(w => w.TextileColorName.Contains(filterText)).Select(s => s.TextileNo).Distinct(); ;
        //    var everyDate = originalSources.Select(s => s.DateTime).Distinct();
        //    var everyTextileNo = filterOriginalSources;
        //    var textileDatas = new List<TextileData>();
        //    foreach (var date in everyDate)
        //    {
        //        textileDatas.Add(new TextileData
        //        {
        //            DateTime = date,
        //            TextileColors = originalSources.Where(w => w.DateTime == date).Select(s => new TextileColor { TextileNo = s.TextileNo, Weight = s.Weight, TextileColorName = s.TextileColorName }).ToList()
        //        });
        //    }

        //    IWorkbook wb = new XSSFWorkbook();
        //    ISheet ws = wb.CreateSheet("Report");
        //    XSSFRow row = (XSSFRow)ws.CreateRow(0);

        //    var textileNameNumber = 1;
        //    CreateCell(row, 0, "時間", null);
        //    var priviousWeightList = new Dictionary<int, double>();
        //    foreach (var item in everyTextileNo)
        //    {
        //        CreateCell(row, textileNameNumber, originalSources.Where(w => w.TextileNo == item).First().TextileColorName, null);
        //        priviousWeightList.Add(item, 0);
        //        textileNameNumber++;
        //    }
        //    var rowNumber = 1;
        //    var priviousYear = textileDatas.First().DateTime.ToString("yyyy");
        //    foreach (var textiles in textileDatas)
        //    {
        //        var columnNumber = 1;
        //        XSSFRow rowTextile = (XSSFRow)ws.CreateRow(rowNumber);
        //        CreateCell(rowTextile, 0, textiles.DateTime.ToString("yyyy/MM/dd"), null);

        //        if (textiles.DateTime.ToString("yyyy") != priviousYear)
        //        {
        //            priviousYear = textiles.DateTime.ToString("yyyy");
        //            foreach (var item in everyTextileNo)
        //            {
        //                priviousWeightList[item] = 0;
        //            }
        //        }
        //        foreach (var textileNo in everyTextileNo)
        //        {
        //            var textileData = textiles.TextileColors.Where(w => w.TextileNo == textileNo);
        //            var weight = textileData.Count() == 0 ? priviousWeightList[textileNo] : textileData.First().Weight + priviousWeightList[textileNo];
        //            CreateCell(rowTextile, columnNumber, weight.ToString(), null);
        //            priviousWeightList[textileNo] = weight;
        //            columnNumber++;
        //        }
        //        rowNumber++;
        //    }

        //    FileStream file = new FileStream(string.Concat(AppSettingConfig.ReportsFilePath(), @"\", filterText, "報表", DatePickerBegin.SelectedDate?.ToString("yyyyMMdd"), "-", DatePickerEnd.SelectedDate?.ToString("yyyyMMdd"), ".xlsx"), FileMode.Create);//產生檔案
        //    wb.Write(file);
        //    file.Close();
        //}

        //private void CreateCell(XSSFRow row, int cellIndex, string cellValue, ICellStyle style)
        //{
        //    var cell = row.CreateCell(cellIndex);
        //    cell.SetCellValue(cellValue);
        //    cell.CellStyle = style;
        //}

        //public static OleDbConnection OleDbDbfOpenConn(string DatabaseDirectory)
        //{
        //    string cnstr = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DatabaseDirectory + "; Extended Properties=dBASE IV;");
        //    OleDbConnection icn = new OleDbConnection();
        //    icn.ConnectionString = cnstr;
        //    if (icn.State == ConnectionState.Open) icn.Close();
        //    icn.Open();
        //    return icn;
        //}

        //public static DataTable GetOleDbDbfDataTable(string DatabaseDirectory, string OleDbString)
        //{
        //    DataTable myDataTable = new DataTable();
        //    OleDbConnection icn = OleDbDbfOpenConn(DatabaseDirectory);
        //    OleDbDataAdapter da = new OleDbDataAdapter(OleDbString, icn);
        //    DataSet ds = new DataSet();
        //    ds.Clear();
        //    da.Fill(ds);
        //    myDataTable = ds.Tables[0];
        //    if (icn.State == ConnectionState.Open) icn.Close();
        //    return myDataTable;
        //}
    }
}
