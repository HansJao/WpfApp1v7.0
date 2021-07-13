using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using WpfApp1.DataClass.Enumeration;
using WpfApp1.DataClass.ExcelDataClass;
using WpfApp1.DataClass.Shipping;
using WpfApp1.Modules.CustomerModule;
using WpfApp1.Modules.CustomerModule.Implement;
using WpfApp1.Utility;

namespace WpfApp1.Pages
{
    /// <summary>
    /// ShippingPage.xaml 的互動邏輯
    /// </summary>
    public partial class ShippingPage : Page
    {
        public ObservableCollection<ShippingSheetData> ShippingSheetDatas = new ObservableCollection<ShippingSheetData>();

        protected ICustomerModule CustomerModule { get; } = new CustomerModule();
        //ObservableCollection<string> TextileList = new ObservableCollection<string>();
        List<ShippingSheetStructure> ShippingSheetStructure = new List<ShippingSheetStructure>();

        private IWorkbook _workbook { get; set; }
        public ShippingPage()
        {
            InitializeComponent();
            //IWorkbook _workbook = null;  //新建IWorkbook對象  
            string fileName = string.Concat(AppSettingConfig.FilePath(), "\\", AppSettingConfig.StoreManageFileName());
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            _workbook = new XSSFWorkbook(fileStream);  //xlsx數據讀入workbook
            List<string> textileList = new List<string>();
            for (int sheetCount = 1; sheetCount < _workbook.NumberOfSheets; sheetCount++)
            {
                ISheet sheet = _workbook.GetSheetAt(sheetCount);  //獲取第i個工作表  
                textileList.Add(sheet.SheetName);
            }
            DataGridTextileList.ItemsSource = textileList;
            DataGridCustomerName.ItemsSource = CustomerModule.GetCustomerNameList();

            GetShippingCacheNameList();
        }

        private void GetShippingCacheNameList()
        {
            var existsFileName = Directory.GetFiles(AppSettingConfig.ShipFilePath(), "*.txt").Select(System.IO.Path.GetFileName);
            ComboBoxShippingCacheName.ItemsSource = existsFileName.ToList();
        }

        private void SelectedTextile_Click(object sender, RoutedEventArgs e)
        {
            if (((ObservableCollection<TextileColorInventoryShipping>)DataGridSelectedTextile.ItemsSource).Select(s => s.ShippingNumber).Sum() <= 0)
            {
                MessageBox.Show("未輸入出布數量!!");
                return;
            }

            IEnumerable<TextileColorInventoryShipping> selectedTextiles = DataGridSelectedTextile.Items.Cast<TextileColorInventoryShipping>().Where(w => w.ShippingNumber > 0);
            AddCustomerShippingFabric(selectedTextiles);
            ObservableCollection<TextileColorInventoryShipping> textileColorInventoryShipping = new ObservableCollection<TextileColorInventoryShipping>();
            foreach (var item in DataGridSelectedTextile.ItemsSource.Cast<TextileColorInventoryShipping>())
            {
                item.ShippingNumber = 0;
                textileColorInventoryShipping.Add(item);
            }
            DataGridSelectedTextile.ItemsSource = textileColorInventoryShipping;


        }

