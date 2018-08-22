using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.Entity;
using WpfApp1.Modules.Process;
using WpfApp1.Modules.Process.Implement;
using WpfApp1.Utility;

namespace WpfApp1.ViewModel.ProcessOrderViewModel
{
    public class ExportProcessOrderRecordViewModel : ViewModelBase
    {
        protected IProcessModule ProcessModule { get; } = new ProcessModule();

        public ExportProcessOrderRecordViewModel()
        {
            //ExportProcessOrderRecordExecute();
            var dateTime = ReadProcessOrderRecordDate();
            IEnumerable<ProcessOrder> processOrders = ProcessModule.GetProcessOrderByDate(dateTime);
            // TestWrite();
        }

        void TestWrite()
        {
            var tempPath = string.Concat(AppSettingConfig.ProcessOrderRecordDateFilePath(), "加工訂單紀錄表.xlsx");

            IWorkbook templateWorkbook;
            using (FileStream fs = new FileStream(tempPath, FileMode.Open, FileAccess.Read))
            {
                templateWorkbook = new XSSFWorkbook(fs);
            }

            string sheetName = "201808";
            ISheet sheet = templateWorkbook.GetSheet(sheetName) ?? templateWorkbook.CreateSheet(sheetName);
            IRow dataRow = sheet.GetRow(4) ?? sheet.CreateRow(4);
            ICell cell = dataRow.GetCell(1) ?? dataRow.CreateCell(1);
            cell.SetCellValue("foo");

            sheet.ShiftRows(0, 10, 10);
            using (FileStream fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
            {
                templateWorkbook.Write(fs);
            }

            //using (FileStream fs = File.Open(tempPath, FileMode.Create, FileAccess.Read))
            //{
            //    //把xls文件读入workbook变量里，之后就可以关闭了
            //    IWorkbook wk = new XSSFWorkbook(fs);
            //    ISheet wsheet = wk.GetSheet("201808");
            //    XSSFRow row = (XSSFRow)wsheet.GetRow(0);
            //    row.Height = 5000;
            //    row.GetCell(0).SetCellValue("123456987");
            //    wsheet.SetColumnWidth(0, 3000);
            //    wsheet.SetColumnWidth(1, 3150);
            //    wsheet.SetColumnWidth(2, 1700);
            //    wsheet.SetColumnWidth(4, 1700);

            //    //ICellStyle positionStyle = wk.CreateCellStyle();
            //    //positionStyle.WrapText = true;
            //    //positionStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            //    //positionStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;

            //    //ExcelHelper.CreateCell(row, 0, "布種", positionStyle);
            //    //ExcelHelper.CreateCell(row, 1, "顏色", positionStyle);
            //    //ExcelHelper.CreateCell(row, 2, "織廠", positionStyle);
            //    //ExcelHelper.CreateCell(row, 3, "整理", positionStyle);
            //    //ExcelHelper.CreateCell(row, 4, "出貨量", positionStyle);
            //    //ExcelHelper.CreateCell(row, 5, "計算庫存量", positionStyle);
            //    //ExcelHelper.CreateCell(row, 6, "時間", positionStyle);
            //    wk.Write(fs);
            //    fs.Close();
            //}
            //建立Excel 2003檔案
            //IWorkbook wb = new XSSFWorkbook();
            //ISheet ws = wb.GetSheet("201808");
            //XSSFRow row = (XSSFRow)ws.CreateRow(0);
            //row.Height = 440;

            //ws.SetColumnWidth(0, 3000);
            //ws.SetColumnWidth(1, 3150);
            //ws.SetColumnWidth(2, 1700);
            //ws.SetColumnWidth(4, 1700);

            //ICellStyle positionStyle = wb.CreateCellStyle();
            //positionStyle.WrapText = true;
            //positionStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            //positionStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;

            //ExcelHelper.CreateCell(row, 0, "布種", positionStyle);
            //ExcelHelper.CreateCell(row, 1, "顏色", positionStyle);
            //ExcelHelper.CreateCell(row, 2, "織廠", positionStyle);
            //ExcelHelper.CreateCell(row, 3, "整理", positionStyle);
            //ExcelHelper.CreateCell(row, 4, "出貨量", positionStyle);
            //ExcelHelper.CreateCell(row, 5, "計算庫存量", positionStyle);
            //ExcelHelper.CreateCell(row, 6, "時間", positionStyle);
            //FileStream file = new FileStream(tempPath, FileMode.Open);//產生檔案
            //wb.Write(file);
            //file.Close();
        }

        public DateTime ReadProcessOrderRecordDate()
        {
            var shippingCacheFileName = string.Concat(AppSettingConfig.ProcessOrderRecordDateFilePath(), AppSettingConfig.ProcessOrderRecordDateFileName());
            //this code segment read data from the file.
            FileStream fs2 = new FileStream(shippingCacheFileName, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(fs2);
            var dateTime = reader.ReadToEnd();
            var exportDate = Convert.ToDateTime(dateTime);
            reader.Close();
            return exportDate;
        }

        public void ExportProcessOrderRecordExecute()
        {
            var existsFileName = Directory.GetFiles(AppSettingConfig.ProcessOrderRecordDateFilePath(), "*.txt").Select(System.IO.Path.GetFileName);
            var processOrderRecordFileName = string.Concat(AppSettingConfig.ProcessOrderRecordDateFilePath(), AppSettingConfig.ProcessOrderRecordDateFileName());
            // Create a new file 
            File.WriteAllText(processOrderRecordFileName, DateTime.Now.ToString());
        }
    }
}
