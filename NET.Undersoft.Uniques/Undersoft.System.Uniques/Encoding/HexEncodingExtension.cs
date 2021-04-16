
using System.Text;

namespace System
{
    public static class HexEncodingExtension
    {       
        private static String byteToHex(Byte[] bytes, bool trim = false)
        {
            StringBuilder s = new StringBuilder();
            int length = bytes.Length;
            if (trim)
            {
                foreach (byte b in bytes)
                    if (b == 0)
                        length--;
                    else break;
            }
            for (int i = 0; i < length; i++)
                s.Append(bytes[i].ToString("x2").ToUpper());
            return s.ToString();
        }
        private static Byte[] hexToByte(String hex, int length = -1)
        {
            if (length < 0)
                length = hex.Length;
            byte[] r = new byte[length / 2];
            for (int i = 0; i < length - 1; i += 2)
            {
                byte a = getHex(hex[i]);
                byte b = getHex(hex[i + 1]);
                r[i / 2] = (byte)(a * 16 + b);
            }
            return r;
        }
        private static Byte getHex(Char x)
        {
            if (x <= '9' && x >= '0')
            {
                return (byte)(x - '0');
            }
            else if (x <= 'z' && x >= 'a')
            {
                return (byte)(x - 'a' + 10);
            }
            else if (x <= 'Z' && x >= 'A')
            {
                return (byte)(x - 'A' + 10);
            }
            return 0;
        }

        public static Byte[] FromHex(this String hex)
        {
            return hexToByte(hex);
        }
        public static String ToHex(this Byte[] ba, bool trim = false)
        {
            return byteToHex(ba, trim);
        }

    }
}
