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
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
        public List<ShippingSheetStructure> ShippingSheetStructure = new List<ShippingSheetStructure>();

        private IWorkbook _workbook { get; set; }

        // 用於防抖的計時器
        private CancellationTokenSource _filterCancellationTokenSource;
        public ShippingPage()
        {
            InitializeComponent();
            //IWorkbook _workbook = null;  //新建IWorkbook對象  
            GetStoreMangeWorkbook();
            GetShippingCacheNameList();
        }
        public void GetStoreMangeWorkbook()
        {
            string fileName = Path.Combine(AppSettingConfig.FilePath(), AppSettingConfig.StoreManageFileName());
            using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            using (_workbook = new XSSFWorkbook(fileStream))
            {
                DataGridTextileList.ItemsSource = GetTextileNames(_workbook);
                DataGridCustomerName.ItemsSource = CustomerModule.GetCustomerNameList();
            }

        }

        private IEnumerable<string> GetTextileNames(IWorkbook workbook)
        {
            for (int sheetCount = 1; sheetCount < workbook.NumberOfSheets; sheetCount++)
            {
                yield return workbook.GetSheetAt(sheetCount).SheetName;
            }
        }       

        public void GetShippingCacheNameList()
        {
            var existsFileName = Directory.GetFiles(AppSettingConfig.ShipFilePath(), "*.txt").Select(System.IO.Path.GetFileName);
            ComboBoxShippingCacheName.ItemsSource = existsFileName;
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
            // 防錯檢查
            if (selectedTextiles == null || !selectedTextiles.Any())
            {
                return;
            }

            string customerName = TextBoxCustomerName.Text?.Trim();
            if (string.IsNullOrEmpty(customerName))
            {
                MessageBox.Show("請輸入客戶名稱！");
                return;
            }

            if (DataGridTextileList.SelectedItem == null)
            {
                MessageBox.Show("請選擇布種！");
                return;
            }
            string textileName = DataGridTextileList.SelectedItem.ToString();
            // 查找現有客戶
            var customer = ShippingSheetStructure.FirstOrDefault(w => w.Customer == customerName);
            //若表中有此客戶
            if (customer != null)
            {
                // 查找現有布種
                var textileData = customer.TextileShippingDatas.FirstOrDefault(w => w.TextileName == textileName);
                if (textileData != null)
                {
                    // 更新現有布種的顏色資料
                    UpdateTextileColors(textileData, selectedTextiles);
                }
                else
                {
                    // 新增布種和顏色
                    customer.TextileShippingDatas.Add(CreateTextileData(textileName, selectedTextiles));
                }
            }
            else
            {
                // 新增客戶、布種和顏色
                ShippingSheetStructure.Add(new ShippingSheetStructure
                {
                    Customer = customerName,
                    TextileShippingDatas = new List<TextileShippingData>
            {
                CreateTextileData(textileName, selectedTextiles)
            }
                });
            }
            //每新增一次則重建顯示資料表
            DataGridShippingDisplay();
            ScrollToCustomer(customerName);
        }

        // 輔助方法：更新布種的顏色資料
        private void UpdateTextileColors(TextileShippingData textileData, IEnumerable<TextileColorInventoryShipping> selectedTextiles)
        {
            foreach (var item in selectedTextiles)
            {
                var existingColor = textileData.ShippingSheetDatas.FirstOrDefault(w => w.ColorName == item.ColorName);
                if (existingColor != null)
                {
                    existingColor.ShippingNumber = item.ShippingNumber;
                    existingColor.CountInventory = item.CountInventory;
                    existingColor.StorageSpaces = item.StorageSpaces;
                    existingColor.Memo = item.Memo;
                }
                else
                {
                    textileData.ShippingSheetDatas.Add(CreateShippingSheetData(item));
                }
            }
        }

        // 輔助方法：創建布種資料
        private TextileShippingData CreateTextileData(string textileName, IEnumerable<TextileColorInventoryShipping> selectedTextiles)
        {
            return new TextileShippingData
            {
                TextileName = textileName,
                ShippingSheetDatas = selectedTextiles.Select(CreateShippingSheetData).ToList()
            };
        }

        // 輔助方法：創建單個顏色資料
        private ShippingSheetData CreateShippingSheetData(TextileColorInventoryShipping item)
        {
            return new ShippingSheetData
            {
                ColorName = item.ColorName,
                CountInventory = item.CountInventory,
                StorageSpaces = item.StorageSpaces,
                ShippingNumber = item.ShippingNumber,
                Memo = item.Memo
            };
        }

        private void ScrollToCustomer(string customerName)
        {
            if (string.IsNullOrEmpty(customerName) || DataGridShippingSheet == null || ShippingSheetDatas == null)
            {
                return;
            }

            // 查找第一個匹配該客戶的項目
            var targetItem = ShippingSheetDatas.FirstOrDefault(item => item.Customer == customerName);
            if (targetItem == null)
            {
                return;
            }

            // 確保目標項目在可見範圍內（初始化位置）
            DataGridShippingSheet.ScrollIntoView(targetItem);

            // 獲取 DataGrid 內的 ScrollViewer
            ScrollViewer scrollViewer = FindScrollViewer(DataGridShippingSheet);
            if (scrollViewer == null)
            {
                return;
            }

            // 獲取目標項目的索引
            int targetIndex = ShippingSheetDatas.IndexOf(targetItem);

            // 估算目標行的偏移量（假設每行高度一致）
            double estimatedRowHeight = 1.0; // 根據實際行高調整
            double targetOffset = targetIndex * estimatedRowHeight;

            // 限制偏移量在有效範圍內
            targetOffset = Math.Max(0, Math.Min(targetOffset, scrollViewer.ScrollableHeight));

            // 創建平滑滾動動畫
            DoubleAnimation animation = new DoubleAnimation
            {
                From = scrollViewer.VerticalOffset,
                To = targetOffset,
                Duration = new Duration(TimeSpan.FromMilliseconds(500)), // 動畫持續時間
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut } // 緩動效果
            };

            // 應用動畫到 ScrollViewer 的 VerticalOffset
            scrollViewer.BeginAnimation(ScrollViewerBehavior.VerticalOffsetProperty, animation);
        }

        // 輔助方法：查找 DataGrid 內的 ScrollViewer
        private ScrollViewer FindScrollViewer(DependencyObject depObj)
        {
            if (depObj is ScrollViewer scrollViewer)
            {
                return scrollViewer;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                var result = FindScrollViewer(child);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        // 輔助類：用於動畫控制 ScrollViewer 的 VerticalOffset
        public static class ScrollViewerBehavior
        {
            public static readonly DependencyProperty VerticalOffsetProperty =
                DependencyProperty.RegisterAttached(
                    "VerticalOffset",
                    typeof(double),
                    typeof(ScrollViewerBehavior),
                    new UIPropertyMetadata(0.0, OnVerticalOffsetChanged));

            public static double GetVerticalOffset(DependencyObject obj)
            {
                return (double)obj.GetValue(VerticalOffsetProperty);
            }

            public static void SetVerticalOffset(DependencyObject obj, double value)
            {
                obj.SetValue(VerticalOffsetProperty, value);
            }

            private static void OnVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                if (d is ScrollViewer scrollViewer)
                {
                    scrollViewer.ScrollToVerticalOffset((double)e.NewValue);
                }
            }
        }

        private async void Export_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 檢查是否有資料
                if (!ShippingSheetStructure.Any())
                {
                    MessageBox.Show("沒有出貨資料可匯出！", "匯出失敗", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 匯出 Excel
                IWorkbook workbook = new XSSFWorkbook();
                List<ExcelSheetContent> shippingSheet = ExportToShip(workbook);
                List<ExcelSheetContent> shippingLocation = ExportToExcel(workbook);

                ExcelContent excelContent = new ExcelContent
                {
                    FileName = $"出貨{DateTime.Now:yyyyMMdd}",
                    ExcelSheetContents = new List<ExcelSheetContent>()
                };
                excelContent.ExcelSheetContents.AddRange(shippingLocation);
                excelContent.ExcelSheetContents.AddRange(shippingSheet);

                ExcelHelper excelHelper = new ExcelHelper();
                await Task.Run(() => excelHelper.CreateExcelFile(workbook, excelContent));

                // 儲存暫存檔案
                string cacheFilePath = await SaveShippingCacheAsync();
                if (cacheFilePath == null)
                {
                    MessageBox.Show("儲存暫存檔案失敗！", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // 更新下拉選單
                GetShippingCacheNameList();
                ComboBoxShippingCacheName.SelectedValue = Path.GetFileName(cacheFilePath);

                MessageBox.Show("出貨單已匯出！", "匯出成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"匯出失敗：{ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        // 輔助方法：儲存出貨暫存檔案
        private async Task<string> SaveShippingCacheAsync()
        {
            string shipFilePath = AppSettingConfig.ShipFilePath();
            try
            {
                // 確保目錄存在
                if (!Directory.Exists(shipFilePath))
                {
                    Directory.CreateDirectory(shipFilePath);
                }

                string cacheFilePath;
                if (ComboBoxShippingCacheName.SelectedIndex == -1)
                {
                    // 生成新檔案名稱
                    string datePrefix = $"出貨暫存{DateTime.Now:yyyyMMdd}";
                    var existingFiles = Directory.GetFiles(shipFilePath, $"{datePrefix}*.txt")
                                               .Select(Path.GetFileNameWithoutExtension)
                                               .Where(name => name.StartsWith(datePrefix))
                                               .ToList();

                    int nextNumber = 1;
                    if (existingFiles.Any())
                    {
                        nextNumber = existingFiles
                            .Select(name => int.TryParse(name.Substring(datePrefix.Length + 1), out int num) ? num : 0)
                            .Max() + 1;
                    }
                    cacheFilePath = Path.Combine(shipFilePath, $"{datePrefix}-{nextNumber}.txt");
                }
                else
                {
                    // 使用選中的檔案名稱
                    string selectedFileName = ComboBoxShippingCacheName.SelectedValue?.ToString();
                    if (string.IsNullOrEmpty(selectedFileName))
                    {
                        return null;
                    }
                    cacheFilePath = Path.Combine(shipFilePath, selectedFileName);
                }

                // 序列化並異步寫入檔案
                string json = JsonConvert.SerializeObject(ShippingSheetStructure);
                using (FileStream fs = new FileStream(cacheFilePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
                {
                    byte[] data = Encoding.UTF8.GetBytes(json);
                    await fs.WriteAsync(data, 0, data.Length);
                }

                return cacheFilePath;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Failed to save cache file: {ex.Message}");
                return null;
            }
        }

        private void Weight_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 預設使用當天日期生成檔名 (格式: 出貨yyyymmdd.xlsx)
                string dateStr = DateTime.Today.ToString("yyyyMMdd");
                string fileName = $"出貨{dateStr}.xlsx";
                string filePath = Path.Combine(AppSettingConfig.FilePath(), fileName);

                // 檢查檔案是否存在
                if (!File.Exists(filePath))
                {
                    MessageBox.Show($"錯誤：檔案 {fileName} 不存在於當前目錄。", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // 儲存所有工作表 G7:M20 的總和
                double totalSum = 0.0;
                int totalCustomer = 0;
                StringBuilder sheetResults = new StringBuilder();

                // 讀取 Excel 檔案
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    // 建立 XSSFWorkbook (適用於 .xlsx)
                    IWorkbook workbook = new XSSFWorkbook(fs);

                    // 檢查是否有至少兩個工作表
                    if (workbook.NumberOfSheets < 2)
                    {
                        MessageBox.Show("錯誤：檔案只有一個或零個工作表，無法計算第二個到最後一個工作表的數據。", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // 從第二個工作表 (索引 1) 到最後一個工作表
                    for (int sheetIndex = 1; sheetIndex < workbook.NumberOfSheets; sheetIndex++)
                    {
                        ISheet sheet = workbook.GetSheetAt(sheetIndex);

                        // 定義範圍 G7:M20 (列 6 到 19，欄 6 到 12，0-based 索引)
                        double sheetSum = 0.0;

                        for (int rowIndex = 6; rowIndex <= 19; rowIndex++) // G7 到 M20 的列 (7-20, 0-based 為 6-19)
                        {
                            IRow row = sheet.GetRow(rowIndex);
                            if (row == null) continue; // 略過空列

                            for (int colIndex = 6; colIndex <= 12; colIndex++) // G 到 M 的欄 (G=6, M=12)
                            {
                                ICell cell = row.GetCell(colIndex, MissingCellPolicy.RETURN_NULL_AND_BLANK);
                                if (cell != null && cell.CellType == CellType.Numeric)
                                {
                                    sheetSum += cell.NumericCellValue;
                                }
                                // 非數字或空單元格則忽略
                            }
                        }

                        // 記錄單個工作表的總和
                        sheetResults.AppendLine($"{sheet.SheetName}: {sheetSum:F1} 公斤");
                        totalCustomer++;
                        totalSum += sheetSum;
                    }
                }

                // 顯示結果
                string message = $"{sheetResults.ToString()}\n總客戶:{totalCustomer} \n總重量: {totalSum:F1} 公斤";
                MessageBox.Show(message, "出貨重量計算結果", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"發生錯誤: {ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private List<ExcelSheetContent> ExportToShip(IWorkbook wb)
        {
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

            List<ExcelSheetContent> excelSheetContents = new List<ExcelSheetContent>();
            ExcelCellContent emptyExcelCellContent = new ExcelCellContent();
            foreach (ShippingSheetStructure shippingSheetStucture in ShippingSheetStructure)
            {
                //拆解每一個客戶的出貨資料
                List<ShippingSheetData> shippingSheetDatasExpand = new List<ShippingSheetData>();
                foreach (TextileShippingData textileShippingData in shippingSheetStucture.TextileShippingDatas)
                {
                    foreach (ShippingSheetData shippingSheetData in textileShippingData.ShippingSheetDatas)
                    {
                        shippingSheetDatasExpand.Add(new ShippingSheetData()
                        {
                            Customer = shippingSheetStucture.Customer,
                            TextileName = textileShippingData.TextileName,
                            ColorName = shippingSheetData.ColorName,
                            StorageSpaces = shippingSheetData.StorageSpaces,
                            Memo = shippingSheetData.Memo,
                            CountInventory = shippingSheetData.CountInventory,
                            ShippingNumber = shippingSheetData.ShippingNumber > 7 ? 7 : shippingSheetData.ShippingNumber,
                        });
                        //出貨數量超過7個則換行
                        for (int shippingNumberCount = 0; shippingNumberCount < (shippingSheetData.ShippingNumber - 7) / 7.0; shippingNumberCount++)
                        {
                            if (shippingSheetDatasExpand.Count() <= 13)
                                shippingSheetDatasExpand.Last().ShippingNumber += shippingSheetData.ShippingNumber - (shippingNumberCount + 1) * 7;
                            shippingSheetDatasExpand.Add(new ShippingSheetData()
                            {
                                Customer = shippingSheetStucture.Customer,
                                TextileName = textileShippingData.TextileName,
                                ColorName = shippingSheetDatasExpand.Count() <= 13 ? string.Empty : shippingSheetData.ColorName,//行數未超過13(換頁)的話，則不顯示顏色名稱
                                StorageSpaces = shippingSheetData.StorageSpaces,
                                Memo = shippingSheetData.Memo,
                                CountInventory = shippingSheetData.CountInventory,
                                ShippingNumber = shippingSheetDatasExpand.Count() <= 13 ? 0 : shippingSheetData.ShippingNumber - (shippingNumberCount + 1) * 7,//行數未超過13(換頁)的話，則不顯示數量
                            });
                        }
                    }
                }

                //分頁客戶出貨資料
                List<ShippingSheetStructure> pageShippingSheetStructures = new List<ShippingSheetStructure>();
                int pageNum = 0;
                while (pageNum * 13 < shippingSheetDatasExpand.Count())
                {
                    var pageData = shippingSheetDatasExpand.Skip(pageNum * 13).Take(13).GroupBy(o => o.TextileName).Select(s => new TextileShippingData
                    {
                        TextileName = s.Key,
                        ShippingSheetDatas = s.Select(ss => new ShippingSheetData { ColorName = ss.ColorName, ShippingNumber = ss.ShippingNumber }).ToList()
                    }).ToList();
                    pageShippingSheetStructures.Add(new ShippingSheetStructure
                    {
                        Customer = shippingSheetStucture.Customer + (pageNum > 0 ? "-" + pageNum : string.Empty),
                        TextileShippingDatas = new List<TextileShippingData>(pageData)
                    });
                    pageNum++;
                }

                foreach (ShippingSheetStructure pageShippingSheetStructure in pageShippingSheetStructures)
                {
                    ExcelSheetContent customerShipSheet = new ExcelSheetContent()
                    {
                        SheetName = pageShippingSheetStructure.Customer,
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
                           CellValue = pageShippingSheetStructure.Customer.Split('-')[0],
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
                    foreach (var textileShippingData in pageShippingSheetStructure.TextileShippingDatas)
                    {
                        bool textileNameDisplay = true;
                        foreach (var shippingSheetData in textileShippingData.ShippingSheetDatas)
                        {
                            string colorName = shippingSheetData.ColorName.Split('-')[0];
                            colorName = shippingSheetData.ShippingNumber <= 1 ? colorName : colorName + "-" + shippingSheetData.ShippingNumber;
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
                        }
                    }
                    excelSheetContents.Add(customerShipSheet);
                }
            }
            return excelSheetContents;
        }

        private List<ExcelSheetContent> ExportToExcel(IWorkbook wb)
        {
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

            return ShipExcelData(positionStyle, shipPosition);
        }

        private List<ExcelSheetContent> ShipExcelData(ICellStyle positionStyle, List<ExcelRowContent> shipPosition)
        {
            //ExcelHelper excelHelper = new ExcelHelper();
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
                }
            };
            return excelContent.ExcelSheetContents;
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
            CustomerAmount.Content = ShippingSheetStructure.Count;
            ShippingAmount.Content = ShippingSheetStructure.SelectMany(s => s.TextileShippingDatas).SelectMany(s => s.ShippingSheetDatas).Select(s => s.ShippingNumber).Sum();
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
            if (((ComboBox)sender).SelectedIndex == -1)
                return;
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

            // 取消之前的過濾操作
            _filterCancellationTokenSource?.Cancel();
            _filterCancellationTokenSource = new CancellationTokenSource();

            // 防抖：延遲 300ms 執行過濾
            Task.Delay(300, _filterCancellationTokenSource.Token).ContinueWith(t =>
            {
                if (t.IsCanceled) return;

                // 在 UI 線程執行過濾
                Dispatcher.InvokeAsync(() =>
                {
                    ICollectionView cv = CollectionViewSource.GetDefaultView(DataGridTextileList.ItemsSource);
                    if (cv == null) return;

                    if (!string.IsNullOrEmpty(filterText))
                    {
                        cv.Filter = o =>
                        {
                            if (o is string row)
                            {
                                // 優化字符串比較
                                return row.Contains(filterText);
                            }
                            return false;
                        };
                    }
                    else
                    {
                        cv.Filter = null; // 清除過濾
                    }
                });
            }, TaskScheduler.Default);
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
