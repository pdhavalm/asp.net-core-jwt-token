namespace JWTAuthentication
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    public static class EncryptDecrypt
    {
        private static readonly TripleDESCryptoServiceProvider MDes = new TripleDESCryptoServiceProvider();

        private static readonly byte[] MIv = { 8, 7, 6, 5, 4, 3, 2, 1 };

        private static readonly byte[] MKey =
            { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 14, 13, 15, 16, 17, 18, 19, 20, 21, 22, 24, 23 };

        private static readonly UTF8Encoding MUtf8 = new UTF8Encoding();

        public static string Decrypt(string text)
        {
            var input = Convert.FromBase64String(text);
            var output = Transform(input, MDes.CreateDecryptor(MKey, MIv));
            return MUtf8.GetString(output);
        }

        public static string Encrypt(string text)
        {
            var input = MUtf8.GetBytes(text);
            var output = Transform(input, MDes.CreateEncryptor(MKey, MIv));
            return Convert.ToBase64String(output);
        }

        private static byte[] Transform(byte[] input, ICryptoTransform cryptoTransform)
        {
            // Create the Neccessary streams
            var memStream = new MemoryStream();
            var cryptoStream = new CryptoStream(memStream, cryptoTransform, CryptoStreamMode.Write);

            // transform the bytes as requested
            cryptoStream.Write(input, 0, input.Length);
            cryptoStream.FlushFinalBlock();

            // Reasd the memory stream and convert it to byte array
            memStream.Position = 0;
            var result = new byte[Convert.ToInt32(memStream.Length - 1) + 1];
            memStream.Read(result, 0, Convert.ToInt32(result.Length));
            memStream.Close();
            cryptoStream.Close();
            return result;
        }
    }
}