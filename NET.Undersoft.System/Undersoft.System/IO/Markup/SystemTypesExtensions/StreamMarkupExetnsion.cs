namespace System.IO
{
    public static class StreamMarkupExtension
    {
        public static Stream Markup(this Stream stream, long blocksize, MarkupType bytenoise)
        {
            long blockSize = blocksize;
            long blockLeft = stream.Length % blockSize;
            long resize = (blockSize - blockLeft >= 28) ? blockSize - blockLeft - 16 : blockSize + (blockSize - blockLeft) - 16;
            byte[] byteMarkup = new byte[resize].Initialize((byte)bytenoise);
            stream.Write(byteMarkup, 0, (int)resize);
            return stream;
        }
        public static Stream Markup(this Stream stream, int blocksize, MarkupType bytenoise)
        {
            int blockSize = blocksize;
            long blockLeft = stream.Length % blockSize;
            long resize = (blockSize - blockLeft >= 28) ? blockSize - blockLeft - 16 : blockSize + (blockSize - blockLeft) - 16;
            byte[] byteMarkup = new byte[resize].Initialize((byte)bytenoise);
            stream.Write(byteMarkup, 0, (int)resize);
            return stream;
        }

        public static MarkupType SeekMarkup(this Stream stream, SeekOrigin seekorigin = SeekOrigin.Begin, SeekDirection direction = SeekDirection.Forward, int offset = 0, int _length = -1)
        {
            bool isFwd = (direction != SeekDirection.Forward) ? false : true;
            short noiseFlag = 0;
            MarkupType noiseKind = MarkupType.None;
            MarkupType lastKind = MarkupType.None;
            if (stream.Length > 0)
            {
                long length = (_length <= 0) ? stream.Length : _length;
                long saveposition = stream.Position;
                offset += (!isFwd) ? 1 : 0;
                length -= ((!isFwd) ? 0 : 1);

                for (int i = offset; i < length; i++)
                {
                    if (!isFwd)
                        stream.Seek(-i, seekorigin);
                    else
                        stream.Seek(i, seekorigin);

                    byte checknoise = (byte)stream.ReadByte();

                    MarkupType tempKind = MarkupType.None;
                    if (checknoise.IsMarkup(out tempKind))
                    {
                        lastKind = tempKind;
                        noiseFlag++;
                    }
                    else if (noiseFlag >= 16)
                    {
                        noiseKind = lastKind;
                        return noiseKind;
                    }
                    else
                    {
                        lastKind = tempKind;
                        noiseFlag = 0;
                    }
                }
                stream.Position = saveposition;
            }
            return lastKind;
        }

    }

}