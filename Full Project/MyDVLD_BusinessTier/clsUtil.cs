using MyDVLD_DataTier;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using static System.Net.Mime.MediaTypeNames;

namespace MyDVLD_BusinessTier
{
    public partial class clsUtil
    {
        public static void CreateFolderIfDoesNotExist(string FolderPath)
        {
            if (!Directory.Exists(FolderPath))
                 Directory.CreateDirectory(FolderPath);
        }

        public static string ReplaceFileNameWithGUID(string sourceFile)
        {
            string extn = new FileInfo(sourceFile).Extension;
            return Guid.NewGuid().ToString() + extn;
        }

        public static bool CopyImageToProjectImagesFolder(ref string sourceFile)
        {
            string DestinationFolder = @"C:\Users\Tonyg\OneDrive\Desktop\Desktop Projects\Course 19 (DVLD) Project\DVLD-People-Images\";

            CreateFolderIfDoesNotExist(DestinationFolder);

            string destinationFile = DestinationFolder + ReplaceFileNameWithGUID(sourceFile);

            File.Copy(sourceFile, destinationFile, true);

            sourceFile = destinationFile;
            return true;
        }

        /// <summary>
        /// Log Event To Windows Event Viewer
        /// </summary>
        /// <param name="Message">Message You Want To Log</param>
        /// <param name="Type">Type Of Event (Error , Warning , Info)</param>
        public static void LogEvent(string Message, EventLogEntryType Type) => clsDB_Util.clsEventLog.LogEvent(Message, Type);

        /// <summary>
        /// This Class Is Resposible For Security...
        /// </summary>
        public static class clsSecurity
        {
            private const string HashPrefix = "sha256:";

            /// <summary>
            /// Hashes The Password Before Inserting To DataBase (Hashing Is One Way You Cant Retrive Password)
            /// </summary>
            /// <param name="UserPassword">User Password</param>
            /// <returns>256 Byte string Of Hexadecimal Code</returns>
            public static string HashPassword(string userPassword)
            {
                if (userPassword.StartsWith(HashPrefix))
                    return userPassword;

                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(userPassword.ToLower().Trim()));
                    return HashPrefix + BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                }
            }

            /// <summary>
            /// Check User Password By Hased Password In DataBase 
            /// </summary>
            /// <param name="UserID">User Id To Find User In DB</param>
            /// <param name="Password">Given Password To Compare</param>
            /// <returns>If Hased Passwords Is Identical</returns>
            public static bool CheckUserPasswordMatchHashed(int UserID, string Password) => clsUsersManagement.Find(UserID).Password == HashPassword(Password.Trim().ToLower());

            /// <summary>
            /// Encrypt Data Given With Provided Key
            /// </summary>
            /// <param name="Text">Data To Encrypt</param>
            /// <returns>New Encrypted String</returns>
            public static string Encrypt(string Text)
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["EncryptionKey"].ToString());
                    aesAlg.IV = new byte[aesAlg.BlockSize / 8];

                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    using (var msEncrypt = new System.IO.MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (var swEncrypt = new System.IO.StreamWriter(csEncrypt))
                            swEncrypt.Write(Text);

                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }

            /// <summary>
            /// Decrypt Data Given
            /// </summary>
            /// <param name="cipherText">Data To Decrypt</param>
            /// <param name="key">Key To Decrypt With</param>
            /// <returns>Decrypted String With Info</returns>
            public static string Decrypt(string Text)
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["EncryptionKey"].ToString());
                    aesAlg.IV = new byte[aesAlg.BlockSize / 8];

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (var msDecrypt = new System.IO.MemoryStream(Convert.FromBase64String(Text)))
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (var srDecrypt = new System.IO.StreamReader(csDecrypt))
                        return srDecrypt.ReadToEnd();
                }
            }
        }
    }
}
