﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Utility;

namespace WpfApp1
{
    public class AppSettingConfig
    {
        public static int MainWindowHeigh()
        {
            return getAppSettings("MainWindowHeigh").ToInt();
        }
        public static int MainWindowWidth()
        {
            return getAppSettings("MainWindowWidth").ToInt();
        }
        public static IEnumerable<string> CustomerList()
        {
           return getAppSettings("CustomerList").Split(',').ToList();
        }

        public  static string FilePath()
        {
            return getAppSettings("FilePath");
        }
        public static string ShipFilePath()
        {
            return getAppSettings("ShipFilePath");
        }

        public static string StoreManageFileName()
        {
            return getAppSettings("StoreManageFileName");
        }

        internal static string DbfConnectionString()
        {
            return getAppSettings("DbfConnectionString");
        }

        public static string ShippingFileName()
        {
            return getAppSettings("ShippingFileName");
        }
        public static string DbfFilePath()
        {
            return getAppSettings("DbfFilePath");
        }
        public static string ReportsFilePath()
        {
            return getAppSettings("ReportsFilePath");
        }
        public static string StoreSearchFilePath()
        {
            return getAppSettings("StoreSearchFilePath");
        }

        public static string ConnectionString()
        {
            return getAppSettings("ConnectionString");
        }

        public static string ProcessOrderFilePath()
        {
            return getAppSettings("ProcessOrderFilePath");
        }

        public static string ProcessOrderFileName()
        {
            return getAppSettings("ProcessOrderFileName");
        }

        public static string ProcessOrderRecordDateFilePath()
        {
            return getAppSettings("ProcessOrderRecordDateFilePath");
        }
        public static string ProcessOrderRecordDateFileName()
        {
            return getAppSettings("ProcessOrderRecordDateFileName");
        }
        public static string InventoryHistoryRecordFilePath()
        {
            return getAppSettings("InventoryHistoryRecordFilePath");
        }
        public static string TextileNameMappingFilePath()
        {
            return getAppSettings("TextileNameMappingFilePath");
        }

        #region Helper
        /// <summary>
        /// Get AppSettings Value
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>value</returns>
        private static string getAppSettings(string key)
        {
            return ConfigurationManager.AppSettings[key] ?? string.Empty;
        }

        /// <summary>
        /// Get AppSettings Value
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="defaultValue">預設值</param>
        /// <returns>value</returns>
        private static string getAppSettings(string key, string defaultValue)
        {
            return ConfigurationManager.AppSettings[key] ?? defaultValue;
        }
        #endregion Helper       
    }
}