        private void AddCustomerShippingFabric(IEnumerable<TextileColorInventoryShipping> selectedTextiles)
        {
            var customerName = TextBoxCustomerName.Text.ToString();
            var textileName = DataGridTextileList.SelectedItem.ToString();
            //若表中有此客戶
            if (ShippingSheetStructure.Count > 0 && ShippingSheetStructure.Where(w => w.Customer == customerName).Count() > 0)
            {
                //若客戶中有此布種
                if (ShippingSheetStructure.Where(w => w.Customer == customerName).First().TextileShippingDatas.Count() > 0 && ShippingSheetStructure.Where(w => w.Customer == customerName).First().TextileShippingDatas.Where(w => w.TextileName == textileName).Count() > 0)
                {
                    //將新的布種顏色資料加入原有的布種資料
                    var currentTextileShippingData = ShippingSheetStructure.Where(w => w.Customer == customerName).First().TextileShippingDatas.Where(w => w.TextileName == textileName).First();
                    foreach (TextileColorInventoryShipping item in selectedTextiles)
                    {
                        if (currentTextileShippingData.ShippingSheetDatas.Where(w => w.ColorName == item.ColorName).Count() > 0)
                        {
                            currentTextileShippingData.ShippingSheetDatas.Where(w => w.ColorName == item.ColorName).First().ShippingNumber = item.ShippingNumber;
                        }
                        else
                            currentTextileShippingData.ShippingSheetDatas.Add(new ShippingSheetData
                            {
                                ColorName = item.ColorName,
                                CountInventory = item.CountInventory,
                                StorageSpaces = item.StorageSpaces,
                                ShippingNumber = item.ShippingNumber,
                                Memo = item.Memo
                            });
                    }
                    //每新增一次則重建顯示資料表
                    DataGridShippingDisplay();
                    if (ShippingSheetStructure.Last().Customer == customerName)
                    {
                        if (VisualTreeHelper.GetChild(DataGridShippingSheet, 0) is Decorator border)
                        {
                            if (border.Child is ScrollViewer scroll) scroll.ScrollToEnd();
                        }
                    }
                }
                else
                {
                    //新增新的布種與顏色
                    var currentTextileShippingDatas = ShippingSheetStructure.Where(w => w.Customer == customerName).First().TextileShippingDatas;
                    var shippingSheetDatas = new List<ShippingSheetData>();
                    foreach (TextileColorInventoryShipping item in selectedTextiles)
                    {
                        shippingSheetDatas.Add(new ShippingSheetData
                        {
                            ColorName = item.ColorName,
                            CountInventory = item.CountInventory,
                            StorageSpaces = item.StorageSpaces,
                            ShippingNumber = item.ShippingNumber,
                            Memo = item.Memo
                        });
                    }
                    currentTextileShippingDatas.Add(new TextileShippingData
                    {
                        TextileName = textileName,
                        ShippingSheetDatas = shippingSheetDatas
                    });
                    //每新增一次則重建顯示資料表
                    DataGridShippingDisplay();
                    if (ShippingSheetStructure.Last().Customer == customerName)
                    {
                        if (VisualTreeHelper.GetChild(DataGridShippingSheet, 0) is Decorator border)
                        {
                            if (border.Child is ScrollViewer scroll) scroll.ScrollToEnd();
                        }
                    }
                }
            }
            else
            {
                //新增新的客戶和布種與顏色
                var shippingSheetDatas = new List<ShippingSheetData>();
                foreach (TextileColorInventoryShipping item in selectedTextiles)
                {
                    shippingSheetDatas.Add(new ShippingSheetData
                    {
                        ColorName = item.ColorName,
                        CountInventory = item.CountInventory,
                        StorageSpaces = item.StorageSpaces,
                        ShippingNumber = item.ShippingNumber,
                        Memo = item.Memo
                    });
                }
                var textileShippingDatas = new List<TextileShippingData>() {
                    new TextileShippingData
                    {
                        TextileName = textileName,
                        ShippingSheetDatas = shippingSheetDatas
                    }
                };
                ShippingSheetStructure.Add(new ShippingSheetStructure
                {
                    Customer = customerName,
                    TextileShippingDatas = textileShippingDatas
                });
                //每新增一次則重建顯示資料表
                DataGridShippingDisplay();
                if (VisualTreeHelper.GetChild(DataGridShippingSheet, 0) is Decorator border)
                {
                    if (border.Child is ScrollViewer scroll) scroll.ScrollToEnd();
                }
            }
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            ExportToShip();
            ExportToExcel();
            if (ComboBoxShippingCacheName.SelectedIndex == -1)
            {
                for (int cacheIndex = 1; cacheIndex < 5; cacheIndex++)
                {
                    var fileName = string.Concat("出貨暫存", DateTime.Now.ToString("yyyyMMdd"), "-", cacheIndex, ".txt");

                    var shippingCacheFileName = string.Concat(AppSettingConfig.ShipFilePath(), @"\", fileName);
                    if (File.Exists(shippingCacheFileName))
                    {
                        continue;
                    }

                    var shippingSheetStructureJson = JsonConvert.SerializeObject(ShippingSheetStructure);

                    // Create a new file 
                    using (FileStream fs = File.Create(shippingCacheFileName))
                    {
                        // Add some text to file
                        Byte[] title = new UTF8Encoding(true).GetBytes(shippingSheetStructureJson);
                        fs.Write(title, 0, title.Length);
                    }
                    break;
                }
            }
            else
            {
                var selectedFileName = ComboBoxShippingCacheName.SelectedValue.ToString();
                var shippingCacheFileName = string.Concat(AppSettingConfig.ShipFilePath(), @"\", selectedFileName);

                var shippingSheetStructureJson = JsonConvert.SerializeObject(ShippingSheetStructure);

                // Create a new file 
                using (FileStream fs = File.Create(shippingCacheFileName))
                {
                    // Add some text to file
                    Byte[] title = new UTF8Encoding(true).GetBytes(shippingSheetStructureJson);
                    fs.Write(title, 0, title.Length);
                }
            }
            GetShippingCacheNameList();
            ComboBoxShippingCacheName.SelectedIndex = ComboBoxShippingCacheName.Items.Count - 1;
        }

        private void ExportToShip()
        {
            IWorkbook wb = new XSSFWorkbook();
            ICellStyle positionStyle = wb.CreateCellStyle();
            positionStyle.WrapText = true;
            positionStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            positionStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
            ICellStyle customerStyle = wb.CreateCellStyle();
            customerStyle.WrapText = true;
            customerStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            customerStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Bottom;

            IFont customerFont = wb.CreateFont();
            customerFont.FontName = "新細明體";
            customerFont.FontHeightInPoints = 19;
            customerStyle.SetFont(customerFont);

            ICellStyle weightStyle = wb.CreateCellStyle();
            weightStyle.WrapText = true;
            weightStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            weightStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
            IFont weightFont = wb.CreateFont();
            weightFont.FontName = "新細明體";
            weightFont.IsBold = true;
            weightFont.FontHeightInPoints = 15;
            weightStyle.SetFont(weightFont);

            ICellStyle LeftStyle = wb.CreateCellStyle();
            LeftStyle.WrapText = true;
            LeftStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
            LeftStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
            LeftStyle.SetFont(weightFont);

            List<ShippingSheetStructure> shippingSheetStructures = new List<ShippingSheetStructure>(ShippingSheetStructure);

            List<ExcelSheetContent> excelSheetContents = new List<ExcelSheetContent>();
            ExcelCellContent emptyExcelCellContent = new ExcelCellContent();
            for (int i = 0; i < shippingSheetStructures.Count; i++)
            {
                int totalRow = 0;

                ShippingSheetStructure shippingSheetStucture = new ShippingSheetStructure()
                {
                    Customer = shippingSheetStructures[i].Customer,
                    TextileShippingDatas = new List<TextileShippingData>(shippingSheetStructures[i].TextileShippingDatas.Select(s => new TextileShippingData { TextileName = s.TextileName, ShippingSheetDatas = new List<ShippingSheetData>(s.ShippingSheetDatas) }))
                };
                /**/
                totalRow = shippingSheetStucture.TextileShippingDatas.Sum(s => s.ShippingSheetDatas.Count());
                foreach (var shippingSheetData in shippingSheetStucture.TextileShippingDatas)
                {
                    foreach (ShippingSheetData color in shippingSheetData.ShippingSheetDatas)
                    {
                        if (color.ShippingNumber >= 8)
                            totalRow = totalRow + color.ShippingNumber / 7;
                    }
                }

                if (totalRow > 13)
                {
                    List<TextileShippingData> cacheLastTextileShippingData = new List<TextileShippingData>();

                    int skipRow = 0;

                    for (int textileShippingDataIndex = 0; textileShippingDataIndex < shippingSheetStucture.TextileShippingDatas.Count(); textileShippingDataIndex++)
                    {
                        TextileShippingData textileShippingData = shippingSheetStucture.TextileShippingDatas[textileShippingDataIndex];

                        for (int shippingSheetDataIndex = 0; shippingSheetDataIndex < textileShippingData.ShippingSheetDatas.Count(); shippingSheetDataIndex++)
                        {
                            ShippingSheetData shippingSheetData = textileShippingData.ShippingSheetDatas[shippingSheetDataIndex];
                            skipRow++;
                            if (shippingSheetData.ShippingNumber > 7)
                            {
                                skipRow = skipRow + shippingSheetData.ShippingNumber / 7;
                            }
                            if (skipRow > 13)
                            {
                                int indexOftextileShippingData = shippingSheetStucture.TextileShippingDatas.IndexOf(textileShippingData);
                                shippingSheetStucture.TextileShippingDatas[indexOftextileShippingData].ShippingSheetDatas.Remove(shippingSheetData);
                                if (shippingSheetStucture.TextileShippingDatas[indexOftextileShippingData].ShippingSheetDatas.Count == 0)
                                {
                                    shippingSheetStucture.TextileShippingDatas.Remove(textileShippingData);
                                }
                                if (cacheLastTextileShippingData.Count(c => c.TextileName == textileShippingData.TextileName) > 0)
                                {
                                    int indexCache = cacheLastTextileShippingData.FindIndex(f => f.TextileName == textileShippingData.TextileName);
                                    cacheLastTextileShippingData[indexCache].ShippingSheetDatas.Add(shippingSheetData);
                                }
                                else
                                {
                                    cacheLastTextileShippingData.Add(
                                        new TextileShippingData
                                        {
                                            TextileName = textileShippingData.TextileName,
                                            ShippingSheetDatas = new List<ShippingSheetData> { shippingSheetData }
                                        }
                                    );
                                }
                            }
                        }
                    }
                    ShippingSheetStructure shippingSheetStructure = new ShippingSheetStructure()
                    {
                        Customer = shippingSheetStucture.Customer + "-1",
                        TextileShippingDatas = cacheLastTextileShippingData
                    };
                    int indexOfCurrentCustomer = shippingSheetStructures.IndexOf(shippingSheetStructures[i]) + 1;
                    shippingSheetStructures.Insert(indexOfCurrentCustomer, shippingSheetStructure);
                }


                ExcelSheetContent customerShipSheet = new ExcelSheetContent()
                {
                    SheetName = shippingSheetStucture.Customer,
                    LeftMargin = 0.2,
                    RightMargin = 0.2,
                    BottomMargin = 0.2,
                    TopMargin = 0.2,
                    ColumnHeight = 510,
                    ExcelColumnContents = new List<ExcelColumnContent>
                    {
                        new ExcelColumnContent
                        {
                            Width = 990,
                        },
                        new ExcelColumnContent
                        {
                            Width = 2650,
                        },
                        new ExcelColumnContent
                        {
                            Width = 2630
                        },
                        new ExcelColumnContent
                        {
                            Width = 850,
                        },
                        new ExcelColumnContent
                        {
                            Width = 1730,
                        },
                        new ExcelColumnContent
                        {
                            Width = 1730,
                        },
                        new ExcelColumnContent
                        {
                            Width = 2418,
                        },
                        new ExcelColumnContent
                        {
                            Width = 2418,
                        },
                        new ExcelColumnContent
                        {
                            Width = 2418,
                        },
                        new ExcelColumnContent
                        {
                            Width = 2418,
                        },
                        new ExcelColumnContent
                        {
                            Width = 2418,
                        },
                        new ExcelColumnContent
                        {
                            Width = 2418,
                        },
                        new ExcelColumnContent
                        {
                            Width = 2418,
                        }
                    },
                    ExcelRowContents = new List<ExcelRowContent>()
                };

                customerShipSheet.ExcelRowContents.Add(new ExcelRowContent { Height = 255 });
                customerShipSheet.ExcelRowContents.Add(new ExcelRowContent { Height = 225 });
                customerShipSheet.ExcelRowContents.Add(new ExcelRowContent { Height = 225 });
                customerShipSheet.ExcelRowContents.Add(new ExcelRowContent
                {
                    Height = 420,
                    ExcelCellContents = new List<ExcelCellContent>()
                    {
                       emptyExcelCellContent,
                       emptyExcelCellContent,
                       new ExcelCellContent()
                       {
                           CellValue = shippingSheetStucture.Customer.Split('-')[0],
                           CellStyle = customerStyle
                       },
                       emptyExcelCellContent,
                       emptyExcelCellContent,  new ExcelCellContent()
                       {
                           CellRangeAddress = new CellRangeAddress(4, 4, 2, 5)
                       },
                       emptyExcelCellContent,
                       emptyExcelCellContent,
                       emptyExcelCellContent,
                       emptyExcelCellContent,
                       new ExcelCellContent()
                       {
                           CellValue = DateTime.Now.AddYears(-1911).Year.ToString(),
                           CellStyle = LeftStyle,
                       },
                       new ExcelCellContent()
                       {
                           CellValue = DateTime.Now.Month.ToString(),
                           CellStyle = weightStyle,
                       },
                       new ExcelCellContent()
                       {
                           CellValue = DateTime.Now.Day.ToString(),
                           CellStyle = weightStyle,
                       }
                    }
                });
                customerShipSheet.ExcelRowContents.Add(new ExcelRowContent() { Height = 330 });

                int rowNumber = 6;
                foreach (var textileShippingData in shippingSheetStucture.TextileShippingDatas)
                {
                    bool textileNameDisplay = true;
                    foreach (var shippingSheetData in textileShippingData.ShippingSheetDatas)
                    {
                        string colorName = shippingSheetData.ColorName.Split('-')[0];
                        colorName = shippingSheetData.ShippingNumber == 1 ? colorName : colorName + "-" + shippingSheetData.ShippingNumber;
                        ExcelRowContent excelRowColorContent = new ExcelRowContent()
                        {
                            Height = 405,
                            ExcelCellContents = new List<ExcelCellContent>()
                            {
                                emptyExcelCellContent,
                                new ExcelCellContent()
                                {
                                    CellValue = textileNameDisplay == true ? textileShippingData.TextileName : string.Empty,
                                    CellStyle = positionStyle,
                                    CellRangeAddress = new CellRangeAddress(rowNumber, rowNumber, 1, 3)
                                },
                                emptyExcelCellContent,
                                emptyExcelCellContent,
                                new ExcelCellContent()
                                {
                                    CellValue = colorName,
                                    CellStyle = positionStyle,
                                    CellRangeAddress = new CellRangeAddress(rowNumber, rowNumber, 4, 5)
                                },
                                new ExcelCellContent
                                {
                                    CellValue = string.Empty,
                                    CellStyle = weightStyle
                                },
                                new ExcelCellContent
                                {
                                    CellValue = string.Empty,
                                    CellStyle = weightStyle
                                },
                                new ExcelCellContent
                                {
                                    CellValue = string.Empty,
                                    CellStyle = weightStyle
                                },
                                new ExcelCellContent
                                {
                                    CellValue = string.Empty,
                                    CellStyle = weightStyle
                                },
                                new ExcelCellContent
                                {
                                    CellValue = string.Empty,
                                    CellStyle = weightStyle
                                },
                                new ExcelCellContent
                                {
                                    CellValue = string.Empty,
                                    CellStyle = weightStyle
                                },
                                new ExcelCellContent
                                {
                                    CellValue = string.Empty,
                                    CellStyle = weightStyle
                                }
                            }
                        };
                        textileNameDisplay = false;

                        customerShipSheet.ExcelRowContents.Add(excelRowColorContent);
                        rowNumber++;
                        if (shippingSheetData.ShippingNumber >= 8)
                        {
                            int skipRowNumber = rowNumber + (shippingSheetData.ShippingNumber / 7);
                            for (int skipRowCount = rowNumber; skipRowCount < skipRowNumber; skipRowCount++)
                            {
                                customerShipSheet.ExcelRowContents.Add(new ExcelRowContent { Height = 405 });
                            }
                            rowNumber = rowNumber + (shippingSheetData.ShippingNumber / 7);
                        }
                    }
                }

                excelSheetContents.Add(customerShipSheet);
            }
            ExcelHelper excelHelper = new ExcelHelper();
            ExcelContent excelContent = new ExcelContent
            {
                FileName = string.Concat("出貨單", DateTime.Now.ToString("yyyy-MM-dd")),
                ExcelSheetContents = excelSheetContents
            };

            excelHelper.CreateExcelFile(wb, excelContent);
        }

        private void ExportToExcel()
        {
            //建立Excel 2003檔案
            IWorkbook wb = new XSSFWorkbook();
            ICellStyle positionStyle = wb.CreateCellStyle();
            positionStyle.WrapText = true;
            positionStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            positionStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;

            ICellStyle textileNameStyle = wb.CreateCellStyle();
            textileNameStyle.WrapText = true;
            textileNameStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            textileNameStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;

            IFont textileNameFont = wb.CreateFont();
            textileNameFont.IsBold = true;
            textileNameFont.FontName = "新細明體";
            textileNameFont.Color = NPOI.HSSF.Util.HSSFColor.Red.Index;
            textileNameStyle.SetFont(textileNameFont);

            ICellStyle shippingNumberStyle = wb.CreateCellStyle();
            shippingNumberStyle.WrapText = true;
            shippingNumberStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            shippingNumberStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
            shippingNumberStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
            shippingNumberStyle.FillPattern = FillPattern.SolidForeground;

            ICellStyle needCheckStyle = wb.CreateCellStyle();
            needCheckStyle.WrapText = true;
            needCheckStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            needCheckStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
            needCheckStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Red.Index;
            needCheckStyle.FillPattern = FillPattern.SolidForeground;

            ICellStyle lightTurquoiseStyle = wb.CreateCellStyle();
            lightTurquoiseStyle.BorderRight = BorderStyle.Thin;
            lightTurquoiseStyle.BorderBottom = BorderStyle.Thin;
            lightTurquoiseStyle.BorderTop = BorderStyle.Thin;
            lightTurquoiseStyle.BorderLeft = BorderStyle.Thin;
            lightTurquoiseStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightTurquoise.Index;
            lightTurquoiseStyle.FillPattern = FillPattern.SolidForeground;

            ICellStyle coralStyle = wb.CreateCellStyle();
            coralStyle.BorderRight = BorderStyle.Thin;
            coralStyle.BorderBottom = BorderStyle.Thin;
            coralStyle.BorderTop = BorderStyle.Thin;
            coralStyle.BorderLeft = BorderStyle.Thin;
            coralStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Coral.Index;
            coralStyle.FillPattern = FillPattern.SolidForeground;

            int rowIndex = 1;
            int colorIndex = 0;
            int total = 0;

            List<ExcelRowContent> shipPosition = new List<ExcelRowContent>();
            foreach (var shippingSheetStructure in ShippingSheetStructure)
            {
                ExcelRowContent excelRowContent = new ExcelRowContent
                {
                    Height = 318,
                    ExcelCellContents = new List<ExcelCellContent>()
                    {
                        new ExcelCellContent()
                        {
                            CellValue = shippingSheetStructure.Customer,
                            CellStyle = null
                        }
                    }
                };
                int customerTotal = 0;
                foreach (var textileShippingData in shippingSheetStructure.TextileShippingDatas)
                {
                    //判斷若為第0筆資料,則與客戶資料同一行,否則跳下一行
                    if (shippingSheetStructure.TextileShippingDatas.IndexOf(textileShippingData) == 0)
                    {
                        excelRowContent.ExcelCellContents.Add(new ExcelCellContent()
                        {
                            CellValue = textileShippingData.TextileName,
                            CellStyle = textileNameStyle,
                            CellRangeAddress = new CellRangeAddress(rowIndex, rowIndex, 1, 3)
                        });
                        shipPosition.Add(excelRowContent);
                    }
                    else
                    {
                        rowIndex++;
                        ExcelRowContent excelRowTextileName = new ExcelRowContent
                        {
                            Height = 318,
                            ExcelCellContents = new List<ExcelCellContent>()
                            {
                                new ExcelCellContent()
                                {
                                       CellValue = null,
                                       CellStyle = null
                                },
                                new ExcelCellContent()
                                {
                                       CellValue = textileShippingData.TextileName,
                                       CellStyle = textileNameStyle,
                                       CellRangeAddress = new CellRangeAddress(rowIndex, rowIndex, 1, 3)
                                }
                            }
                        };
                        shipPosition.Add(excelRowTextileName);
                    }

                    foreach (var shippingSheetData in textileShippingData.ShippingSheetDatas)
                    {
                        rowIndex++;
                        var colorByStorageSpaces = ExcelHelper.GetColorByStorageSpaces(wb, shippingSheetData.StorageSpaces);
                        if (!string.IsNullOrEmpty(shippingSheetData.StorageSpaces))
                        {
                            var font = wb.CreateFont();
                            font.FontName = "新細明體";
                            font.IsBold = true;
                            colorByStorageSpaces.SetFont(font);
                        }
                        ExcelRowContent excelRowColorInfo = new ExcelRowContent
                        {
                            Height = 440,
                            ExcelCellContents = new List<ExcelCellContent>
                            {
                                new ExcelCellContent()
                                {
                                    CellValue = null,
                                    CellStyle = null,
                                },
                                new ExcelCellContent()
                                {
                                    CellValue = shippingSheetData.ColorName,
                                    CellStyle = positionStyle
                                },
                                new ExcelCellContent()
                                {
                                    CellValue = shippingSheetData.CountInventory.ToString(),
                                    CellStyle = (shippingSheetData.CountInventory - shippingSheetData.ShippingNumber) <= 3 ? needCheckStyle : positionStyle
                                },
                                new ExcelCellContent()
                                {
                                    CellValue = shippingSheetData.StorageSpaces,
                                    CellStyle = colorByStorageSpaces,
                                },
                                new ExcelCellContent()
                                {
                                    CellValue = shippingSheetData.ShippingNumber.ToString(),
                                    CellStyle = shippingNumberStyle
                                },
                                new ExcelCellContent()
                                {
                                    CellValue = shippingSheetData.Memo,
                                    CellStyle = null
                                }
                            }
                        };
                        shipPosition.Add(excelRowColorInfo);
                        int weightCellNumber = (int)ExcelEnum.ShippingSheetEnum.WeightCellNumber;
                        int startIndex = (int)ExcelEnum.ShippingSheetEnum.WeightCellStartIndex + shippingSheetData.Memo.Count() / (int)ExcelEnum.ShippingSheetEnum.CellOfStringLength;
                        for (int i = 6; i < startIndex; i++)
                        {
                            excelRowColorInfo.ExcelCellContents.Add(new ExcelCellContent());

                        }
                        for (int shippingCount = startIndex; shippingCount < startIndex + shippingSheetData.ShippingNumber; shippingCount++)
                        {
                            if ((shippingCount - 1) % weightCellNumber == 0 && shippingCount - 1 > weightCellNumber)
                            {
                                rowIndex++;
                                shipPosition.Add(new ExcelRowContent()
                                {
                                    Height = 440
                                });
                                shipPosition.Last().ExcelCellContents = new List<ExcelCellContent>();
                                for (int i = 0; i < 6; i++)
                                {
                                    shipPosition.Last().ExcelCellContents.Add(new ExcelCellContent());
                                }
                            }
                            if (colorIndex % 2 == 0)
                            {
                                shipPosition.Last().ExcelCellContents.Add(new ExcelCellContent()
                                {
                                    CellValue = string.Empty,
                                    CellStyle = lightTurquoiseStyle,
                                });
                            }
                            else
                            {
                                shipPosition.Last().ExcelCellContents.Add(new ExcelCellContent()
                                {
                                    CellValue = string.Empty,
                                    CellStyle = coralStyle,
                                });
                            }
                        }
                        colorIndex++;
                        customerTotal += shippingSheetData.ShippingNumber;
                    }
                }
                rowIndex++;

                ExcelRowContent excelRowTotal = new ExcelRowContent
                {
                    Height = 440,
                    ExcelCellContents = new List<ExcelCellContent>()
                    {
                        new ExcelCellContent()
                        {
                                CellValue = null,
                                CellStyle = null
                        },
                        new ExcelCellContent()
                        {
                                CellValue = null,
                                CellStyle = null
                        },
                        new ExcelCellContent()
                        {
                                CellValue = null,
                                CellStyle = null
                        },
                        new ExcelCellContent()
                        {
                                CellValue = "總數",
                                CellStyle = positionStyle
                        },
                        new ExcelCellContent()
                        {
                                CellValue = customerTotal.ToString(),
                                CellStyle = positionStyle
                        }
                    }
                };
                shipPosition.Add(excelRowTotal);
                total += customerTotal;
                rowIndex++;
            }

            ExcelRowContent excelRowContentTotal = new ExcelRowContent
            {
                Height = 440,
                ExcelCellContents = new List<ExcelCellContent>()
                {
                    new ExcelCellContent()
                    {
                        CellValue = null,
                        CellStyle = null
                    },
                        new ExcelCellContent()
                    {
                        CellValue = null,
                        CellStyle = null
                    },
                        new ExcelCellContent()
                    {
                        CellValue = null,
                        CellStyle = null
                    },
                        new ExcelCellContent()
                    {
                        CellValue = "總出貨數",
                        CellStyle = positionStyle
                    }, new ExcelCellContent()
                    {
                        CellValue = total.ToString(),
                        CellStyle = positionStyle
                    },
                }
            };
            shipPosition.Add(excelRowContentTotal);

            ShipExcelData(wb, positionStyle, shipPosition);
        }

        private static void ShipExcelData(IWorkbook wb, ICellStyle positionStyle, List<ExcelRowContent> shipPosition)
        {
            ExcelHelper excelHelper = new ExcelHelper();
            ExcelContent excelContent = new ExcelContent
            {
                FileName = string.Concat("出貨", DateTime.Now.ToString("yyyy-MM-dd")),
                ExcelSheetContents = new List<ExcelSheetContent>
                {
                    new ExcelSheetContent
                    {
                        SheetName = "出貨位置表",
                        LeftMargin = 0.04,
                        RightMargin = 0.04,
                        BottomMargin = 0.1,
                        TopMargin = 0.1,
                        ExcelColumnContents = new List<ExcelColumnContent>
                        {
                            new ExcelColumnContent
                            {
                                CellValue = "客戶",
                                Width = 3000,
                                CellStyle = positionStyle
                            },
                            new ExcelColumnContent
                            {
                                CellValue = "顏色",
                                Width = 5020,
                                CellStyle = positionStyle
                            },
                            new ExcelColumnContent
                            {
                                CellValue = "庫存量",
                                Width = 1700,
                                CellStyle = positionStyle
                            },
                            new ExcelColumnContent
                            {
                                CellValue = "儲位",
                                CellStyle = positionStyle
                            },
                            new ExcelColumnContent
                            {
                                CellValue = "出貨數量",
                                Width = 1700,
                                CellStyle = positionStyle
                            },
                            new ExcelColumnContent
                            {
                                CellValue = "備註",
                                CellStyle = positionStyle
                            },
                            new ExcelColumnContent
                            {
                                CellValue = "1",
                                CellStyle = positionStyle
                            },
                            new ExcelColumnContent
                            {
                                CellValue = "2",
                                CellStyle = positionStyle
                            },
                            new ExcelColumnContent
                            {
                                CellValue = "3",
                                CellStyle = positionStyle
                            },
                            new ExcelColumnContent
                            {
                                CellValue = "4",
                                CellStyle = positionStyle
                            },
                            new ExcelColumnContent
                            {
                                CellValue = "5",
                                CellStyle = positionStyle
                            }
                        },
                        ExcelRowContents = shipPosition
                    },
                    //new ExcelSheetContent
                    //{
                    //    SheetName = "Excel為主",
                    //    ExcelColumnContents = new List<ExcelColumnContent>
                    //    {
                    //        new ExcelColumnContent
                    //        {
                    //            CellValue = "Super布種名稱顏色",
                    //            Width = 6450
                    //        },
                    //        new ExcelColumnContent
                    //        {
                    //            CellValue = "出貨重量",
                    //            Width = 2800
                    //        },
                    //        new ExcelColumnContent
                    //        {
                    //            CellValue = "約略出貨數",
                    //            Width = 2000
                    //        },
                    //        new ExcelColumnContent
                    //        {
                    //            CellValue = "布種名稱",
                    //            Width = 4550
                    //        },
                    //        new ExcelColumnContent
                    //        {
                    //            CellValue = "顏色",
                    //            Width = 5550
                    //        },
                    //        new ExcelColumnContent
                    //        {
                    //            CellValue = "出貨數量",
                    //            Width = 1850
                    //        }
                    //    },
                    //    ExcelRowContents = null
                    //}
                }
            };
            excelHelper.CreateExcelFile(wb, excelContent);
        }

        private void CreateCell(XSSFRow row, int cellIndex, string cellValue, ICellStyle style)
        {
            var cell = row.CreateCell(cellIndex);
            cell.SetCellValue(cellValue);
            cell.CellStyle = style;
        }

        private void ButtonShippingDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = DataGridShippingSheet.SelectedItem as ShippingSheetData;
            if (selectedItem.Array1 == -1 && selectedItem.Array2 == -1)
            {
                ShippingSheetStructure.RemoveAt(selectedItem.Array0);
            }
            else if (selectedItem.Array2 == -1)
            {
                ShippingSheetStructure.ElementAt(selectedItem.Array0).TextileShippingDatas.RemoveAt(selectedItem.Array1);
            }
            else
            {
                ShippingSheetStructure.ElementAt(selectedItem.Array0).TextileShippingDatas.ElementAt(selectedItem.Array1).ShippingSheetDatas.RemoveAt(selectedItem.Array2);
            }
            DataGridShippingDisplay();
        }

        private void DataGridShippingDisplay()
        {
            ShippingSheetDatas = new ObservableCollection<ShippingSheetData>();
            DataGridShippingSheet.ItemsSource = ShippingSheetDatas;

            int array0Index = 0;
            foreach (var item in ShippingSheetStructure)
            {
                int array1Index = 0;
                int customerShippingTotal = 0;
                ShippingSheetDatas.Add(new ShippingSheetData
                {
                    Customer = item.Customer,
                    Array0 = array0Index,
                    Array1 = -1,
                    Array2 = -1
                });
                foreach (var textile in item.TextileShippingDatas)
                {
                    int array2Index = 0;
                    ShippingSheetDatas.Add(new ShippingSheetData
                    {
                        TextileName = textile.TextileName,
                        Array0 = array0Index,
                        Array1 = array1Index,
                        Array2 = -1
                    });
                    foreach (var color in textile.ShippingSheetDatas)
                    {
                        ShippingSheetDatas.Add(new ShippingSheetData
                        {
                            ColorName = color.ColorName,
                            CountInventory = color.CountInventory,
                            StorageSpaces = color.StorageSpaces,
                            ShippingNumber = color.ShippingNumber,
                            Memo = color.Memo,
                            Array0 = array0Index,
                            Array1 = array1Index,
                            Array2 = array2Index,

                        });
                        Regex reg = new Regex(@"碼布$");
                        var isAccessories = reg.IsMatch(textile.TextileName);
                        //如果是配件碼布的話只要加1
                        customerShippingTotal += isAccessories ? 1 : color.ShippingNumber;
                        array2Index++;
                    }
                    array1Index++;
                }
                ShippingSheetDatas.Add(new ShippingSheetData
                {
                    StorageSpaces = "總數",
                    ShippingNumber = customerShippingTotal
                });
                array0Index++;
            }

        }
        private string[] GetFileNames(string path, string filter)
        {
            string[] files = Directory.GetFiles(path, filter);
            for (int i = 0; i < files.Length; i++)
                files[i] = System.IO.Path.GetFileName(files[i]);
            return files;
        }

        private void ComboBoxShippingCacheName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var fileName = ((ComboBox)sender).SelectedValue.ToString();
            var shippingCacheFileName = string.Concat(AppSettingConfig.ShipFilePath(), @"\", fileName);
            //this code segment read data from the file.
            FileStream fs2 = new FileStream(shippingCacheFileName, FileMode.OpenOrCreate, FileAccess.Read);
            StreamReader reader = new StreamReader(fs2);
            var cacheJson = reader.ReadToEnd();
            ShippingSheetStructure = JsonConvert.DeserializeObject<List<ShippingSheetStructure>>(cacheJson);
            reader.Close();

            DataGridShippingDisplay();
        }

        private void TextBoxCustomerName_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBoxName = (TextBox)sender;
            string filterText = textBoxName.Text;
            ICollectionView cv = CollectionViewSource.GetDefaultView(DataGridCustomerName.ItemsSource);
            if (!string.IsNullOrEmpty(filterText))
            {
                cv.Filter = o =>
                {
                    /* change to get data row value */
                    string p = o as string;
                    return (p.ToUpper().Contains(filterText.ToUpper()));
                    /* end change to get data row value */
                };
            }
            else
            {
                cv.Filter = o =>
                {
                    return (true);
                };
            }
        }

