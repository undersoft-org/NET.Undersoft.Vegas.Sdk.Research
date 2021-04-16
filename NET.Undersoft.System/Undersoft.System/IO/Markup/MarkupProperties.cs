namespace System.IO
{
    public struct MarkedSegment
    {
        public long Offset;
        public long Length;
        public int  Count => (int)(Length / ItemSize);
        public int  ItemSize;
        public long ItemOffset(int index)
        {
            return (Offset + (index * ItemSize));
        }
    }

    public enum MarkupType
    {
        None = (byte)0xFF,        
        Block = (byte)0x17,
        End = (byte)0x04,
        Empty = (byte)0x00,
        Line = (byte)0x10,
        Space = (byte)0x32,
        Semi = (byte)0x59,
        Coma = (byte)0x44,
        Colon = (byte)0x58,
        Dot = (byte)0x46,
        Cancel = (byte)0x18,
    }

    public enum SeekDirection
    {
        Forward,
        Backward
    }
}