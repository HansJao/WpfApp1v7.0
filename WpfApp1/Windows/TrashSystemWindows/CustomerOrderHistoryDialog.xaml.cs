﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfApp1.DataClass.TrashSystem;
using WpfApp1.ViewModel.TrashSystemViewModel.WindowViewModel;

namespace WpfApp1.Windows.TrashSystemWindows
{
    /// <summary>
    /// CustomerOrderHistoryDialog.xaml 的互動邏輯
    /// </summary>
    public partial class CustomerOrderHistoryDialog : Window
    {
        public CustomerOrderHistoryDialog(TrashCustomer trashCustomer, DateTime begin, DateTime end)
        {
            InitializeComponent();
            this.DataContext = new CustomerOrderHistoryViewModel(trashCustomer, begin, end);
        }
    }
}
