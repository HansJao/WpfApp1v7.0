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
using WpfApp1.DataClass.Enumeration;
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
            string fileName = string.Concat(AppSettingConfig.FilePath(), "/", AppSettingConfig.StoreManageFileName());
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            _workbook = new XSSFWorkbook(fileStream);  //xlsx數據讀入workbook
            List<string> textileList = new List<string>();
            for (int sheetCount = 1; sheetCount < _workbook.NumberOfSheets; sheetCount++)
            {
                ISheet sheet = _workbook.GetSheetAt(sheetCount);  //獲取第i個工作表  
                textileList.Add(sheet.SheetName);
            }

            ComboBoxTextileList.ItemsSource = textileList;
            ComboBoxTextileList.Loaded += (ls, le) =>
            {
                var targetTextBox = ComboBoxTextileList?.Template.FindName("PART_EditableTextBox", ComboBoxTextileList) as TextBox;

                if (targetTextBox == null) return;

                ComboBoxTextileList.Tag = "TextInput";
                ComboBoxTextileList.StaysOpenOnEdit = true;
                ComboBoxTextileList.IsEditable = true;
                ComboBoxTextileList.IsTextSearchEnabled = false;

                targetTextBox.TextChanged += (o, args) =>
                {
                    var textBox = (TextBox)o;

                    var searchText = textBox.Text;

                    if (ComboBoxTextileList.Tag.ToString() == "Selection")
                    {
                        ComboBoxTextileList.Tag = "TextInput";
                        ComboBoxTextileList.IsDropDownOpen = true;
                    }
                    else if (ComboBoxTextileList.Tag.ToString() == "TextInput" && ComboBoxTextileList.Items.Contains(searchText) || searchText == "　")
                    {
                        return;
                    }
                    else
                    {
                        if (ComboBoxTextileList.SelectionBoxItem != null)
                        {
                            ComboBoxTextileList.SelectedItem = null;
                            targetTextBox.Text = searchText;
                            ComboBoxTextileList.IsDropDownOpen = true;
                            targetTextBox.SelectionStart = targetTextBox.Text.Length;
                        }

                        if (string.IsNullOrEmpty(searchText))
                        {
                            ComboBoxTextileList.Items.Filter = item => true;
                            ComboBoxTextileList.SelectedItem = default(object);
                        }
                        else
                            ComboBoxTextileList.Items.Filter = item =>
                                    item.ToString().ToLower().Contains(searchText.ToLower());

                        //Keyboard.ClearFocus();
                        //Keyboard.Focus(targetTextBox);
                        ComboBoxTextileList.IsDropDownOpen = true;
                        targetTextBox.SelectionStart = targetTextBox.Text.Length;
                    }
                };

                ComboBoxTextileList.SelectionChanged += (o, args) =>
                {
                    var comboBox = o as ComboBox;
                    if (comboBox?.SelectedItem == null) return;
                    comboBox.Tag = "Selection";
                };
            };
            DataGridCustomerName.ItemsSource = CustomerModule.GetCustomerNameList();

            GetShippingCacheNameList();
        }

        private void GetShippingCacheNameList()
        {
            var existsFileName = Directory.GetFiles(AppSettingConfig.FilePath(), "*.txt").Select(System.IO.Path.GetFileName);
            ComboBoxShippingCacheName.ItemsSource = existsFileName.ToList();
        }

        private void ComboBoxTextileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (sender as ComboBox).SelectedItem;
            if (selectedItem == null)
                return;
            string textileName = (sender as ComboBox).SelectedItem.ToString();

            ISheet sheet = _workbook.GetSheet(textileName);  //獲取工作表
            IList selectedTextiles = new ObservableCollection<SelectedTextile>();

            DataGridSelectedTextile.ItemsSource = selectedTextiles;
            IRow row;
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                row = sheet.GetRow(i);
                if (row == null)
                {
                    break;
                }
                var differentCylinder = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.DifferentCylinder) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.DifferentCylinder).CellType == CellType.Blank ? "" : "有不同缸應注意";
                var cellValue = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.CountInventory) == null || (row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.CountInventory).CellType == CellType.Formula ? row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.CountInventory).CachedFormulaResultType == CellType.Error : false) ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.CountInventory).NumericCellValue.ToString(); //獲取i行j列數據
                selectedTextiles.Add(new SelectedTextile
                {
                    ColorName = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ColorName) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ColorName).ToString(),
                    StorageSpaces = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.StorageSpaces) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.StorageSpaces).ToString(),
                    CountInventory = cellValue,
                    ClearFactory = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ClearFactory) == null ? "" : row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.ClearFactory).ToString(),
                    Memo = row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.Memo) == null ? differentCylinder : string.Concat(row.GetCell((int)ExcelEnum.ExcelInventoryColumnIndexEnum.Memo).ToString(), ",", differentCylinder)
                });
            }
        }

        private void SelectedTextile_Click(object sender, RoutedEventArgs e)
        {
            if (((ObservableCollection<SelectedTextile>)DataGridSelectedTextile.ItemsSource).Select(s => s.ShippingNumber).Sum() <= 0)
            {
                MessageBox.Show("未輸入出布數量!!");
                return;
            }
            var customerName = TextBoxCustomerName.Text.ToString();
            var textileName = ComboBoxTextileList.Text.ToString();
            //若表中有此客戶
            if (ShippingSheetStructure.Count > 0 && ShippingSheetStructure.Where(w => w.Customer == customerName).Count() > 0)
            {
                //若客戶中有此布種
                if (ShippingSheetStructure.Where(w => w.Customer == customerName).First().TextileShippingDatas.Count() > 0 && ShippingSheetStructure.Where(w => w.Customer == customerName).First().TextileShippingDatas.Where(w => w.TextileName == textileName).Count() > 0)
                {
                    //將新的布種顏色資料加入原有的布種資料
                    var currentTextileShippingData = ShippingSheetStructure.Where(w => w.Customer == customerName).First().TextileShippingDatas.Where(w => w.TextileName == textileName).First();
                    foreach (var item in DataGridSelectedTextile.Items)
                    {
                        var selectedTextile = item as SelectedTextile;
                        if ((selectedTextile.ShippingNumber == 0 ? 0 : selectedTextile.ShippingNumber) > 0)
                        {
                            currentTextileShippingData.ShippingSheetDatas.Add(new ShippingSheetData
                            {
                                ColorName = selectedTextile.ColorName,
                                CountInventory = selectedTextile.CountInventory,
                                StorageSpaces = selectedTextile.StorageSpaces,
                                ShippingNumber = selectedTextile.ShippingNumber,
                                Memo = selectedTextile.Memo
                            });
                        }
                    }
                }
                else
                {
                    //新增新的布種與顏色
                    var currentTextileShippingDatas = ShippingSheetStructure.Where(w => w.Customer == customerName).First().TextileShippingDatas;
                    var shippingSheetDatas = new List<ShippingSheetData>();
                    foreach (var item in DataGridSelectedTextile.Items)
                    {
                        var selectedTextile = item as SelectedTextile;
                        if ((selectedTextile.ShippingNumber == 0 ? 0 : selectedTextile.ShippingNumber) > 0)
                        {
                            shippingSheetDatas.Add(new ShippingSheetData
                            {
                                ColorName = selectedTextile.ColorName,
                                CountInventory = selectedTextile.CountInventory,
                                StorageSpaces = selectedTextile.StorageSpaces,
                                ShippingNumber = selectedTextile.ShippingNumber,
                                Memo = selectedTextile.Memo
                            });
                        }
                    }
                    currentTextileShippingDatas.Add(new TextileShippingData
                    {
                        TextileName = textileName,
                        ShippingSheetDatas = shippingSheetDatas
                    });
                }
            }
            else
            {
                //新增新的客戶和布種與顏色
                var shippingSheetDatas = new List<ShippingSheetData>();
                foreach (var item in DataGridSelectedTextile.Items)
                {
                    var selectedTextile = item as SelectedTextile;
                    if ((selectedTextile.ShippingNumber == 0 ? 0 : selectedTextile.ShippingNumber) > 0)
                    {
                        shippingSheetDatas.Add(new ShippingSheetData
                        {
                            ColorName = selectedTextile.ColorName,
                            CountInventory = selectedTextile.CountInventory,
                            StorageSpaces = selectedTextile.StorageSpaces,
                            ShippingNumber = selectedTextile.ShippingNumber,
                            Memo = selectedTextile.Memo
                        });
                    }
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
            }
            //每新增一次則重建顯示資料表
            DataGridShippingDisplay();
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            ExportToExcel();
            if (ComboBoxShippingCacheName.SelectedIndex == -1)
            {
                for (int cacheIndex = 1; cacheIndex < 5; cacheIndex++)
                {
                    var fileName = string.Concat("出貨暫存", DateTime.Now.ToString("yyyyMMdd"), "-", cacheIndex, ".txt");

                    var shippingCacheFileName = string.Concat(AppSettingConfig.FilePath(), @"\", fileName);
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
                var shippingCacheFileName = string.Concat(AppSettingConfig.FilePath(), @"\", selectedFileName);

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

        private void ExportToExcel()
        {
            //建立Excel 2003檔案
            IWorkbook wb = new XSSFWorkbook();
            ISheet ws = wb.CreateSheet("Class");
            //ws.Autobreaks = false;
            //ws.PrintSetup.FitWidth = 1;
            //ws.SetColumnBreak(11);
            ws.SetMargin(MarginType.LeftMargin, 0.04);
            ws.SetMargin(MarginType.RightMargin, 0.04);
            XSSFRow row = (XSSFRow)ws.CreateRow(0);
            row.Height = 440;
            ICellStyle cellCenter = wb.CreateCellStyle();
            cellCenter.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;

            ws.SetColumnWidth(0, 3000);
            ws.SetColumnWidth(1, 3150);
            ws.SetColumnWidth(4, 1700);
            ws.SetColumnWidth(2, 1700);

            ICellStyle positionStyle = wb.CreateCellStyle();
            positionStyle.WrapText = true;
            positionStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            positionStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;

            ICellStyle shippingNumberStyle = wb.CreateCellStyle();
            shippingNumberStyle.WrapText = true;
            shippingNumberStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            shippingNumberStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
            shippingNumberStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
            shippingNumberStyle.FillPattern = FillPattern.SolidForeground;

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

            CreateColumnTitle(row, positionStyle);
            int rowIndex = 1;
            int colorIndex = 0;
            int total = 0;
            foreach (var shippingSheetStructure in ShippingSheetStructure)
            {
                XSSFRow rowTextile = (XSSFRow)ws.CreateRow(rowIndex);
                rowTextile.Height = 440;
                CreateCell(rowTextile, 0, shippingSheetStructure.Customer, null);
                int customerTotal = 0;
                foreach (var textileShippingData in shippingSheetStructure.TextileShippingDatas)
                {
                    //判斷若為第0筆資料,則與客戶資料同一行,否則跳下一行
                    if (shippingSheetStructure.TextileShippingDatas.IndexOf(textileShippingData) == 0)
                    {
                        CreateCell(rowTextile, 1, textileShippingData.TextileName, positionStyle);
                    }
                    else
                    {
                        rowIndex++;
                        XSSFRow rowTextileName = (XSSFRow)ws.CreateRow(rowIndex);
                        rowTextileName.Height = 440;
                        CreateCell(rowTextileName, 1, textileShippingData.TextileName, positionStyle);
                    }
                    //判斷是否為配件碼布
                    Regex reg = new Regex(@"碼布$");
                    var isAccessories = reg.IsMatch(textileShippingData.TextileName);
                    ws.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 1, 3));
                    foreach (var shippingSheetData in textileShippingData.ShippingSheetDatas)
                    {
                        rowIndex++;
                        XSSFRow rowColorInfo = (XSSFRow)ws.CreateRow(rowIndex);
                        rowColorInfo.Height = 440;
                        CreateCell(rowColorInfo, 1, shippingSheetData.ColorName, positionStyle);
                        CreateCell(rowColorInfo, 2, shippingSheetData.CountInventory, positionStyle);
                        var colorByStorageSpaces = ExcelHelper.GetColorByStorageSpaces(wb, shippingSheetData.StorageSpaces);
                        if (!string.IsNullOrEmpty(shippingSheetData.StorageSpaces))
                        {
                            var font = wb.CreateFont();
                            font.FontName = "新細明體";
                            font.Boldweight = (short)FontBoldWeight.Bold;
                            colorByStorageSpaces.SetFont(font);
                        }
                        CreateCell(rowColorInfo, 3, shippingSheetData.StorageSpaces, colorByStorageSpaces);
                        CreateCell(rowColorInfo, 4, shippingSheetData.ShippingNumber.ToString(), shippingNumberStyle);
                        CreateCell(rowColorInfo, 5, shippingSheetData.Memo, null);
                        int weightCellNumber = (int)ExcelEnum.ShippingSheetEnum.WeightCellNumber;
                        int startIndex = weightCellNumber + shippingSheetData.Memo.Count() / (int)ExcelEnum.ShippingSheetEnum.CellOfStringLength;
                        for (int shippingCount = startIndex; shippingCount < startIndex + shippingSheetData.ShippingNumber; shippingCount++)
                        {
                            if (shippingCount % weightCellNumber == 0 && shippingCount > weightCellNumber)
                            {
                                rowIndex++;
                                rowColorInfo = (XSSFRow)ws.CreateRow(rowIndex);
                                rowColorInfo.Height = 440;
                            }
                            if (colorIndex % 2 == 0)
                            {
                                CreateCell(rowColorInfo, shippingCount - (shippingCount / weightCellNumber - 1) * weightCellNumber, null, lightTurquoiseStyle);
                            }
                            else
                            {
                                CreateCell(rowColorInfo, shippingCount - (shippingCount / weightCellNumber - 1) * weightCellNumber, null, coralStyle);
                            }
                            if (isAccessories)
                            {
                                break;
                            }
                        }
                        colorIndex++;
                        //如果是配件碼布的話只要加1
                        customerTotal += isAccessories ? 1 : shippingSheetData.ShippingNumber;
                    }
                }
                rowIndex++;
                XSSFRow rowCustomerTotal = (XSSFRow)ws.CreateRow(rowIndex);
                rowCustomerTotal.Height = 440;
                CreateCell(rowCustomerTotal, 3, "總數", positionStyle);
                CreateCell(rowCustomerTotal, 4, customerTotal.ToString(), positionStyle);
                total += customerTotal;
                rowIndex++;
            }
            XSSFRow rowTotal = (XSSFRow)ws.CreateRow(rowIndex);
            rowTotal.Height = 440;
            CreateCell(rowTotal, 3, "總出貨數", positionStyle);
            CreateCell(rowTotal, 4, total.ToString(), positionStyle);
            FileStream file = new FileStream(string.Concat(AppSettingConfig.FilePath(), "/", "出貨", DateTime.Now.ToString("yyyyMMdd"), ".xlsx"), FileMode.Create);//產生檔案
            wb.Write(file);
            file.Close();
        }

        private void CreateColumnTitle(XSSFRow row, ICellStyle positionStyle)
        {
            CreateCell(row, 0, "客戶", positionStyle);
            CreateCell(row, 1, "顏色", positionStyle);
            CreateCell(row, 2, "庫存量", positionStyle);
            CreateCell(row, 3, "儲位", positionStyle);
            CreateCell(row, 4, "出貨數量", positionStyle);
            CreateCell(row, 5, "備註", positionStyle);
            CreateCell(row, 6, "1", positionStyle);
            CreateCell(row, 7, "2", positionStyle);
            CreateCell(row, 8, "3", positionStyle);
            CreateCell(row, 9, "4", positionStyle);
            CreateCell(row, 10, "5", positionStyle);
            CreateCell(row, 11, "6", positionStyle);
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

        private void HttpTest()
        {
            HttpClientService httpClientService = new HttpClientService(HttpClientService.SerializerFormat.Json);
            var header = new Dictionary<string, string>();
            header.Add("authorization", "token 33f38708-65d1-4c88-98da-89f46662b38f");
            //header.Add("Content-Type", "application/json");
            httpClientService.HttpHeader = header;
            Uri uri = new Uri(@"https://jsonbin.org/me/test");
            var myObject = httpClientService.Get<object>(uri);
        }

        private void ComboBoxShippingCacheName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var fileName = ((ComboBox)sender).SelectedValue.ToString();
            var shippingCacheFileName = string.Concat(AppSettingConfig.FilePath(), @"\", fileName);
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

        private void TextBoxCustomerName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                DataGridCustomerName.SelectedIndex = -1;
            }
        }

        private void DataGridCustomerName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextBoxCustomerName.Text = DataGridCustomerName.SelectedItem as string;
        }
    }
}
