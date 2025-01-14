using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DecryptApp
{
    public class Encodeservice
    {
        #region Base64
        /// <summary>
        /// 編碼
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string SettingEncryptionBase64(string source)
        {
            DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
            byte[] bytes = Encoding.ASCII.GetBytes("ABCe2019");
            byte[] bytes2 = Encoding.ASCII.GetBytes("2019ABCe");
            byte[] bytes3 = Encoding.UTF8.GetBytes(source);
            dESCryptoServiceProvider.Key = bytes;
            dESCryptoServiceProvider.IV = bytes2;
            string result = "";
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(bytes3, 0, bytes3.Length);
                    cryptoStream.FlushFinalBlock();
                    result = Convert.ToBase64String(memoryStream.ToArray());
                }
            }

            return result;
        }

        /// <summary>
        /// 解碼
        /// </summary>
        /// <param name="encrypt"></param>
        /// <returns></returns>
        public static string desDecryptBase64(string encrypt)
        {
            DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
            byte[] bytes = Encoding.ASCII.GetBytes("ABCe2019");
            byte[] bytes2 = Encoding.ASCII.GetBytes("2019ABCe");
            dESCryptoServiceProvider.Key = bytes;
            dESCryptoServiceProvider.IV = bytes2;
            byte[] array = Convert.FromBase64String(encrypt);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(array, 0, array.Length);
                    cryptoStream.FlushFinalBlock();
                    return Encoding.UTF8.GetString(memoryStream.ToArray());
                }
            }
        }

        /// <summary>
        /// 檢查字符串是否為有效的 Base64 字符串
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsBase64String(string s)
        {
            s = s.Trim();
            return (s.Length % 4 == 0) && System.Text.RegularExpressions.Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", System.Text.RegularExpressions.RegexOptions.None);
        }
        #endregion

        #region 計算雜湊
        /// <summary>
        /// 計算雜湊
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="hashAlgorithm"></param>
        /// <param name="saltBytes"></param>
        /// <returns></returns>
        public static string ComputeHash(string plainText, string hashAlgorithm, byte[] saltBytes)
        {
            saltBytes = new byte[8];
            byte[] bytes = Encoding.UTF8.GetBytes(plainText);
            byte[] array = new byte[bytes.Length + saltBytes.Length];
            for (int i = 0; i < bytes.Length; i++)
            {
                array[i] = bytes[i];
            }

            for (int j = 0; j < saltBytes.Length; j++)
            {
                array[bytes.Length + j] = saltBytes[j];
            }

            using HashAlgorithm hash = (hashAlgorithm.ToUpper() switch
            {
                "SHA1" => new SHA1Managed(),
                "SHA256" => new SHA256Managed(),
                "SHA384" => new SHA384Managed(),
                "SHA512" => new SHA512Managed(),
                _ => new MD5CryptoServiceProvider(),
            });

            // 計算哈希值
            byte[] hashBytes = hash.ComputeHash(array);

            // 創建一個新的陣列來存放哈希值和 saltBytes
            byte[] array3 = new byte[hashBytes.Length + saltBytes.Length];
            Buffer.BlockCopy(hashBytes, 0, array3, 0, hashBytes.Length);
            Buffer.BlockCopy(saltBytes, 0, array3, hashBytes.Length, saltBytes.Length);

            return Convert.ToBase64String(array3);
        }

        #endregion

    }
}
