using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace WpfApp1.Utility
{
    public class ControllerHelper
    {
        public static void CreateDataGridTextColumn(DataGrid dataGrid, string Header, string BindingName, string stringFormat)
        {
            DataGridTextColumn column = new DataGridTextColumn
            {
                Header = Header,
                Binding = new Binding(BindingName)
                {
                    StringFormat = stringFormat
                }
            };
            dataGrid.Columns.Add(column);
        }
    }
}
