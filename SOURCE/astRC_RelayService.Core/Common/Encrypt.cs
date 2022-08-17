using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Common
{
    public static class Encrypt
    {
        private const string password = "hK9VMw)smB5.8wFee6W42|Zg";
        private static readonly byte[] salt = System.Text.Encoding.UTF8.GetBytes("q2VJT(DHZq2j/iXGvA9|P)Wr");
        public static Encoding Encoding { get; } = Encoding.UTF8;

        private static void GenerateKey(
            int keySize, out byte[] key, int blockSize, out byte[] iv)
        {
            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt))
            {
                key = deriveBytes.GetBytes(keySize / 8);
                iv = deriveBytes.GetBytes(blockSize / 8);
            }
        }

        private static void EnsureDirectory(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath))
            {
                return;
            }
            if (Directory.Exists(directoryPath))
            {
                return;
            }

            Directory.CreateDirectory(directoryPath);
        }

        /// <summary>
        ///   文字列を暗号化してファイルへ出力する
        /// </summary>
        /// <param name = "filePath">保存先ファイル名</param>
        /// <param name = "content">暗号化する文字列</param>
        public static void EncryptToFile(string filePath, string content)
        {
            if (filePath is null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("Parameter can not be empty.", nameof(filePath));
            }
            if (content is null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            EnsureDirectory(Path.GetDirectoryName(filePath));

            byte[] contentBytes = Encoding.GetBytes(content);
            var encBytes = EncryptBytes(contentBytes);
            File.WriteAllBytes(filePath, encBytes);
        }

        private static byte[] EncryptBytes(byte[] contentBytes)
        {
            using (var rijndael = Aes.Create())
            {
                GenerateKey(rijndael.KeySize, out byte[] key, rijndael.BlockSize, out byte[] iv);
                rijndael.Key = key;
                rijndael.IV = iv;

                using (var encryptor = rijndael.CreateEncryptor())
                {
                    return encryptor.TransformFinalBlock(contentBytes, 0, contentBytes.Length);
                }
            }
        }

        public static string EncryptToString(string content)
        {
            if (content is null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            byte[] contentBytes = Encoding.GetBytes(content);
            var encBytes = EncryptBytes(contentBytes);
            return Convert.ToBase64String(encBytes);
        }

        /// <summary>
        ///   ファイルを複合化して文字列を取得する
        /// </summary>
        /// <param name = "filePath">複合化するファイル名</param>
        /// <returns>複合化された文字列</returns>
        public static string DecryptFromFile(string filePath)
        {
            if (filePath is null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("Parameter can not be empty.", nameof(filePath));
            }

            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                var bytes = new byte[1 + fileStream.Length];
                var readedLength = fileStream.Read(bytes, 0, bytes.Length);

                return DecryptBytes(bytes, readedLength);
            }
        }

        private static string DecryptBytes(byte[] sourceBytes, int readedLength)
        {
            using (var rijndael = Aes.Create())
            {
                GenerateKey(
                    rijndael.KeySize, out byte[] key, rijndael.BlockSize, out byte[] iv);
                rijndael.Key = key;
                rijndael.IV = iv;

                using (var decryptor = rijndael.CreateDecryptor())
                {
                    byte[] decBytes = decryptor.TransformFinalBlock(sourceBytes, 0, readedLength);
                    return Encoding.GetString(decBytes);
                }
            }
        }

        public static string DecryptToString(string encryptedString)
        {
            if (string.IsNullOrEmpty(encryptedString))
            {
                return "";
            }

            var sourceBytes = Convert.FromBase64String(encryptedString);
            return DecryptBytes(sourceBytes, sourceBytes.Length);
        }
    }
}
