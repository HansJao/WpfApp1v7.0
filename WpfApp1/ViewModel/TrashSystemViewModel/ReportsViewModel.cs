using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfApp1.Command;
using WpfApp1.DataClass.ExcelDataClass;
using WpfApp1.DataClass.Fabric;
using WpfApp1.DataClass.Reports;
using WpfApp1.DataClass.StoreSearch;
using WpfApp1.DataClass.TrashSystem;
using WpfApp1.Modules.ExcelModule;
using WpfApp1.Modules.ExcelModule.Implement;
using WpfApp1.Modules.TrashModule;
using WpfApp1.Modules.TrashModule.Implement;
using WpfApp1.Utility;
using WpfApp1.DataClass.Enumeration;

namespace WpfApp1.ViewModel.TrashSystemViewModel
{
    public class ReportsViewModel : ViewModelBase
    {
        protected IExcelModule ExcelModule { get; } = new ExcelModule();
        protected ITrashModule TrashModule { get; } = new TrashModule();
        public ICommand ExportShippingCheckExecuteClick { get { return new RelayCommand(ExportShippingCheckExecute, CanExecute); } }
        public ICommand ButtonExportExecuteClick { get { return new RelayCommand(ButtonExportExecute, CanExecute); } }
        public ICommand ButtonExportCustomerClick { get { return new RelayCommand(ButtonExportCustomerExecute, CanExecute); } }
        public ICommand ButtonExportShippedListClick { get { return new RelayCommand(ButtonExportShippedListExecute, CanExecute); } }
        public string CustomerName { get; set; }
        public string TextileName { get; set; }
        public DateTime CustomerDatePickerBegin { get; set; } = DateTime.Now.AddYears(-1);
        public DateTime CustomerDatePickerEnd { get; set; } = DateTime.Now;

        #region 出貨區間
        public string TextileShippedIntervalName { get; set; }
        public string IntervalDate { get; set; } = string.Empty;
        public ICommand ButtonIntervalDateClick { get { return new RelayCommand(ButtonIntervalDateExecute, CanExecute); } }
        public ICommand ButtonExportShippedIntervalClick { get { return new RelayCommand(ButtonExportShippedIntervalExecute, CanExecute); } }
        public DateTime TextileShippedIntervalDatePickerBegin { get; set; } = DateTime.Now.AddYears(-1);
        public DateTime TextileShippedIntervalDatePickerEnd { get; set; } = DateTime.Now;
        #endregion

        private void ButtonIntervalDateExecute()
        {
            if (IntervalDate != string.Empty) IntervalDate += ",";
            IntervalDate = IntervalDate + TextileShippedIntervalDatePickerBegin.ToString("yyyy/MM/dd") + "~" + TextileShippedIntervalDatePickerEnd.ToString("yyyy/MM/dd");
            RaisePropertyChanged("IntervalDate");
        }

