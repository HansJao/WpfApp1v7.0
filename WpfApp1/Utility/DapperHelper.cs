﻿using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Utility
{
    public class DapperHelper
    {

        #region Dbf File
        public static IEnumerable<T> QueryDbfCollection<T>(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            IEnumerable<T> result;
            using (var con = new OleDbConnection(connectionString))
            {
                DynamicParameters parameters = parseParameters(commandParameters);
                result = con.Query<T>(commandText, parameters, commandType: commandType, commandTimeout: 30);
            }
            return result;
        }
        public static int ExecuteDbfParameter(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            int result;
            using (var con = new OleDbConnection(connectionString))
            {
                DynamicParameters parameters = parseParameters(commandParameters);
                result = con.Execute(commandText, parameters, commandType: commandType, commandTimeout: 30);
            }
            return result;
        }
        #endregion
        public static int ExecuteParameter(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {

            //var conString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + AppSettingConfig.DbfFilePath() + "; Extended Properties=dBASE IV;");
            //using (var con = new OleDbConnection(conString))
            //{
            //    string sql4 = "SELECT * FROM INVOSUB.dbf";
            //    var test = con.Query<dynamic>(sql4);
            //}
            int result;
            using (var con = new SqlConnection(connectionString))
            {
                DynamicParameters parameters = parseParameters(commandParameters);
                result = con.Execute(commandText, parameters, commandType: commandType, commandTimeout: 30);
            }
            return result;
        }

        public static int Execute(string connectionString, CommandType commandType, string commandText, object commandParameters)
        {
            int result;
            using (var con = new SqlConnection(connectionString))
            {
                result = con.Execute(commandText, commandParameters, commandType: commandType, commandTimeout: 30);
            }
            return result;
        }

        public static IEnumerable<T> QueryCollection<T, P>(string connectionString, CommandType commandType, string commandText, P parameter)
        {
            IEnumerable<T> result;
            using (var con = new SqlConnection(connectionString))
            {
                result = con.Query<T>(commandText, parameter, commandType: commandType, commandTimeout: 30);
            }
            return result;
        }

        public static T Query<T>(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            T result;
            using (var con = new SqlConnection(connectionString))
            {
                DynamicParameters parameters = parseParameters(commandParameters);
                result = con.QueryFirst<T>(commandText, parameters, commandType: commandType, commandTimeout: 30);
            }
            return result;
        }

        public static IEnumerable<T> QueryCollection<T>(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            IEnumerable<T> result;
            using (var con = new SqlConnection(connectionString))
            {
                DynamicParameters parameters = parseParameters(commandParameters);
                result = con.Query<T>(commandText, parameters, commandType: commandType, commandTimeout: 30);
            }
            return result;
        }

        private static DynamicParameters parseParameters(SqlParameter[] commandParameters)
        {
            DynamicParameters result = null;
            if (commandParameters != null)
            {
                result = new DynamicParameters();
                foreach (SqlParameter p in commandParameters)
                {
                    if (p == null) continue;
                    if ((p.Direction == ParameterDirection.Input || p.Direction == ParameterDirection.InputOutput) && (p.Value == null))
                    {
                        p.Value = DBNull.Value;
                    }
                    result.Add(p.ParameterName, p.Value, p.DbType, p.Direction);
                }
            }
            return result;
        }
    }
}
