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

namespace WpfApp1.Windows.CommonWindows
{
    /// <summary>
    /// TextBoxMessageDialog.xaml 的互動邏輯
    /// </summary>
    public partial class TextBoxMessageDialog : Window
    {
        public TextBoxMessageDialog()
        {
            InitializeComponent();
            TextBoxProportion.Focus();
        }

        private void ButonDialogOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