        private void ButtonExportCustomerExecute()
        {
            IEnumerable<TrashCustomerShipped> trashCustomerShippedList = TrashModule.GetCustomerShippedList(CustomerName, CustomerDatePickerBegin, CustomerDatePickerEnd)
                .GroupBy(g => new { g.C_Name, g.I_03 })
                .Select(s => new TrashCustomerShipped { C_Name = s.Key.C_Name, I_03 = s.Key.I_03, Quantity = s.Sum(item => item.Quantity) });
            IEnumerable<TrashCustomerShipped> trashCustomerShippeds = string.IsNullOrEmpty(TextileName) ? trashCustomerShippedList.OrderBy(o => o.I_03) : trashCustomerShippedList.Where(w => w.I_03.Contains(TextileName)).OrderBy(o => o.I_03);

            IOrderedEnumerable<IGrouping<string, TrashCustomerShipped>> groupCustomerShippeds = trashCustomerShippeds
                                                                                                .OrderByDescending(o => o.Quantity)
                                                                                                .GroupBy(g => g.C_Name)
                                                                                                .OrderByDescending(o => o.Select(s => s.Quantity).Sum());
            ExcelContent excelContent = new ExcelContent
            {
                FileName = string.Concat(TextileName + "客戶出貨紀錄", DateTime.Now.ToString("yyyyMMdd")),
                ExcelSheetContents = new List<ExcelSheetContent>(),
            };
            List<ExcelSheetContent> excelSheetContents = new List<ExcelSheetContent>();

            foreach (IGrouping<string, TrashCustomerShipped> customerShippeds in groupCustomerShippeds)
            {
                ExcelSheetContent excelSheetContent = new ExcelSheetContent();
                excelSheetContent.SheetName = customerShippeds.Key;
                excelSheetContent.ExcelColumnContents = new List<ExcelColumnContent>()
                {
                    new ExcelColumnContent()
                    {
                        CellValue = "客戶名稱",
                        Width = 3000
                    },
                    new ExcelColumnContent()
                    {
                        CellValue = "布種名稱",
                        Width = 5200
                    },
                    new ExcelColumnContent()
                    {
                        CellValue = "重量",
                        Width = 3000
                    }
                };
                excelSheetContent.ExcelRowContents = new List<ExcelRowContent>();
                foreach (TrashCustomerShipped customerShipped in customerShippeds)
                {
                    excelSheetContent.ExcelRowContents.Add(new ExcelRowContent()
                    {
                        ExcelCellContents = new List<ExcelCellContent>()
                        {
                             new ExcelCellContent()
                            {
                                CellValue = customerShipped.C_Name
                            },
                            new ExcelCellContent()
                            {
                                CellValue = customerShipped.I_03
                            },
                            new ExcelCellContent()
                            {
                                CellValue = customerShipped.Quantity.ToString()
                            }
                        }
                    });
                }
                excelSheetContents.Add(excelSheetContent);
            }
            var customerTotals = groupCustomerShippeds.Select(s => new { customerName = s.Key, totalQuantity = s.Sum(sum => sum.Quantity) });
            ExcelSheetContent excelSheetContentTotal = new ExcelSheetContent()
            {
                SheetName = "客戶排名",
                ExcelColumnContents = new List<ExcelColumnContent>()
                    {
                        new ExcelColumnContent()
                        {
                            CellValue = "客戶名稱",
                            Width = 3000
                        },
                        new ExcelColumnContent()
                        {
                            CellValue = "總出貨數",
                            Width = 3000
                        }
                    },
                ExcelRowContents = new List<ExcelRowContent>()
            };

            foreach (var customerTotal in customerTotals)
            {
                List<ExcelRowContent> excelRowContent = new List<ExcelRowContent>()
                {
                    new ExcelRowContent()
                    {
                        ExcelCellContents = new List<ExcelCellContent>()
                        {
                            new ExcelCellContent()
                            {
                                CellValue = customerTotal.customerName
                            },
                            new ExcelCellContent()
                            {
                                CellValue = customerTotal.totalQuantity.ToString()
                            }
                        }
                    }
                };
                excelSheetContentTotal.ExcelRowContents.AddRange(excelRowContent);
            }

            List<ExcelSheetContent> excelSheetContentTotals = new List<ExcelSheetContent>();
            excelSheetContentTotals.Add(excelSheetContentTotal);

            excelContent.ExcelSheetContents.AddRange(excelSheetContentTotals);
            excelContent.ExcelSheetContents.AddRange(excelSheetContents);

            ExcelHelper excelHelper = new ExcelHelper();
            IWorkbook wb = new XSSFWorkbook();
            excelHelper.CreateExcelFile(wb, excelContent);
        }

