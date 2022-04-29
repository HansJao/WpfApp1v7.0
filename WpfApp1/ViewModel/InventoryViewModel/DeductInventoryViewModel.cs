using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfApp1.Command;
using WpfApp1.DataClass.Enumeration;
using WpfApp1.DataClass.ExcelDataClass;
using WpfApp1.DataClass.Fabric;
using WpfApp1.DataClass.Shipping;
using WpfApp1.Utility;

namespace WpfApp1.ViewModel.InventoryViewModel
{
    public class DeductInventoryViewModel : ViewModelBase
    {
        public ICommand DeductInventoryClick { get { return new RelayCommand(DeductInventoryExecute, CanExecute); } }
        public DateTime ShippingDate { get; set; } = DateTime.Now;
        public string StorageName { get; set; } = AppSettingConfig.StoreManageFileName();
        public DeductInventoryViewModel()
        {

        }
        public void DeductInventoryExecute()
        {
            DirectoryInfo d = new DirectoryInfo(AppSettingConfig.FilePath()); //Assuming Test is your Folder
            IEnumerable<FileInfo> fileInfos = d.GetFiles("*.xlsx").Where(w => w.Name.Contains(string.Concat("出貨", ShippingDate.ToString("yyyyMMdd"), "-")));
            List<List<ShippingSheetStructure>> multiShippingSheetStructures = new List<List<ShippingSheetStructure>>();

            if (MessageBox.Show("是否要執行以下出貨單？\n" + string.Join("\n", fileInfos.Select(s => s.Name).ToArray()), "扣庫存執行確認", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }

            ExternalDataHelper externalDataHelper = new ExternalDataHelper();
            IEnumerable<TextileNameMapping> textileNameMappings = externalDataHelper.GetTextileNameMappings();

            foreach (FileInfo fileInfo in fileInfos)
            {

                IWorkbook shippingWorkbook = null;
                using (FileStream fs = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read))
                {
                    shippingWorkbook = new XSSFWorkbook(fs);
                }
                List<ShippingSheetStructure> ShippingSheetStructures = new List<ShippingSheetStructure>(GetCountShippingSheetSturcture(textileNameMappings, shippingWorkbook));
                multiShippingSheetStructures.Add(ShippingSheetStructures);
            }

            string InventoryfileName = string.Concat(AppSettingConfig.FilePath(), "\\", StorageName);
            IWorkbook inventoryWorkbook = null;

            using (FileStream fs = new FileStream(InventoryfileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                inventoryWorkbook = new XSSFWorkbook(fs);
            }
            int dateColumnNum = -1;
            dateColumnNum = GetCurrentDateColumn(inventoryWorkbook);

            List<List<ShippingSheetStructure>> multiCheckShippingSheetStructure = new List<List<ShippingSheetStructure>>(multiShippingSheetStructures);

            ICellStyle positionStyle = inventoryWorkbook.CreateCellStyle();
            positionStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            positionStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
            foreach (List<ShippingSheetStructure> item in multiShippingSheetStructures)
            {
                //取得每一個客戶的出貨清單
                foreach (ShippingSheetStructure structure in item)
                {
                    //取得客戶出貨的布種
                    foreach (TextileShippingData textileShippingData in structure.TextileShippingDatas)
                    {
                        //取得庫存布種的活頁薄
                        ISheet inventorySheet = inventoryWorkbook.GetSheet(textileShippingData.TextileName);
                        if (inventorySheet == null) break;
                        //取得客戶的出貨布種顏色與數量
                        foreach (ShippingSheetData shippingSheetData in textileShippingData.ShippingSheetDatas)
                        {
                            for (int rowCount = 1; rowCount <= inventorySheet.LastRowNum; rowCount++)
                            {
                                try
                                {
                                    IRow inventoryRow = inventorySheet.GetRow(rowCount);
                                    inventorySheet.ForceFormulaRecalculation = true;
                                    if (inventoryRow == null) break;
                                    ICell inventoryCell = inventoryRow.GetCell(ExcelEnum.ExcelInventoryColumnIndexEnum.ColorName.ToInt());
                                    if (inventoryCell.StringCellValue.Split('-')[0] == shippingSheetData.ColorName)
                                    {
                                        if (shippingSheetData.ShippingNumber == 0)
                                        {
                                            shippingSheetData.ColorName = shippingSheetData.ColorName + "/-/" + inventoryCell.StringCellValue;
                                            break;
                                        }
                                        else
                                        {
                                            shippingSheetData.ColorName = shippingSheetData.ColorName + "/*/" + inventoryCell.StringCellValue;
                                        }

                                        ICell deductCell = inventoryRow.GetCell(dateColumnNum) ?? inventoryRow.CreateCell(dateColumnNum);
                                        // ICell countInventoryCell = inventoryRow.GetCell(ExcelEnum.ExcelInventoryColumnIndexEnum.CountInventory.ToInt());
                                        double totalDeduct = shippingSheetData.ShippingNumber + deductCell.NumericCellValue;
                                        //若是出貨為0，則直接跳過，不寫入庫存
                                        deductCell.SetCellValue(totalDeduct);
                                        deductCell.CellStyle = positionStyle;
                                        break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(string.Concat("----庫存管理---", "\n", inventorySheet.SheetName, "Row：", rowCount, "\n",
                                        "----出貨單----\n", structure.Customer, "\n", textileShippingData.TextileName, "\n", shippingSheetData.ColorName, "\n", ex.StackTrace));
                                    throw ex;
                                }
                            }
                        }
                    }
                }
            }
            ExportCheckDeductShipping(multiShippingSheetStructures);
            using (FileStream fs = new FileStream(InventoryfileName, FileMode.Create, FileAccess.Write))
            {
                inventoryWorkbook.Write(fs);
            }
        }

        /// <summary>
        /// 匯出扣庫存檢查表
        /// </summary>
        private void ExportCheckDeductShipping(List<List<ShippingSheetStructure>> multiShippingSheetStructures)
        {
            IWorkbook wb = new XSSFWorkbook();

            ICellStyle redStyle = wb.CreateCellStyle();
            redStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Red.Index;
            redStyle.FillPattern = FillPattern.SolidForeground;
            redStyle.WrapText = true;
            redStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            redStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
            redStyle.BorderRight = BorderStyle.Thin;
            redStyle.BorderBottom = BorderStyle.Thin;
            redStyle.BorderTop = BorderStyle.Thin;
            redStyle.BorderLeft = BorderStyle.Thin;

            ICellStyle orangeStyle = wb.CreateCellStyle();
            orangeStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Orange.Index;
            orangeStyle.FillPattern = FillPattern.SolidForeground;
            orangeStyle.WrapText = true;
            orangeStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            orangeStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
            orangeStyle.BorderRight = BorderStyle.Thin;
            orangeStyle.BorderBottom = BorderStyle.Thin;
            orangeStyle.BorderTop = BorderStyle.Thin;
            orangeStyle.BorderLeft = BorderStyle.Thin;

            ICellStyle lightGreenStyle = wb.CreateCellStyle();
            lightGreenStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightGreen.Index;
            lightGreenStyle.FillPattern = FillPattern.SolidForeground;
            lightGreenStyle.WrapText = true;
            lightGreenStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            lightGreenStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
            lightGreenStyle.BorderRight = BorderStyle.Thin;
            lightGreenStyle.BorderBottom = BorderStyle.Thin;
            lightGreenStyle.BorderTop = BorderStyle.Thin;
            lightGreenStyle.BorderLeft = BorderStyle.Thin;

            ICellStyle tanStyle = wb.CreateCellStyle();
            tanStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Tan.Index;
            tanStyle.FillPattern = FillPattern.SolidForeground;
            tanStyle.WrapText = true;
            tanStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            tanStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
            tanStyle.BorderRight = BorderStyle.Thin;
            tanStyle.BorderBottom = BorderStyle.Thin;
            tanStyle.BorderTop = BorderStyle.Thin;
            tanStyle.BorderLeft = BorderStyle.Thin;

            ICellStyle grey25PercentStyle = wb.CreateCellStyle();
            grey25PercentStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
            grey25PercentStyle.FillPattern = FillPattern.SolidForeground;
            grey25PercentStyle.WrapText = true;
            grey25PercentStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            grey25PercentStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
            grey25PercentStyle.BorderRight = BorderStyle.Thin;
            grey25PercentStyle.BorderBottom = BorderStyle.Thin;
            grey25PercentStyle.BorderTop = BorderStyle.Thin;
            grey25PercentStyle.BorderLeft = BorderStyle.Thin;

            ICellStyle skyBlueStyle = wb.CreateCellStyle();
            skyBlueStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.SkyBlue.Index;
            skyBlueStyle.FillPattern = FillPattern.SolidForeground;
            skyBlueStyle.WrapText = true;
            skyBlueStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            skyBlueStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
            skyBlueStyle.BorderRight = BorderStyle.Thin;
            skyBlueStyle.BorderBottom = BorderStyle.Thin;
            skyBlueStyle.BorderTop = BorderStyle.Thin;
            skyBlueStyle.BorderLeft = BorderStyle.Thin;

            ICellStyle limeStyle = wb.CreateCellStyle();
            limeStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Lime.Index;
            limeStyle.FillPattern = FillPattern.SolidForeground;
            limeStyle.WrapText = true;
            limeStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            limeStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
            limeStyle.BorderRight = BorderStyle.Thin;
            limeStyle.BorderBottom = BorderStyle.Thin;
            limeStyle.BorderTop = BorderStyle.Thin;
            limeStyle.BorderLeft = BorderStyle.Thin;

            ExcelContent excelContent = new ExcelContent
            {
                FileName = string.Concat("扣庫存檢查表", DateTime.Now.ToString("yyyyMMdd")),
                ExcelSheetContents = new List<ExcelSheetContent>(),
            };
            List<ExcelSheetContent> excelSheetContents = new List<ExcelSheetContent>();
            ExcelSheetContent excelSheetContent = new ExcelSheetContent()
            {
                SheetName = "庫存檢查",
                ExcelColumnContents = new List<ExcelColumnContent>()
                {
                    new ExcelColumnContent()
                    {
                        Width = 3000,
                        CellValue = "客戶名稱",
                    },
                    new ExcelColumnContent()
                    {
                        Width = 5000,
                        CellValue = "布種名稱",
                    },
                    new ExcelColumnContent()
                    {
                        Width = 7500,
                        CellValue = "布種顏色",
                    }
                },
                ExcelRowContents = new List<ExcelRowContent>()

            };
            foreach (List<ShippingSheetStructure> shippingSheetStructures in multiShippingSheetStructures)
            {
                ICellStyle pageStyle = null;
                switch (multiShippingSheetStructures.IndexOf(shippingSheetStructures) + 1)
                {
                    case 1:
                        pageStyle = lightGreenStyle;
                        break;
                    case 2:
                        pageStyle = tanStyle;
                        break;
                    case 3:
                        pageStyle = grey25PercentStyle;
                        break;
                    case 4:
                        pageStyle = skyBlueStyle;
                        break;
                    case 5:
                        pageStyle = limeStyle;
                        break;
                    default:
                        break;
                }
                foreach (ShippingSheetStructure shippingSheetStructure in shippingSheetStructures)
                {
                    excelSheetContent.ExcelRowContents.Add(new ExcelRowContent()
                    {
                        ExcelCellContents = new List<ExcelCellContent>()
                        {
                            new ExcelCellContent()
                            {
                                CellValue =  shippingSheetStructure.Customer,
                                CellStyle = pageStyle
                            },
                            new ExcelCellContent()
                            {
                                CellStyle = pageStyle
                            },
                            new ExcelCellContent()
                            {
                                CellStyle = pageStyle
                            }

                        }
                    });
                    foreach (TextileShippingData textileShippingData in shippingSheetStructure.TextileShippingDatas)
                    {
                        excelSheetContent.ExcelRowContents.Add(new ExcelRowContent()
                        {
                            ExcelCellContents = new List<ExcelCellContent>()
                            {
                                new ExcelCellContent()
                                {
                                    CellValue =  "",
                                    CellStyle = pageStyle
                                },
                                new ExcelCellContent()
                                {
                                    CellValue =  textileShippingData.TextileName,
                                    CellStyle = pageStyle
                                },
                                new ExcelCellContent()
                                {
                                    CellStyle = pageStyle
                                }
                            }
                        });
                        foreach (ShippingSheetData shippingSheetData in textileShippingData.ShippingSheetDatas)
                        {
                            ICellStyle cellStyle = pageStyle;
                            if (shippingSheetData.ColorName.Contains("/-/"))
                            {
                                cellStyle = redStyle;
                            }
                            else if (shippingSheetData.ColorName.Contains("/*/"))
                            {
                            }
                            else
                            {
                                cellStyle = orangeStyle;
                            }
                            excelSheetContent.ExcelRowContents.Add(new ExcelRowContent()
                            {
                                ExcelCellContents = new List<ExcelCellContent>()
                                {
                                    new ExcelCellContent()
                                    {
                                        CellValue =  "",
                                        CellStyle = pageStyle
                                    },
                                          new ExcelCellContent()
                                    {
                                        CellValue =  "",
                                        CellStyle = pageStyle
                                    },
                                    new ExcelCellContent()
                                    {
                                        CellValue =  shippingSheetData.ColorName,
                                        CellStyle = cellStyle
                                    }
                                }
                            });
                        }
                    }
                }
            }
            excelSheetContents.Add(excelSheetContent);
            excelContent.ExcelSheetContents.AddRange(excelSheetContents);
            ExcelHelper excelHelper = new ExcelHelper();
            excelHelper.CreateExcelFile(wb, excelContent);
        }
        /// <summary>
        /// 取得整理過後的ShippingSheetSturcture
        /// Step1 如有配件，則使用布種名稱對應到與庫存管理相同的名字
        /// Step2 判斷顏色是否含有數量數字
        /// Step3 計算實際出貨數量有多少
        /// Step4 判斷有幾行，然後直接跳行, 如遇到配件則不用跳行
        /// Step5 判斷此客戶是否已有加入布種，或是前後布種名稱不一樣時，則加入新的布種, 如布種名稱為空格，則為上一個布種
        /// </summary>
        /// <param name="textileNameMappings"></param>
        /// <param name="shippingWorkbook"></param>
        /// <returns></returns>
        private List<ShippingSheetStructure> GetCountShippingSheetSturcture(IEnumerable<TextileNameMapping> textileNameMappings, IWorkbook shippingWorkbook)
        {
            List<ShippingSheetStructure> ShippingSheetStructures = new List<ShippingSheetStructure>();
            for (int sheetCount = 1; sheetCount < shippingWorkbook.NumberOfSheets; sheetCount++)
            {
                ISheet sheet = shippingWorkbook.GetSheetAt(sheetCount);
                ShippingSheetStructure shippingSheetStructure = new ShippingSheetStructure
                {
                    Customer = sheet.SheetName,
                    TextileShippingDatas = new List<TextileShippingData>()
                };
                for (int rowCount = 6; rowCount <= sheet.LastRowNum; rowCount++)
                {
                    IRow row = sheet.GetRow(rowCount);
                    string textileName = row.GetCell(1).StringCellValue;
                    //如有配件，則使用布種名稱對應到與庫存管理相同的名字
                    if (textileName.Contains("配件"))
                    {
                        if (textileNameMappings.ToList().Find(f => f.Inventory.Contains(textileName)) == null)
                        {

                        }
                        else
                            textileName = textileNameMappings.ToList().Find(f => f.Inventory.Contains(textileName)).Inventory.ToList().Find(f => f.Contains("碼布")) ?? textileName;

                    }
                    try
                    {
                        string colorFullName = row.GetCell(4).StringCellValue;
                        //判斷顏色是否含有數量數字
                        string colorName = colorFullName.Split('-').Count() == 2 ? colorFullName.Split('*')[0] : colorFullName.Split('-')[0];
                        int quantity = colorFullName.Split('-').Count() == 2 ? 1 : colorFullName.Split('-')[1].Split('*')[0].ToInt();
                        //計算實際出貨數量有多少
                        int totalQuantity = CountTotalQuantity(sheet, rowCount);
                        //判斷有幾行，然後直接跳行, 如遇到配件則不用跳行
                        rowCount = colorFullName.Contains("配件") ? rowCount : (decimal)totalQuantity / 7 > 1 ? rowCount + (totalQuantity / 7) : rowCount;

                        //判斷此客戶是否已有加入布種，或是前後布種名稱不一樣時，則加入新的布種, 如布種名稱為空格，則為上一個布種
                        if (shippingSheetStructure.TextileShippingDatas.Count == 0 || (shippingSheetStructure.TextileShippingDatas.Last().TextileName != textileName && textileName != string.Empty))
                        {
                            shippingSheetStructure.TextileShippingDatas.Add(new TextileShippingData()
                            {
                                TextileName = textileName,
                                ShippingSheetDatas = new List<ShippingSheetData>()
                            });
                        }

                        shippingSheetStructure.TextileShippingDatas.Last().ShippingSheetDatas.Add(new ShippingSheetData()
                        {
                            ColorName = colorName,
                            //計算出貨總數
                            ShippingNumber = totalQuantity,
                            //原出貨數
                            CountInventory = quantity,
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Concat("----出貨單---", "\n", sheet.SheetName, "Row：", rowCount, "\n", textileName, "\n", ex.StackTrace));
                        throw ex;
                    }
                }
                ShippingSheetStructures.Add(shippingSheetStructure);
            }
            return ShippingSheetStructures;
        }

        //取得目前日期的列
        private int GetCurrentDateColumn(IWorkbook inventoryWorkbook)
        {
            int dateColumnNum = -1;
            ISheet sheetDate = inventoryWorkbook.GetSheet("仿韓國棉");
            IRow rowDate = sheetDate.GetRow(0);
            string date = DateTime.Now.ToString("MM/dd");
            for (int dateColumn = 5; dateColumn < 14; dateColumn++)
            {
                if (rowDate.GetCell(dateColumn).StringCellValue.Contains(date))
                {
                    dateColumnNum = dateColumn;
                    break;
                }
            }
            return dateColumnNum;
        }

        /// <summary>
        /// 計算實際出貨數量有多少
        /// Step1 判斷是否為配件，配件數量要除0.34 OR  0.3
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        private int CountTotalQuantity(ISheet sheet, int rowCount)
        {
            IRow row = sheet.GetRow(rowCount);
            //配件數量要除0.34 OR  0.3
            if (row.GetCell(4).StringCellValue.Contains("配件"))
            {
                int accessoriesCount = 0;
                for (int quantityCount = 6; quantityCount <= 12; quantityCount++)
                {
                    if (row.GetCell(quantityCount) != null && row.GetCell(quantityCount).CellType == CellType.Numeric)
                        if (row.GetCell(4).StringCellValue.Contains("TC"))
                            accessoriesCount += Math.Round(row.GetCell(quantityCount).NumericCellValue / 0.3, 0).ToInt();
                        else
                            accessoriesCount += Math.Round(row.GetCell(quantityCount).NumericCellValue / 0.34, 0).ToInt();
                    else
                        break;
                }
                return accessoriesCount;
            }
            int quantityCheck = 0;
            //數量確認
            for (int quantityCount = 6; quantityCount <= 12; quantityCount++)
            {
                if (row.GetCell(quantityCount) != null && row.GetCell(quantityCount).CellType == CellType.Numeric)
                    quantityCheck++;
                else
                    break;
            }
            IRow rowNext = sheet.GetRow(rowCount + 1);
            if (rowNext == null)
                return quantityCheck;

            ICell cellNext = rowNext.GetCell(4);

            if (cellNext == null)
                return quantityCheck;

            string colorFullNameNext = cellNext.StringCellValue;

            if (quantityCheck % 7 == 0 && colorFullNameNext == string.Empty)
            {
                rowCount++;
                return quantityCheck + CountTotalQuantity(sheet, rowCount);
            }
            else
            {
                return quantityCheck;
            }
        }
    }
}
