using System.Runtime.CompilerServices; 

namespace System
{
    public static class HexTetraEncodingExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ToHexTetraByte(this char phchar)
        {
            if (phchar <= '.')
                return (byte)(phchar + 17); //0-9
            else if (phchar <= '9')
                return (byte)(phchar - 48); //A-Z
            else if (phchar <= 'Z')
                return (byte)(phchar - 55); //a-z
            return (byte)(phchar - 61);      //- or .
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ToHexTetraChar(this byte phbyte)
        {
            if (phbyte <= 9)
                return (char)(phbyte + 48); //0-9
            else if (phbyte <= 35)
                return (char)(phbyte + 55); //A-Z
            else if (phbyte <= 61)
                return (char)(phbyte + 61); //a-z
            return (char)(phbyte - 17);      //- or .
        }

        // Character spectrum - below array is not used in algorithm - informational only  
        private static readonly char[] _base64 = new[]{
            '0','1','2','3','4','5','6','7','8','9','A',
            'B','C','D','E','F','G','H','I','J','K','a',
            'b','c','d','e','f','g','h','i','j','k','L',
            'M','N','O','P','Q','R','S','T','U','V','W',
            'X','Y','Z','l','m','n','o','p','q','r','s',
            't','u','v','w','x','y','z','-','.'};
    }
}