        private void ButtonExportShippedIntervalExecute()
        {
            Dictionary<string, IEnumerable<IGrouping<string, TrashShipped>>> TrashShippedsDictionary = new Dictionary<string, IEnumerable<IGrouping<string, TrashShipped>>>();
            ExternalDataHelper externalDataHelper = new ExternalDataHelper();
            IEnumerable<TextileNameMapping> textileNameMappings = externalDataHelper.GetTextileNameMappings();
            TextileNameMapping textileNameMapping = textileNameMappings.Where(w => w.Inventory.Contains(TextileShippedIntervalName)).FirstOrDefault();
            List<string> textileNames = new List<string>();
            string[] intervalDates = IntervalDate.Split(',');
            foreach (string intervalDate in intervalDates)
            {
                DateTime dateTimeBegin = DateTime.Parse(intervalDate.Split('~')[0]);
                DateTime dateTimeEnd = DateTime.Parse(intervalDate.Split('~')[1]);
                List<TrashShipped> trashShippedList = TrashModule.GetTrashShippedList(dateTimeBegin, dateTimeEnd).Where(w => w.I_03.Contains(textileNameMapping.Account.FirstOrDefault().Split('*')[0])).ToList();
                TrashShippedsDictionary.Add(intervalDate, trashShippedList.GroupBy(g => g.I_03));
                textileNames.AddRange(trashShippedList.Select(s => s.I_03).Distinct());
            }
            textileNames = textileNames.Distinct().ToList();
            #region 依照庫存管理排序
            string fileName = string.Concat(AppSettingConfig.FilePath(), "\\", AppSettingConfig.StoreManageFileName());
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            IWorkbook Workbook;
            Workbook = new XSSFWorkbook(fileStream);
            ISheet sheet = Workbook.GetSheet(TextileShippedIntervalName);

            List<TextileColorInventory> colorName = new List<TextileColorInventory>();

            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                if (sheet.GetRow(i) == null || sheet.GetRow(i).GetCell(1) == null)
                    break;
                colorName.Add(new TextileColorInventory()
                {
                    ColorName = textileNameMapping.Account.FirstOrDefault().Substring(0, textileNameMapping.Account.FirstOrDefault().Length - 1) + sheet.GetRow(i).GetCell(ExcelEnum.ExcelInventoryColumnIndexEnum.ColorName.ToInt()).StringCellValue.Split('-')[0],
                    CountInventory = sheet.GetRow(i).GetCell(ExcelEnum.ExcelInventoryColumnIndexEnum.CountInventory.ToInt()) == null ? 0 : sheet.GetRow(i).GetCell(ExcelEnum.ExcelInventoryColumnIndexEnum.CountInventory.ToInt()).NumericCellValue.ToInt(),
                });
            }
            IEnumerable<TextileColorInventory> TextileColorInventoryList = colorName.GroupBy(g => g.ColorName).Select(s => new TextileColorInventory() { ColorName = s.Key, CountInventory =s.Sum(ss=>ss.CountInventory)});
            textileNames = textileNames.OrderBy(o => colorName.Select(s => s.ColorName).ToList().IndexOf(o)).ToList();
            #endregion


            ExcelContent excelContent = new ExcelContent
            {
                FileName = string.Concat(FilterText + "出貨區間-", TextileShippedIntervalName),
                ExcelSheetContents = new List<ExcelSheetContent>(),
            };
            List<ExcelSheetContent> excelSheetContents = new List<ExcelSheetContent>();
            ExcelSheetContent excelSheetContent = new ExcelSheetContent();
            excelSheetContent.SheetName = TextileShippedIntervalName;
            excelSheetContent.ExcelColumnContents = new List<ExcelColumnContent>()
            {
                new ExcelColumnContent()
                {
                    CellValue = "布種名稱",
                    Width = 5200
                }
            };
            foreach (string intervalDate in intervalDates)
            {
                excelSheetContent.ExcelColumnContents.Add(new ExcelColumnContent() { CellValue = intervalDate, Width = 5200 });
                excelSheetContent.ExcelColumnContents.Add(new ExcelColumnContent() { CellValue = "疋數", Width = 1400 });
            }
            excelSheetContent.ExcelColumnContents.Add(new ExcelColumnContent() { CellValue = "庫存數量", Width = 2000 });
            excelSheetContent.ExcelRowContents = new List<ExcelRowContent>();

