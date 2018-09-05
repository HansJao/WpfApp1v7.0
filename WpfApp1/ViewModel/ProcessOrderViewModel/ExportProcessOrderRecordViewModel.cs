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
using WpfApp1.DataClass.ProcessOrder;
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
            var processOrderRecordText = ReadProcessOrderRecordText();
            List<ProcessOrderStructure> processOrderStructures = ProcessModule.GetNewOrEditProcessOrderStructures(processOrderRecordText.DateTime, processOrderRecordText.OrderNos);
            //SetProcessOrderRecordDateExecute();

            TestWrite(processOrderStructures);
        }

        void TestWrite(List<ProcessOrderStructure> processOrderStructures)
        {
            var tempPath = string.Concat(AppSettingConfig.ProcessOrderRecordDateFilePath(), "加工訂單紀錄表.xlsx");

            IWorkbook templateWorkbook;
            using (FileStream fs = new FileStream(tempPath, FileMode.Open, FileAccess.Read))
            {
                templateWorkbook = new XSSFWorkbook(fs);
            }

            var containOrderString = new List<string>();
            Dictionary<string, List<ProcessOrderRecordOrderStringRow>> keyValuePairs = new Dictionary<string, List<ProcessOrderRecordOrderStringRow>>();

            //取得每一個sheet內的訂單編號
            foreach (var item in processOrderStructures)
            {
                string sheetName = item.ProcessOrder.OrderString.Substring(2, 5);
                ISheet sheet = templateWorkbook.GetSheet(sheetName) ?? templateWorkbook.CreateSheet(sheetName);
                int lastRow = sheet.LastRowNum;
                if (!keyValuePairs.ContainsKey(sheetName))
                    for (int row = 0; row <= lastRow; row++)
                    {
                        //為了防止cell null 錯誤
                        if (lastRow == 0)
                            break;
                        IRow dataRow = sheet.GetRow(row);
                        ICell cell = dataRow.GetCell(0) ?? null;
                        //判斷Excel是否含有值
                        if (cell != null && cell.CellType == CellType.String)
                        {
                            //判斷是否已存在於字典中 如不在字典中則加入key
                            if (!keyValuePairs.ContainsKey(sheetName))
                            {
                                keyValuePairs.Add(sheetName, new List<ProcessOrderRecordOrderStringRow>
                                                                    {
                                                                        new ProcessOrderRecordOrderStringRow { OrderString = cell.StringCellValue, RowNumber = row }
                                                                    });
                            }
                            else
                            {
                                keyValuePairs[sheetName].Add(new ProcessOrderRecordOrderStringRow { OrderString = cell.StringCellValue, RowNumber = row });
                            }
                        }
                    }
            }


            ICellStyle positionStyle = templateWorkbook.CreateCellStyle();
            positionStyle.WrapText = true;
            positionStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            positionStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;

            foreach (var item in processOrderStructures)
            {
                string sheetName = item.ProcessOrder.OrderString.Substring(2, 5);
                ISheet sheet = templateWorkbook.GetSheet(sheetName) ?? templateWorkbook.CreateSheet(sheetName);
                sheet.SetColumnWidth(0, 3300);
                sheet.SetColumnWidth(1, 6300);
                sheet.SetColumnWidth(2, 4100);
                sheet.SetColumnWidth(3, 3700);
                sheet.SetColumnWidth(4, 5000);
                sheet.SetColumnWidth(5, 6000);
                sheet.SetColumnWidth(6, 2500);
                int lastRow = sheet.LastRowNum;
                //判斷是否已存在於字典中 如果存在則是使用更新
                if (keyValuePairs.ContainsKey(sheetName) && keyValuePairs[sheetName].Where(w => w.OrderString == item.ProcessOrder.OrderString).Count() > 0)
                {

                }
                else
                {
                    lastRow++;
                    IRow dataRow = sheet.CreateRow(lastRow);

                    CreateStringCell(dataRow, 0, item.ProcessOrder.OrderString, positionStyle);

                    CreateStringCell(dataRow, 1, item.ProcessOrder.Fabric, positionStyle);

                    CreateStringCell(dataRow, 2, item.ProcessOrder.Specification, positionStyle);

                    CreateStringCell(dataRow, 3, item.ProcessOrder.ProcessItem, positionStyle);

                    CreateStringCell(dataRow, 4, item.ProcessOrder.Precautions, positionStyle);

                    CreateStringCell(dataRow, 5, item.ProcessOrder.Memo, positionStyle);

                    CreateStringCell(dataRow, 6, item.ProcessOrder.HandFeel, positionStyle);


                    foreach (var orderDetail in item.ProcessOrderColorGroups)
                    {
                        lastRow++;
                        IRow colorDetailRow = sheet.CreateRow(lastRow);
                        CreateStringCell(colorDetailRow, 1, orderDetail.ProcessOrderColorDetail.Color, positionStyle);
                        CreateStringCell(colorDetailRow, 2, orderDetail.ProcessOrderColorDetail.ColorNumber, positionStyle);
                        CreateStringCell(colorDetailRow, 3, orderDetail.ProcessOrderColorDetail.Quantity + "疋", positionStyle);

                        foreach (var orderFlowDate in orderDetail.ProcessOrderFlowDateDetails)
                        {
                            lastRow++;
                            IRow orderFlowDateRow = sheet.CreateRow(lastRow);
                            CreateStringCell(orderFlowDateRow, 2, orderFlowDate.Name, positionStyle);
                            CreateStringCell(orderFlowDateRow, 3, orderFlowDate.InputDate?.ToString("yyyy/MM/dd"), positionStyle);
                            CreateStringCell(orderFlowDateRow, 4, orderFlowDate.CompleteDate?.ToString("yyyy/MM/dd"), positionStyle);
                        }
                        foreach (var factoryShipping in orderDetail.FactoryShippings)
                        {
                            lastRow++;
                            IRow factoryShippingRow = sheet.CreateRow(lastRow);
                            string sentence = string.Concat(factoryShipping.CreateDate.ToString("yyyy/MM/dd"), "載", factoryShipping.Quantity, "疋到", factoryShipping.Name);
                            CreateStringCell(factoryShippingRow, 1, sentence, positionStyle);
                        }
                    }

                }
            }


            //string sheetName = "201808";
            //ISheet sheet = templateWorkbook.GetSheet(sheetName) ?? templateWorkbook.CreateSheet(sheetName);
            //IRow dataRow = sheet.GetRow(4) ?? sheet.CreateRow(4);
            //ICell cell = dataRow.GetCell(1) ?? dataRow.CreateCell(1);
            //cell.SetCellValue("foo");

            //sheet.ShiftRows(0, 10, 10);
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

        public void CreateStringCell(IRow row, int cellIndex, string cellValue, ICellStyle style)
        {
            var cell = row.CreateCell(cellIndex);
            cell.SetCellValue(cellValue);
            cell.SetCellType(CellType.String);
            cell.CellStyle = style;
        }


        public ProcessOrderRecordText ReadProcessOrderRecordText()
        {
            var shippingCacheFileName = string.Concat(AppSettingConfig.ProcessOrderRecordDateFilePath(), AppSettingConfig.ProcessOrderRecordDateFileName());
            //this code segment read data from the file.
            FileStream fs2 = new FileStream(shippingCacheFileName, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(fs2);
            var text = reader.ReadToEnd();
            var processOrderRecordText = JsonConvert.DeserializeObject<ProcessOrderRecordText>(text);
            reader.Close();
            return processOrderRecordText;
        }

        public void SetProcessOrderRecordDateExecute()
        {
            var existsFileName = Directory.GetFiles(AppSettingConfig.ProcessOrderRecordDateFilePath(), "*.txt").Select(System.IO.Path.GetFileName);
            var processOrderRecordFileName = string.Concat(AppSettingConfig.ProcessOrderRecordDateFilePath(), AppSettingConfig.ProcessOrderRecordDateFileName());
            ProcessOrderRecordText processOrderRecordText = new ProcessOrderRecordText
            {
                DateTime = DateTime.Now,
                OrderNos = null
            };
            // Create a new file 
            File.WriteAllText(processOrderRecordFileName, JsonConvert.SerializeObject(processOrderRecordText));
        }
    }
}