        private void DataGridCustomerName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridCustomerName.SelectedIndex != -1)
            {
                TextBoxCustomerName.Text = DataGridCustomerName.SelectedItem as string;
            }
        }

        private void TextBoxTextile_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBoxName = (TextBox)sender;
            string filterText = textBoxName.Text;
            ICollectionView cv = CollectionViewSource.GetDefaultView(DataGridTextileList.ItemsSource);
            if (!string.IsNullOrEmpty(filterText))
            {
                cv.Filter = o =>
                {
                    /* change to get data row value */
                    string p = o as string;
                    return (p.ToUpper().Contains(filterText.ToUpper()));
                    /* end change to get data row value */
                };
            }
            else
            {
                cv.Filter = o =>
                {
                    return (true);
                };
            }
        }

        private void TextBoxTextile_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                DataGridTextileList.SelectedIndex = -1;
            }
        }

        private void DataGridTextileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (sender as DataGrid).SelectedItem;
            if (selectedItem == null)
                return;
            string textileName = selectedItem.ToString();
            ExcelHelper excelHelper = new ExcelHelper();
            IEnumerable<TextileColorInventoryShipping> textileColorInventoryShippings = excelHelper.GetInventoryData(_workbook, textileName).Select(s => new TextileColorInventoryShipping
            {
                ColorName = s.ColorName,
                StorageSpaces = s.StorageSpaces,
                CountInventory = s.CountInventory,
                ClearFactory = s.ClearFactory,
                Memo = s.Memo,
                ShippingNumber = 0
            });

            IList selectedTextiles = new ObservableCollection<TextileColorInventoryShipping>(textileColorInventoryShippings);
            DataGridSelectedTextile.ItemsSource = selectedTextiles;
        }

        private void TextBoxColor_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBoxName = (TextBox)sender;
            string filterText = textBoxName.Text;
            ICollectionView cv = CollectionViewSource.GetDefaultView(DataGridSelectedTextile.ItemsSource);
            if (!string.IsNullOrEmpty(filterText))
            {
                cv.Filter = o =>
                {
                    /* change to get data row value */
                    TextileColorInventory p = o as TextileColorInventory;
                    return (p.ColorName.ToUpper().Contains(filterText.ToUpper()));
                    /* end change to get data row value */
                };
            }
            else
            {
                cv.Filter = o =>
                {
                    return (true);
                };
            }
        }

        private void ButtonAddQuantity_Click(object sender, RoutedEventArgs e)
        {
            TextileColorInventoryShipping selectedTextile = DataGridSelectedTextile.SelectedItem as TextileColorInventoryShipping;
            string customerName = TextBoxCustomerName.Text;
            string textileName = DataGridTextileList.SelectedItem.ToString();
            selectedTextile.ShippingNumber = 1;
            if (ShippingSheetStructure.Count > 0 && ShippingSheetStructure.Where(w => w.Customer == customerName).Count() > 0)
            {
                //若客戶中有此布種
                if (ShippingSheetStructure.Where(w => w.Customer == customerName).First().TextileShippingDatas.Count() > 0 && ShippingSheetStructure.Where(w => w.Customer == customerName).First().TextileShippingDatas.Where(w => w.TextileName == textileName).Count() > 0)
                {
                    if (ShippingSheetStructure.Where(w => w.Customer == customerName).First()
                         .TextileShippingDatas
                         .Where(w => w.TextileName == textileName).First()
                         .ShippingSheetDatas.Where(w => w.ColorName == selectedTextile.ColorName).Count() > 0)
                    {
                        ShippingSheetData shippingSheetData = ShippingSheetStructure.Where(w => w.Customer == customerName).First()
                             .TextileShippingDatas
                             .Where(w => w.TextileName == textileName).First()
                             .ShippingSheetDatas.Where(w => w.ColorName == selectedTextile.ColorName).First();

                        selectedTextile.ShippingNumber = shippingSheetData.ShippingNumber + 1;
                    }
                }
            }
            List<TextileColorInventoryShipping> selectedTextiles = new List<TextileColorInventoryShipping>
            {
                selectedTextile
            };
            AddCustomerShippingFabric(selectedTextiles);
            selectedTextile.ShippingNumber = 0;
        }
    }
}
