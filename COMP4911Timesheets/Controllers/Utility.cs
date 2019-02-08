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

namespace COMP4911Timesheets.Controllers {
    public static class Unility {
        private static string EncryptionKey = "COMP4911Timesheet";  
        public static string HashEncrypt(string encryptStr)  {
            
            byte[] clearBytes = Encoding.Unicode.GetBytes(encryptStr);  
            using(Aes encryptor = Aes.Create())   {  
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {  
                    0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76  
                });  
                encryptor.Key = pdb.GetBytes(32);  
                encryptor.IV = pdb.GetBytes(16);  
                using(MemoryStream ms = new MemoryStream())  
                {  
                    using(CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write)) {  
                        cs.Write(clearBytes, 0, clearBytes.Length);  
                        cs.Close();  
                    }  
                    encryptStr = Convert.ToBase64String(ms.ToArray());  
                }  
            }  
            return encryptStr;  

        }
 
        public static string HashDecrypt(string decryptStr) {  
     
            decryptStr = decryptStr.Replace(" ", "+");  
            byte[] cipherBytes = Convert.FromBase64String(decryptStr);  
            using(Aes encryptor = Aes.Create())  {  
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {  
                    0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76  
                });  
                encryptor.Key = pdb.GetBytes(32);  
                encryptor.IV = pdb.GetBytes(16);  
                using(MemoryStream ms = new MemoryStream())   
                {  
                    using(CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write)) {  
                        cs.Write(cipherBytes, 0, cipherBytes.Length);  
                        cs.Close();  
                    }  
                    decryptStr = Encoding.Unicode.GetString(ms.ToArray());  
                }  
            }  
            return decryptStr;  
        }
    }
}
