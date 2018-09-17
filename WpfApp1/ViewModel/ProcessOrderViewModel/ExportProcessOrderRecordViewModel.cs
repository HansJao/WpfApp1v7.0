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

            #region 更新邏輯
            ////取得每一個sheet內的訂單編號
            //foreach (var item in processOrderStructures)
            //{
            //    string sheetName = item.ProcessOrder.OrderString.Substring(2, 5);
            //    ISheet sheet = templateWorkbook.GetSheet(sheetName) ?? templateWorkbook.CreateSheet(sheetName);
            //    int lastRow = sheet.LastRowNum;

            //    string priviousSheetName = string.Empty, priviousOrderString = string.Empty;

            //    for (int row = 0; row <= lastRow; row++)
            //    {
            //        //為了防止cell null 錯誤
            //        if (lastRow == 0)
            //            break;
            //        IRow dataRow = sheet.GetRow(row);
            //        ICell cell = dataRow.GetCell(0) ?? null;
            //        //判斷Excel是否含有值
            //        if (cell != null && cell.CellType == CellType.String)
            //        {
            //            if (item.ProcessOrder.OrderString == cell.StringCellValue)
            //            {
            //                //判斷是否已存在於字典中 如不在字典中則加入key
            //                if (!keyValuePairs.ContainsKey(sheetName))
            //                {
            //                    keyValuePairs.Add(sheetName, new List<ProcessOrderRecordOrderStringRow>
            //                                                        {
            //                                                            new ProcessOrderRecordOrderStringRow { OrderString = cell.StringCellValue, StartRow = row }
            //                                                        });
            //                }
            //                else
            //                {
            //                    keyValuePairs[sheetName].Add(new ProcessOrderRecordOrderStringRow { OrderString = cell.StringCellValue, StartRow = row });
            //                }
            //            }
            //            //if (!keyValuePairs.ContainsKey(priviousSheetName) || keyValuePairs[priviousSheetName].Count() == 0)
            //            //{
            //            //    priviousSheetName = sheetName;
            //            //    priviousOrderString = cell.StringCellValue;
            //            //    continue;
            //            //}
            //            if (priviousOrderString != cell.StringCellValue && priviousSheetName != string.Empty && keyValuePairs[priviousSheetName].Where(w => w.OrderString == priviousOrderString).Count() > 0)
            //                keyValuePairs[priviousSheetName].Where(w => w.OrderString == priviousOrderString).First().EndRow = row;
            //            priviousSheetName = sheetName;
            //            priviousOrderString = cell.StringCellValue;
            //        }

            //    }
            //} 
            #endregion


            ICellStyle positionStyle = templateWorkbook.CreateCellStyle();
            
            positionStyle.WrapText = true;
            positionStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            positionStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
            positionStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
            positionStyle.FillPattern = FillPattern.SolidForeground;

            ICellStyle colorDetailStyle = templateWorkbook.CreateCellStyle();
            colorDetailStyle.WrapText = true;
            colorDetailStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            colorDetailStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
            colorDetailStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightGreen.Index;
            colorDetailStyle.FillPattern = FillPattern.SolidForeground;

            ICellStyle colorDetailStyle2 = templateWorkbook.CreateCellStyle();
            colorDetailStyle2.WrapText = true;
            colorDetailStyle2.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            colorDetailStyle2.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
            colorDetailStyle2.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LemonChiffon.Index;
            colorDetailStyle2.FillPattern = FillPattern.SolidForeground;

            do
            {
                templateWorkbook.RemoveSheetAt(0);
            } while (templateWorkbook.NumberOfSheets != 0);

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
                    #region 更新邏輯
                    int totalRoqCount = 1 + item.ProcessOrderColorGroups.Count() + item.ProcessOrderColorGroups.Sum(s => s.FactoryShippings.Count() + s.ProcessOrderFlowDateDetails.Count());
                    var currentOrder = keyValuePairs[sheetName].Where(w => w.OrderString == item.ProcessOrder.OrderString).First();
                    sheet.ShiftRows(currentOrder.EndRow, lastRow, totalRoqCount - (currentOrder.EndRow - currentOrder.StartRow));

                    int updateRowNumber = currentOrder.StartRow;
                    IRow dataRow = sheet.CreateRow(updateRowNumber);

                    CreateStringCell(dataRow, 0, item.ProcessOrder.OrderString, positionStyle);

                    CreateStringCell(dataRow, 1, item.ProcessOrder.Fabric, positionStyle);

                    CreateStringCell(dataRow, 2, item.ProcessOrder.Specification, positionStyle);

                    CreateStringCell(dataRow, 3, item.ProcessOrder.ProcessItem, positionStyle);

                    CreateStringCell(dataRow, 4, item.ProcessOrder.Precautions, positionStyle);

                    CreateStringCell(dataRow, 5, item.ProcessOrder.Memo, positionStyle);

                    CreateStringCell(dataRow, 6, item.ProcessOrder.HandFeel, positionStyle);


                    foreach (var orderDetail in item.ProcessOrderColorGroups)
                    {
                        updateRowNumber++;

                        IRow colorDetailRow = sheet.CreateRow(updateRowNumber);
                        CreateStringCell(colorDetailRow, 1, orderDetail.ProcessOrderColorDetail.Color, positionStyle);
                        CreateStringCell(colorDetailRow, 2, orderDetail.ProcessOrderColorDetail.ColorNumber, positionStyle);
                        CreateStringCell(colorDetailRow, 3, orderDetail.ProcessOrderColorDetail.Quantity + "疋", positionStyle);

                        foreach (var orderFlowDate in orderDetail.ProcessOrderFlowDateDetails)
                        {
                            updateRowNumber++;
                            IRow orderFlowDateRow = sheet.CreateRow(updateRowNumber);
                            CreateStringCell(orderFlowDateRow, 2, orderFlowDate.Name, positionStyle);
                            CreateStringCell(orderFlowDateRow, 3, orderFlowDate.InputDate?.ToString("yyyy/MM/dd"), positionStyle);
                            CreateStringCell(orderFlowDateRow, 4, orderFlowDate.CompleteDate?.ToString("yyyy/MM/dd"), positionStyle);
                        }
                        foreach (var factoryShipping in orderDetail.FactoryShippings)
                        {
                            updateRowNumber++;
                            IRow factoryShippingRow = sheet.CreateRow(updateRowNumber);
                            string sentence = string.Concat(factoryShipping.CreateDate.ToString("yyyy/MM/dd"), "載", factoryShipping.Quantity, "疋到", factoryShipping.Name);
                            CreateStringCell(factoryShippingRow, 1, sentence, positionStyle);
                        }
                    }
                    #endregion
                }
                else
                {
                    if (lastRow != 0) lastRow++;
                    IRow dataRow = sheet.CreateRow(lastRow);

                    CreateStringCell(dataRow, 0, item.ProcessOrder.OrderString, positionStyle);

                    CreateStringCell(dataRow, 1, item.ProcessOrder.Fabric, positionStyle);

                    CreateStringCell(dataRow, 2, item.ProcessOrder.Specification, positionStyle);

                    CreateStringCell(dataRow, 3, item.ProcessOrder.ProcessItem, positionStyle);

                    CreateStringCell(dataRow, 4, item.ProcessOrder.Precautions, positionStyle);

                    CreateStringCell(dataRow, 5, item.ProcessOrder.Memo, positionStyle);

                    CreateStringCell(dataRow, 6, item.ProcessOrder.HandFeel, positionStyle);
                    int index = 0;
                    foreach (var orderDetail in item.ProcessOrderColorGroups)
                    {
                        lastRow++;
                        bool isOdd = index % 2 == 1;
                        index++;
                        IRow colorDetailRow = sheet.CreateRow(lastRow);
                        CreateBlankCell(colorDetailRow, 0, isOdd ? colorDetailStyle : colorDetailStyle2);
                        CreateStringCell(colorDetailRow, 1, orderDetail.ProcessOrderColorDetail.Color, isOdd ? colorDetailStyle : colorDetailStyle2);
                        CreateStringCell(colorDetailRow, 2, orderDetail.ProcessOrderColorDetail.ColorNumber, isOdd ? colorDetailStyle : colorDetailStyle2);
                        CreateStringCell(colorDetailRow, 3, orderDetail.ProcessOrderColorDetail.Quantity + "疋", isOdd ? colorDetailStyle : colorDetailStyle2);
                        CreateBlankCell(colorDetailRow, 4, isOdd ? colorDetailStyle : colorDetailStyle2);
                        CreateBlankCell(colorDetailRow, 5, isOdd ? colorDetailStyle : colorDetailStyle2);
                        CreateBlankCell(colorDetailRow, 6, isOdd ? colorDetailStyle : colorDetailStyle2);
                        foreach (var orderFlowDate in orderDetail.ProcessOrderFlowDateDetails)
                        {
                            lastRow++;
                            IRow orderFlowDateRow = sheet.CreateRow(lastRow);
                            CreateBlankCell(orderFlowDateRow, 0, isOdd ? colorDetailStyle : colorDetailStyle2);
                            CreateBlankCell(orderFlowDateRow, 1, isOdd ? colorDetailStyle : colorDetailStyle2);
                            CreateStringCell(orderFlowDateRow, 2, orderFlowDate.Name, isOdd ? colorDetailStyle : colorDetailStyle2);
                            CreateStringCell(orderFlowDateRow, 3, orderFlowDate.InputDate?.ToString("yyyy/MM/dd"), isOdd ? colorDetailStyle : colorDetailStyle2);
                            CreateStringCell(orderFlowDateRow, 4, orderFlowDate.CompleteDate?.ToString("yyyy/MM/dd"), isOdd ? colorDetailStyle : colorDetailStyle2);
                            CreateBlankCell(orderFlowDateRow, 5, isOdd ? colorDetailStyle : colorDetailStyle2);
                            CreateBlankCell(orderFlowDateRow, 6, isOdd ? colorDetailStyle : colorDetailStyle2);
                        }
                        foreach (var factoryShipping in orderDetail.FactoryShippings)
                        {
                            lastRow++;
                            IRow factoryShippingRow = sheet.CreateRow(lastRow);
                            string sentence = string.Concat(factoryShipping.CreateDate.ToString("yyyy/MM/dd"), "載", factoryShipping.Quantity, "疋到", factoryShipping.Name);

                            CreateBlankCell(factoryShippingRow, 0, isOdd ? colorDetailStyle : colorDetailStyle2);
                            CreateStringCell(factoryShippingRow, 1, sentence, isOdd ? colorDetailStyle : colorDetailStyle2);
                            CreateBlankCell(factoryShippingRow, 2, isOdd ? colorDetailStyle : colorDetailStyle2);
                            CreateBlankCell(factoryShippingRow, 3, isOdd ? colorDetailStyle : colorDetailStyle2);
                            CreateBlankCell(factoryShippingRow, 4, isOdd ? colorDetailStyle : colorDetailStyle2);
                            CreateBlankCell(factoryShippingRow, 5, isOdd ? colorDetailStyle : colorDetailStyle2);
                            CreateBlankCell(factoryShippingRow, 6, isOdd ? colorDetailStyle : colorDetailStyle2);
                        }
                    }
                }
            }

            using (FileStream fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
            {
                templateWorkbook.Write(fs);
            }
        }

        public void CreateStringCell(IRow row, int cellIndex, string cellValue, ICellStyle style)
        {
            var cell = row.CreateCell(cellIndex);
            cell.SetCellValue(cellValue);
            cell.SetCellType(CellType.String);
            cell.CellStyle = style;
        }
        public void CreateBlankCell(IRow row, int cellIndex, ICellStyle style)
        {
            var cell = row.CreateCell(cellIndex);
            cell.SetCellType(CellType.Blank);
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
