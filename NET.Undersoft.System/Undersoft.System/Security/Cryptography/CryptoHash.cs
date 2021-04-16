using System;
using System.Text;
using System.Security.Cryptography;

namespace System
{
    public static class CryptoHashkey
    {
        public static string alghoritm = "HMACSHA256";

        public static bool Verify(string hashedPassword, string hashedSalt, string providedPassword)
        {
            string passwordHash = hashedPassword;
            const int format = 1;
            string salt = hashedSalt;
            if (String.Equals(Encrypt(providedPassword, format, salt), passwordHash, StringComparison.CurrentCultureIgnoreCase))
                return true;
            else
                return false;
        }

        public static string Salt()
        {
            return Convert.ToBase64String(DateTime.Now.ToLongDateString().ToBytes(CharEncoding.ASCII));
        }

        public static string Encrypt(string pass, int format, string salt)
        {
            if (format == 0)
                return pass;

            byte[] bIn = Encoding.Unicode.GetBytes(pass);
            byte[] bSalt = Convert.FromBase64String(salt);
            byte[] bRet = null;

            if (format == 1)
            {
                HMACSHA512 hm = new HMACSHA512();

                if (hm is KeyedHashAlgorithm)
                {
                    KeyedHashAlgorithm kha = (KeyedHashAlgorithm)hm;
                    if (kha.Key.Length == bSalt.Length)
                    {
                        kha.Key = bSalt;
                    }
                    else if (kha.Key.Length < bSalt.Length)
                    {
                        byte[] bKey = new byte[kha.Key.Length];
                        Buffer.BlockCopy(bSalt, 0, bKey, 0, bKey.Length);
                        kha.Key = bKey;
                    }
                    else
                    {
                        byte[] bKey = new byte[kha.Key.Length];
                        for (int iter = 0; iter < bKey.Length;)
                        {
                            int len = Math.Min(bSalt.Length, bKey.Length - iter);
                            Buffer.BlockCopy(bSalt, 0, bKey, iter, len);
                            iter += len;
                        }
                        kha.Key = bKey;
                    }
                    bRet = kha.ComputeHash(bIn);
                }
                else
                {
                    byte[] bAll = new byte[bSalt.Length + bIn.Length];
                    Buffer.BlockCopy(bSalt, 0, bAll, 0, bSalt.Length);
                    Buffer.BlockCopy(bIn, 0, bAll, bSalt.Length, bIn.Length);
                    bRet = hm.ComputeHash(bAll);
                }
            }
            return Convert.ToBase64String(bRet);
        }
    }
}