            foreach (string textileName in textileNames)
            {
                ExcelRowContent excelRowContent = new ExcelRowContent()
                {
                    ExcelCellContents = new List<ExcelCellContent>
                    {
                        new ExcelCellContent
                        {
                            CellValue = textileName
                        }
                    }
                };
                foreach (var item in TrashShippedsDictionary.Values)
                {
                    string intervalDateQuantity = item.Where(w => w.Key == textileName).FirstOrDefault() == null ? string.Empty : item.Where(w => w.Key == textileName).FirstOrDefault().Sum(s => s.Quantity).ToString();
                    excelRowContent.ExcelCellContents.Add(new ExcelCellContent()
                    {
                        CellValue = intervalDateQuantity == string.Empty ? "0" : intervalDateQuantity,
                    });
                    excelRowContent.ExcelCellContents.Add(new ExcelCellContent()
                    {
                        CellValue = (intervalDateQuantity == string.Empty ? 0 : Math.Round(intervalDateQuantity.ToDecimal() / 22)).ToString()
                    });
                }
                excelRowContent.ExcelCellContents.Add(new ExcelCellContent()
                {
                    CellValue = TextileColorInventoryList.Where(w => w.ColorName == textileName).Count() == 0 ? "0" : TextileColorInventoryList.Where(w => w.ColorName == textileName).FirstOrDefault().CountInventory.ToString()
                });
                excelSheetContent.ExcelRowContents.Add(excelRowContent);
            }

            excelSheetContents.Add(excelSheetContent);
            excelContent.ExcelSheetContents.AddRange(excelSheetContents);

            ExcelHelper excelHelper = new ExcelHelper();
            IWorkbook wb = new XSSFWorkbook();
            excelHelper.CreateExcelFile(wb, excelContent);
        }
        private void ButtonExportShippedListExecute()
        {
            List<TrashShipped> trashShippedList = TrashModule.GetTrashShippedList(TextileShippedIntervalDatePickerBegin, TextileShippedIntervalDatePickerEnd).Where(w => w.I_03.Contains(TextileShippedIntervalName)).ToList();
            IOrderedEnumerable<IGrouping<string, TrashShipped>> groupTrashShippedList = trashShippedList
                                                                                        .OrderByDescending(o => o.Quantity)
                                                                                        .GroupBy(g => g.I_03)
                                                                                        .OrderByDescending(o => o.Select(s => s.Quantity).Sum());

            ExcelContent excelContent = new ExcelContent
            {
                FileName = string.Concat(TextileShippedIntervalName + "出貨區間", TextileShippedIntervalDatePickerBegin.ToString("yyyyMMdd"), "-", TextileShippedIntervalDatePickerEnd.ToString("yyyyMMdd")),
                ExcelSheetContents = new List<ExcelSheetContent>(),
            };
            List<ExcelSheetContent> excelSheetContents = new List<ExcelSheetContent>();
            ExcelSheetContent excelSheetContent = new ExcelSheetContent();
            excelSheetContent.SheetName = TextileShippedIntervalName;
            excelSheetContent.ExcelColumnContents = new List<ExcelColumnContent>()
            {
                new ExcelColumnContent()
                {
                    CellValue = "布種名稱",
                    Width = 5200
                },
                new ExcelColumnContent()
                {
                    CellValue = "重量",
                    Width = 3000
                }
            };
            excelSheetContent.ExcelRowContents = new List<ExcelRowContent>();
            foreach (IGrouping<string, TrashShipped> trashShippeds in groupTrashShippedList)
            {
                excelSheetContent.ExcelRowContents.Add(new ExcelRowContent()
                {
                    ExcelCellContents = new List<ExcelCellContent>()
                        {
                            new ExcelCellContent()
                            {
                                CellValue = trashShippeds.Key
                            },
                            new ExcelCellContent()
                            {
                                CellValue = trashShippeds.Sum(s=>s.Quantity).ToString()
                            }
                        }
                });
            }
            excelSheetContents.Add(excelSheetContent);
            excelContent.ExcelSheetContents.AddRange(excelSheetContents);

            ExcelHelper excelHelper = new ExcelHelper();
            IWorkbook wb = new XSSFWorkbook();
            excelHelper.CreateExcelFile(wb, excelContent);
        }

