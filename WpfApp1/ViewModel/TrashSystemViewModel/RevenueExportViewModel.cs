using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.ExcelDataClass;
using WpfApp1.DataClass.TrashSystem;
using WpfApp1.Modules.TrashModule;
using WpfApp1.Modules.TrashModule.Implement;
using WpfApp1.Utility;

namespace WpfApp1.ViewModel.TrashSystemViewModel
{
    public class RevenueExportViewModel
    {
        protected ITrashModule TrashModule { get; } = new TrashModule();
        public RevenueExportViewModel(string revenueDate)//2021-5
        {
            DateTime date = DateTime.Parse(revenueDate);
            List<TrashCustomerShipped> invoSubList = new List<TrashCustomerShipped>(TrashModule.GetInvoSub(date));

            List<CustomerRevenue> customerRevenue = invoSubList.GroupBy(g => new
            {
                cusotmerID = g.C_01,
                customerName = g.C_Name
            }).Select(s => new CustomerRevenue
            {
                CustomerID = s.Key.cusotmerID,
                CustomerName = s.Key.customerName,
                Revenue = s.Sum(sum => Math.Ceiling(sum.Price * sum.Quantity))
            }).OrderBy(o => o.CustomerID).ToList();

            customerRevenue.Add(new CustomerRevenue
            {
                CustomerName = "總金額",
                Revenue = customerRevenue.Sum(s => s.Revenue)
            });


            List<List<ExcelCellContent>> excelRowContent = new List<List<ExcelCellContent>>();
            foreach (var item in customerRevenue)
            {
                List<ExcelCellContent> excelCellContents = new List<ExcelCellContent>
                {
                    new ExcelCellContent{CellValue = item.CustomerName},
                    new ExcelCellContent{CellValue = item.Revenue.ToString() }
                };
                excelRowContent.Add(new List<ExcelCellContent>(excelCellContents));
            };

            var excelHelper = new ExcelHelper();
            ExcelContent excelContent = new ExcelContent
            {
                FileName = string.Concat("營收表", date.ToString("yyyy-MM")),
                ExcelSheetContents = new List<ExcelSheetContent>
                {
                    new ExcelSheetContent
                    {
                        SheetName = "營業額",
                        ExcelColumnContents = new List<ExcelCellContent>
                        {
                            new ExcelCellContent
                            {
                                CellValue = "客戶名稱",
                                Width = 3000
                            },
                            new ExcelCellContent
                            {
                                CellValue = "營業額",
                                Width = 5000
                            },
                            new ExcelCellContent
                            {
                                CellValue = "實收金額",
                                Width = 5000
                            },
                            new ExcelCellContent
                            {
                                CellValue = "差額",
                                Width = 5000
                            },
                            new ExcelCellContent
                            {
                                CellValue = "0.99",
                                Width = 5000
                            },
                            new ExcelCellContent
                            {
                                CellValue = "單價折",
                                Width = 5000
                            },
                            new ExcelCellContent
                            {
                                CellValue = "故障折",
                                Width = 5000
                            },
                            new ExcelCellContent
                            {
                                CellValue = "尾數折",
                                Width = 5000
                            },
                            new ExcelCellContent
                            {
                                CellValue = "稅金發票1",
                                Width = 5000
                            },
                            new ExcelCellContent
                            {
                                CellValue = "稅金發票2",
                                Width = 5000
                            },
                            new ExcelCellContent
                            {
                                CellValue = "稅金發票3",
                                Width = 5000
                            }
                        },
                        ExcelRowContents = excelRowContent
                    },
                    new ExcelSheetContent
                    {
                        SheetName = "人事開銷",
                        ExcelColumnContents = new List<ExcelCellContent>(),
                        ExcelRowContents = new List<List<ExcelCellContent>>()
                    },
                    new ExcelSheetContent
                    {
                        SheetName = "基本開銷",
                        ExcelColumnContents = new List<ExcelCellContent>(),
                        ExcelRowContents = new List<List<ExcelCellContent>>()
                    },
                    new ExcelSheetContent
                    {
                        SheetName = "紗商",
                        ExcelColumnContents = new List<ExcelCellContent>(),
                        ExcelRowContents = new List<List<ExcelCellContent>>()
                    },
                    new ExcelSheetContent
                    {
                        SheetName = "織廠",
                        ExcelColumnContents = new List<ExcelCellContent>(),
                        ExcelRowContents = new List<List<ExcelCellContent>>()
                    },
                    new ExcelSheetContent
                    {
                        SheetName = "加工廠",
                        ExcelColumnContents = new List<ExcelCellContent>(),
                        ExcelRowContents = new List<List<ExcelCellContent>>()
                    },
                    new ExcelSheetContent
                    {
                        SheetName = "其他開銷",
                        ExcelColumnContents = new List<ExcelCellContent>(),
                        ExcelRowContents = new List<List<ExcelCellContent>>()
                    }
                }
            };
            IWorkbook wb = new XSSFWorkbook();
            excelHelper.CreateExcelFile(wb, excelContent);
        }

        private void CreateCustomerShippedExcelAction(IWorkbook wb, ISheet ws, ICellStyle positionStyle, ref int rowIndex, CustomerRevenue storeData)
        {
            ICellStyle estyle = wb.CreateCellStyle();
            estyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Yellow.Index;
            estyle.FillPattern = FillPattern.SolidForeground;

            ICellStyle a2style = wb.CreateCellStyle();
            a2style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Coral.Index;
            a2style.FillPattern = FillPattern.SolidForeground;

            XSSFRow rowTextile = (XSSFRow)ws.CreateRow(rowIndex);
            ExcelHelper.CreateCell(rowTextile, 0, storeData.CustomerName, positionStyle);
            ExcelHelper.CreateCell(rowTextile, 1, storeData.Revenue.ToString(), positionStyle);

            rowIndex++;
        }
    }
    public class CustomerRevenue
    {
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public double Revenue { get; set; }
    }
}
