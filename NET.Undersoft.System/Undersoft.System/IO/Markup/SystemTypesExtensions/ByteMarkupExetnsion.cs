namespace System.IO
{
    public static class ByteMarkupExtension
    {
        public static bool IsMarkup(this byte checknoise, out MarkupType noisekind)
        {
            switch (checknoise)
            { 
                case (byte)MarkupType.Block:
                    noisekind = MarkupType.Block;
                    return true;
                case (byte)MarkupType.End:
                    noisekind = MarkupType.End;
                    return true;
                case (byte)MarkupType.Empty:
                    noisekind = MarkupType.Empty;
                    return false;
                default:
                    noisekind = MarkupType.None;
                    return false;
            }
        }
        public static bool IsSpliter(this byte checknoise, out MarkupType spliterkind)
        {
            switch (checknoise)
            {
                case (byte)MarkupType.Empty:
                    spliterkind = MarkupType.Empty;
                    return true;
                case (byte)MarkupType.Line:
                    spliterkind = MarkupType.Line;
                    return true;
                case (byte)MarkupType.Space:
                    spliterkind = MarkupType.Space;
                    return true;
                case (byte)MarkupType.Semi:
                    spliterkind = MarkupType.Semi;
                    return true;
                case (byte)MarkupType.Coma:
                    spliterkind = MarkupType.Coma;
                    return true;
                case (byte)MarkupType.Colon:
                    spliterkind = MarkupType.Colon;
                    return true;
                case (byte)MarkupType.Dot:
                    spliterkind = MarkupType.Dot;
                    return true;
                default:
                    spliterkind = MarkupType.None;
                    return false;
            }
        }
    }
}