        private void CreateCustomerShippedExcelAction(IWorkbook wb, ISheet ws, ICellStyle positionStyle, ref int rowIndex, TrashCustomerShipped storeData)
        {
            ICellStyle estyle = wb.CreateCellStyle();
            estyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Yellow.Index;
            estyle.FillPattern = FillPattern.SolidForeground;

            ICellStyle a2style = wb.CreateCellStyle();
            a2style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Coral.Index;
            a2style.FillPattern = FillPattern.SolidForeground;

            XSSFRow rowTextile = (XSSFRow)ws.CreateRow(rowIndex);
            ExcelHelper.CreateCell(rowTextile, 0, storeData.C_Name, positionStyle);
            ExcelHelper.CreateCell(rowTextile, 1, storeData.I_03, positionStyle);
            ExcelHelper.CreateCell(rowTextile, 2, storeData.Quantity.ToString(), positionStyle);

            rowIndex++;
        }

        public DateTime ShippingCheckDate { get; set; } = DateTime.Now;

        public DateTime DatePickerBegin { get; set; } = DateTime.Now;
        public DateTime DatePickerEnd { get; set; } = DateTime.Now;

        public string FilterText { get; set; }


        private ObservableCollection<TrashShipped> _trashShippeds { get; set; }
        public ObservableCollection<TrashShipped> TrashShippeds
        {
            get { return _trashShippeds; }
            set { _trashShippeds = value; }
        }

        public ReportsViewModel()
        {
            _trashShippeds = new ObservableCollection<TrashShipped>();
        }


