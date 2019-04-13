using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using COMP4911Timesheets.Models;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Globalization;
using System.Data.SqlClient;
using System.Data;
using System.Text.RegularExpressions;

namespace COMP4911Timesheets.Controllers
{
    public static class Utility
    {
        private static string EncryptionKey = "COMP4911Timesheet";
        public static string ConnectionString { get; set; }
        public static string HashEncrypt(string encryptStr)
        {

            byte[] clearBytes = Encoding.Unicode.GetBytes(encryptStr);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
                    0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
                });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptStr = Convert.ToBase64String(ms.ToArray());
                }
            }
            return encryptStr;

        }

        public static string HashDecrypt(string decryptStr)
        {

            decryptStr = decryptStr.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(decryptStr);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
                    0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
                });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    decryptStr = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return decryptStr;
        }

        public static DateTime GetNextWeekday(DateTime start, DayOfWeek day)
        {
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd);
        }

        public static DateTime GetPreviousWeekday(DateTime start, DayOfWeek day)
        {
            start = start.AddDays(-1);
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd);
        }

        public static int GetWeekNumberByDate(DateTime time)
        {
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        public static void BackupDatabase()
        {
            var matchCollections = Regex.Matches(Utility.ConnectionString, @"(Database=\w+)");
            var matches = matchCollections.Cast<Match>().Select(match => match.Value).ToList();
            if (matches.Count == 0)
            {
                return;
            }
            string connectionDbName = matches.ElementAt(0);
            string toBeSearched = "Database=";
            string databaseName = connectionDbName.Substring(connectionDbName.IndexOf(toBeSearched) + toBeSearched.Length);

            string backupPath = @"./erupt.bak";
            string commandText = $@"BACKUP DATABASE [{databaseName}] TO DISK = N'{backupPath}' WITH NOFORMAT, INIT, NAME = N'{databaseName}-Full Database Backup', SKIP, NOREWIND, NOUNLOAD,  STATS = 10";

            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder
            {
                ConnectionString = Utility.ConnectionString
            };
            using (SqlConnection connection = new SqlConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();
                connection.InfoMessage += Connection_InfoMessage;
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = commandText;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void RestoreDatabase()
        {
            var matchCollections = Regex.Matches(Utility.ConnectionString, @"(Database=\w+)");
            var matches = matchCollections.Cast<Match>().Select(match => match.Value).ToList();
            if (matches.Count == 0)
            {
                return;
            }
            string connectionDbName = matches.ElementAt(0);
            string toBeSearched = "Database=";
            string databaseName = connectionDbName.Substring(connectionDbName.IndexOf(toBeSearched) + toBeSearched.Length);

            string backupPath = @"./erupt.bak";
            string commandText = $@"USE [master];ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; RESTORE DATABASE [{databaseName}] FROM DISK = N'{backupPath}' WITH FILE = 1, NOUNLOAD, REPLACE, STATS = 5; ALTER DATABASE [{databaseName}] SET MULTI_USER;";

            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder
            {
                ConnectionString = Utility.ConnectionString
            };
            using (SqlConnection connection = new SqlConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();
                connection.InfoMessage += Connection_InfoMessage;
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = commandText;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
        }
        private static void Connection_InfoMessage(object sender, SqlInfoMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
