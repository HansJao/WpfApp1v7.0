﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DataClass.TrashSystem;

namespace WpfApp1.DataClass.AccountSystem
{
    public class CustomerCheckBillSheet : TrashCustomerShipped
    {
        public int AccountTextileID { get; set; }
        private int _defaultPrice { get; set; }
        public int DefaultPrice
        {
            get { return _defaultPrice; }
            set
            {
                _defaultPrice = value;
                PropertyChangedInvoke(this, new PropertyChangedEventArgs("DefaultPrice"));
            }
        }
        private int _customerPrice { get; set; }
        public int CustomerPrice
        {
            get { return _customerPrice; }
            set
            {
                _customerPrice = value;
                PropertyChangedInvoke(this, new PropertyChangedEventArgs("CustomerPrice"));
            }
        }

    }
}