        private void ButtonExportExecute()
        {

            _trashShippeds.AddRange(TrashModule.GetTrashShippedQuantitySum(DatePickerBegin, DatePickerEnd));
            #region OleDB
            //string databaseDirectory = AppSettingConfig.DbfFilePath();
            //string sql4 = "SELECT invosub.IN_DATE,invosub.I_01,item.I_03,SUM(invosub.QUANTITY) FROM INVOSUB.dbf invosub " +
            //                "INNER JOIN ITEM.dbf item ON invosub.I_01 = item.I_01 AND invosub.F_01 = item.F_01 " +
            //                "WHERE invosub.IN_DATE Between cDate('" + DatePickerBegin.ToString() + "') and cDate('" + DatePickerEnd.ToString() + "') " +
            //                "GROUP BY invosub.IN_DATE,invosub.I_01,item.I_03 " +
            //                "ORDER BY invosub.IN_DATE,invosub.I_01";
            //DataTable dt4 = GetOleDbDbfDataTable(databaseDirectory, sql4);
            //DataGridInCash_Copy3.ItemsSource = dt4.DefaultView; 
            #endregion
            var originalSources = new List<OriginalSource>();
            foreach (var shipped in _trashShippeds)
            {
                originalSources.Add(new OriginalSource
                {
                    DateTime = shipped.IN_DATE,
                    TextileNo = shipped.I_01,
                    TextileColorName = shipped.I_03,
                    Weight = shipped.Quantity
                });
            }

            var filterText = FilterText;
            var filterOriginalSources = string.IsNullOrEmpty(filterText) == true ? originalSources.Select(s => s.TextileNo).Distinct() : originalSources.Where(w => w.TextileColorName.Contains(filterText)).Select(s => s.TextileNo).Distinct(); ;
            var everyDate = originalSources.Select(s => s.DateTime).Distinct();
            var everyTextileNo = filterOriginalSources;
            var textileDatas = new List<TextileData>();
            foreach (var date in everyDate)
            {
                textileDatas.Add(new TextileData
                {
                    DateTime = date,
                    TextileColors = originalSources.Where(w => w.DateTime == date).Select(s => new TextileColor { TextileNo = s.TextileNo, Weight = s.Weight, TextileColorName = s.TextileColorName }).ToList()
                });
            }

            IWorkbook wb = new XSSFWorkbook();
            ISheet ws = wb.CreateSheet("Report");
            XSSFRow row = (XSSFRow)ws.CreateRow(0);

            var textileNameNumber = 1;
            ExcelHelper.CreateCell(row, 0, "時間", null);
            var priviousWeightList = new Dictionary<string, double>();
            foreach (var item in everyTextileNo)
            {
                ExcelHelper.CreateCell(row, textileNameNumber, originalSources.Where(w => w.TextileNo == item).First().TextileColorName, null);
                priviousWeightList.Add(item, 0);
                textileNameNumber++;
            }
            var rowNumber = 1;
            var priviousYear = textileDatas.First().DateTime.ToString("yyyy");
            foreach (var textiles in textileDatas)
            {
                var columnNumber = 1;
                XSSFRow rowTextile = (XSSFRow)ws.CreateRow(rowNumber);
                ExcelHelper.CreateCell(rowTextile, 0, textiles.DateTime.ToString("yyyy/MM/dd"), null);

                if (textiles.DateTime.ToString("yyyy") != priviousYear)
                {
                    priviousYear = textiles.DateTime.ToString("yyyy");
                    foreach (var item in everyTextileNo)
                    {
                        priviousWeightList[item] = 0;
                    }
                }
                foreach (var textileNo in everyTextileNo)
                {
                    var textileData = textiles.TextileColors.Where(w => w.TextileNo == textileNo);
                    var weight = textileData.Count() == 0 ? priviousWeightList[textileNo] : textileData.First().Weight + priviousWeightList[textileNo];
                    ExcelHelper.CreateCell(rowTextile, columnNumber, weight.ToString(), null);
                    priviousWeightList[textileNo] = weight;
                    columnNumber++;
                }
                rowNumber++;
            }

            FileStream file = new FileStream(string.Concat(AppSettingConfig.ReportsFilePath(), @"\", filterText, "報表", DatePickerBegin.ToString("yyyyMMdd"), "-", DatePickerEnd.ToString("yyyyMMdd"), ".xlsx"), FileMode.Create);//產生檔案
            wb.Write(file);
            file.Close();
        }

        private void ExportShippingCheckExecute()
        {
            List<StoreSearchData<StoreSearchColorDetail>> excelDailyShippedList = ExcelModule.GetExcelDailyShippedList(ShippingCheckDate);
            IEnumerable<TrashShipped> trashShipped = TrashModule.GetTrashShippedQuantitySum(ShippingCheckDate, ShippingCheckDate);
            ExternalDataHelper externalDataHelper = new ExternalDataHelper();
            IEnumerable<TextileNameMapping> textileNameMappings = externalDataHelper.GetTextileNameMappings();

            List<OriginalSource> trashItems = new List<OriginalSource>();
            foreach (TrashShipped shipped in trashShipped)
            {
                trashItems.Add(new OriginalSource
                {
                    DateTime = shipped.IN_DATE,
                    TextileNo = shipped.I_01,
                    TextileColorName = shipped.I_03,
                    Weight = shipped.Quantity
                });
            }
            List<Container> trashPrimary = new List<Container>();
            foreach (OriginalSource trashItem in trashItems)
            {
                int priviousDistance = 10;
                string textileName = string.Empty;
                string textileColor = string.Empty;
                int shippedCount = 0;
                foreach (StoreSearchData<StoreSearchColorDetail> excelDailyShippedItem in excelDailyShippedList)
                {
                    TextileNameMapping textileNameMapping = textileNameMappings.ToList().Find(f => f.Inventory.Contains(excelDailyShippedItem.TextileName)) ?? new TextileNameMapping();
                    foreach (StoreSearchColorDetail colorDetail in excelDailyShippedItem.StoreSearchColorDetails)
                    {
                        string accountMapping = textileNameMapping.Account == null ? string.Empty : textileNameMapping.Account.FirstOrDefault();
                        if (trashItem.TextileColorName == string.Concat(accountMapping.Split('*')[0], colorDetail.ColorName.Split('-')[0]))
                        {
                            textileColor = colorDetail.ColorName;
                            textileName = excelDailyShippedItem.TextileName;
                            priviousDistance = 0;
                            shippedCount = colorDetail.ShippedCount;
                            break;
                        }
                    }
                }
                trashPrimary.Add(new Container()
                {
                    OriginalSource = trashItem,
                    TextileName = textileName,
                    ColorName = textileColor,
                    ShippedCount = shippedCount,
                    Distance = priviousDistance
                });
            }

            List<Container> excelPrimary = new List<Container>();
            foreach (StoreSearchData<StoreSearchColorDetail> excelDailyShippedItem in excelDailyShippedList)
            {
                var priviousDistance = 10;
                var textileName = string.Empty;
                var textileColor = string.Empty;
                foreach (var colorDetail in excelDailyShippedItem.StoreSearchColorDetails)
                {
                    TextileNameMapping textileNameMapping = textileNameMappings.ToList().Find(f => f.Inventory.Contains(excelDailyShippedItem.TextileName)) ?? new TextileNameMapping();
                    OriginalSource originalSource = new OriginalSource();
                    foreach (var trashItem in trashItems)
                    {
                        string accountMapping = textileNameMapping.Account == null ? string.Empty : textileNameMapping.Account.FirstOrDefault();
                        if (trashItem.TextileColorName == string.Concat(accountMapping.Split('*')[0], colorDetail.ColorName.Split('-')[0]))
                        {
                            originalSource.DateTime = trashItem.DateTime;
                            originalSource.TextileColorName = trashItem.TextileColorName;
                            originalSource.Weight = trashItem.Weight;
                            originalSource.TextileNo = trashItem.TextileNo;
                            break;
                        }
                    }
                    excelPrimary.Add(new Container()
                    {
                        OriginalSource = originalSource,
                        TextileName = excelDailyShippedItem.TextileName,
                        ColorName = colorDetail.ColorName,
                        ShippedCount = colorDetail.ShippedCount,
                        Distance = priviousDistance
                    });
                }
            }


            ExcelHelper excelHelper = new ExcelHelper();

            IWorkbook wb = new XSSFWorkbook();

            ICellStyle positionStyle = wb.CreateCellStyle();
            positionStyle.WrapText = true;
            positionStyle.Alignment = HorizontalAlignment.Center;
            positionStyle.VerticalAlignment = VerticalAlignment.Center;

            ICellStyle estyle = wb.CreateCellStyle();
            estyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Yellow.Index;
            estyle.FillPattern = FillPattern.SolidForeground;

            ICellStyle a2style = wb.CreateCellStyle();
            a2style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Coral.Index;
            a2style.FillPattern = FillPattern.SolidForeground;

            List<ExcelRowContent> trashPrimaryExcelRowContent = new List<ExcelRowContent>();
            foreach (var item in trashPrimary.OrderByDescending(t => t.OriginalSource.TextileColorName == null).ThenBy(t => t.TextileName).ThenBy(o => o.ColorName))
            {
                var approximateNumber = item.OriginalSource.Weight / 20;
                var round = Math.Round(approximateNumber, 0, MidpointRounding.AwayFromZero);

                var isEqual = round == item.ShippedCount;

                ExcelRowContent excelCellContents = new ExcelRowContent
                {
                    ExcelCellContents = new List<ExcelCellContent>
                    {
                        new ExcelCellContent{CellValue = item.OriginalSource.TextileColorName, CellStyle = positionStyle},
                        new ExcelCellContent{CellValue = item.OriginalSource.Weight.ToString(),CellStyle = positionStyle },
                        new ExcelCellContent{CellValue =  (approximateNumber).ToString(),CellStyle = positionStyle },
                        new ExcelCellContent{CellValue = item.TextileName,CellStyle = positionStyle },
                        new ExcelCellContent{CellValue = item.ColorName,CellStyle = positionStyle },
                        new ExcelCellContent{CellValue = item.ShippedCount.ToString(), CellStyle = isEqual ? positionStyle : estyle },
                    }
                };
                trashPrimaryExcelRowContent.Add(excelCellContents);
            };

            List<ExcelRowContent> excelPrimaryExcelRowContent = new List<ExcelRowContent>();
            foreach (var item in excelPrimary.OrderByDescending(t => t.OriginalSource.TextileColorName == null).ThenBy(t => t.TextileName).ThenBy(o => o.ColorName))
            {
                var approximateNumber = item.OriginalSource.Weight / 20;
                var round = Math.Round(approximateNumber, 0, MidpointRounding.AwayFromZero);

                var isEqual = round == item.ShippedCount;

                ExcelRowContent excelCellContents = new ExcelRowContent
                {
                    ExcelCellContents = new List<ExcelCellContent>
                    {
                        new ExcelCellContent{CellValue = item.OriginalSource.TextileColorName, CellStyle = positionStyle},
                        new ExcelCellContent{CellValue = item.OriginalSource.Weight.ToString(),CellStyle = positionStyle },
                        new ExcelCellContent{CellValue =  (approximateNumber).ToString(),CellStyle = positionStyle },
                        new ExcelCellContent{CellValue = item.TextileName,CellStyle = positionStyle },
                        new ExcelCellContent{CellValue = item.ColorName,CellStyle = positionStyle },
                        new ExcelCellContent{CellValue = item.ShippedCount.ToString(), CellStyle = isEqual ? positionStyle : estyle }
                    }
                };
                excelPrimaryExcelRowContent.Add(excelCellContents);
            };


            ExcelContent excelContent = new ExcelContent
            {
                FileName = string.Concat("庫存對照清單", ShippingCheckDate.ToString("yyyy-MM-dd")),
                ExcelSheetContents = new List<ExcelSheetContent>
                {
                    new ExcelSheetContent
                    {
                        SheetName = "Super為主",
                        ExcelColumnContents = new List<ExcelColumnContent>
                        {
                            new ExcelColumnContent
                            {
                                CellValue = "Super布種名稱顏色",
                                Width = 6450
                            },
                            new ExcelColumnContent
                            {
                                CellValue = "出貨重量",
                                Width = 2800
                            },
                            new ExcelColumnContent
                            {
                                CellValue = "約略出貨數",
                                Width = 2000
                            },
                            new ExcelColumnContent
                            {
                                CellValue = "布種名稱",
                                Width = 4550
                            },
                            new ExcelColumnContent
                            {
                                CellValue = "顏色",
                                Width = 5550
                            },
                            new ExcelColumnContent
                            {
                                CellValue = "出貨數量",
                                Width = 1850
                            }
                        },
                        ExcelRowContents = trashPrimaryExcelRowContent
                    },
                    new ExcelSheetContent
                    {
                        SheetName = "Excel為主",
                        ExcelColumnContents = new List<ExcelColumnContent>
                        {
                            new ExcelColumnContent
                            {
                                CellValue = "Super布種名稱顏色",
                                Width = 6450
                            },
                            new ExcelColumnContent
                            {
                                CellValue = "出貨重量",
                                Width = 2800
                            },
                            new ExcelColumnContent
                            {
                                CellValue = "約略出貨數",
                                Width = 2000
                            },
                            new ExcelColumnContent
                            {
                                CellValue = "布種名稱",
                                Width = 4550
                            },
                            new ExcelColumnContent
                            {
                                CellValue = "顏色",
                                Width = 5550
                            },
                            new ExcelColumnContent
                            {
                                CellValue = "出貨數量",
                                Width = 1850
                            }
                        },
                        ExcelRowContents = excelPrimaryExcelRowContent
                    }
                }
            };
            excelHelper.CreateExcelFile(wb, excelContent);
        }

        public class Container
        {
            public OriginalSource OriginalSource { get; set; }
            public string TextileName { get; set; }
            public string ColorName { get; set; }
            public int ShippedCount { get; set; }
            public int Distance { get; set; }
        }
        public int LevenshteinDistance(string accountTextileColorName, string excelTextileName, string excelColorName)
        {

            string replaceTextileName = excelTextileName.Replace("采毓", "").Replace("佳隆", "");

            switch (excelTextileName)
            {
                case "搖粒布碼布":
                case "搖粒布配件":
                case "單刷布碼布":
                case "單刷布配件":
                    replaceTextileName = "SP+T2X2配件";
                    break;
                case "CVC大絨布配件碼布":
                case "CVC大絨布配件":
                case "30CVC細絨布碼布":
                case "30CVC細絨布配件":
                    replaceTextileName = "CVC2X2+OP配件";
                    break;
                case "C大絨碼布":
                case "C大絨配件":
                case "30C細絨布碼布":
                case "30C細絨布配件":
                    replaceTextileName = "20C2X2+OP配件";
                    break;
                default:
                    break;
            }

            string excelTextileColorName = string.Concat(replaceTextileName, excelColorName);
            int n = accountTextileColorName.Length;
            int m = excelTextileColorName.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (excelTextileColorName[j - 1] == accountTextileColorName[i - 1]) ? 0 : 1;

                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            return d[n, m];
        }


        #region OleDB
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
        #endregion
    }
}
