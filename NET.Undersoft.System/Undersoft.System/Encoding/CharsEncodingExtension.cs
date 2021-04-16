
using System.Text;

namespace System
{
    public static class CharsEncodingExtension
    {
        public static Char[] ToChars(this Byte[] ba, CharEncoding tf = CharEncoding.ASCII)
        {
            switch (tf)
            {
                case CharEncoding.ASCII:
                    return Encoding.ASCII.GetChars(ba);
                case CharEncoding.UTF8:
                    return Encoding.UTF8.GetChars(ba);
                case CharEncoding.Unicode:
                    return Encoding.Unicode.GetChars(ba);
                default:
                    return Encoding.ASCII.GetChars(ba);
            }
        }
        public static Char[] ToChars(this Byte ba, CharEncoding tf = CharEncoding.ASCII)
        {
            switch (tf)
            {
                case CharEncoding.ASCII:
                    return Encoding.ASCII.GetChars(new byte[] { ba });
                case CharEncoding.UTF8:
                    return Encoding.UTF8.GetChars(new byte[] { ba });
                case CharEncoding.Unicode:
                    return Encoding.Unicode.GetChars(new byte[] { ba });
                default:
                    return Encoding.ASCII.GetChars(new byte[] { ba });
            }
        }

        public static Byte[] ToBytes(this String ca, CharEncoding tf = CharEncoding.ASCII)
        {
            switch (tf)
            {
                case CharEncoding.ASCII:
                    return Encoding.ASCII.GetBytes(ca);
                case CharEncoding.UTF8:
                    return Encoding.UTF8.GetBytes(ca);
                case CharEncoding.Unicode:
                    return Encoding.Unicode.GetBytes(ca);
                default:
                    return Encoding.ASCII.GetBytes(ca);
            }
        }
        public static Byte[] ToBytes(this Char[] ca, CharEncoding tf = CharEncoding.ASCII)
        {
            switch (tf)
            {
                case CharEncoding.ASCII:
                    return Encoding.ASCII.GetBytes(ca);
                case CharEncoding.UTF8:
                    return Encoding.UTF8.GetBytes(ca);
                case CharEncoding.Unicode:
                    return Encoding.Unicode.GetBytes(ca);
                default:
                    return Encoding.ASCII.GetBytes(ca);
            }
        }
        public static Byte[] ToBytes(this Char ca, CharEncoding tf = CharEncoding.ASCII)
        {
            switch (tf)
            {
                case CharEncoding.ASCII:
                    return Encoding.ASCII.GetBytes(new char[] { ca });
                case CharEncoding.UTF8:
                    return Encoding.UTF8.GetBytes(new char[] { ca });
                case CharEncoding.Unicode:
                    return Encoding.Unicode.GetBytes(new char[] { ca });
                default:
                    return Encoding.ASCII.GetBytes(new char[] { ca });
            }
        }
    }

    public enum CharEncoding
    {
        ASCII,
        UTF8,
        Unicode
    }
}
