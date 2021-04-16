using System;

/*************************************************************************************
    Copyright (c) 2020 Undersoft

    System.Submix
              
    @author Darius Hanc                                                  
    @project NETStandard.Undersoft.SDK                      
    @version 0.7.1.r.d (Feb 7, 2021)                                            
    @licence MIT                                       
 *********************************************************************************/
namespace System
{
    public static class Submix
    {        
        public static uint  Map(uint src, uint dest)
        {            
            uint subsrc, 
                sumsub = 0, 
                _src = src;

            int msbid = (int)Bitscan.ReverseIndex32(dest);
            uint bitmask = 0xFFFFFFFF >> (31 - msbid);
          
            for(; ;)
            {
                subsrc = (_src & bitmask);

                if (subsrc > dest)
                    subsrc -= dest;

                sumsub += subsrc;

                if (sumsub > dest)
                    sumsub -= dest;

                _src >>= msbid;

                if (_src == 0)
                    return sumsub;
            }           
        }
        public static uint Map(int src, int dest)
        {
            uint subsrc, 
                sumsub = 0, 
                _src = (uint)src, 
                _dest = (uint)dest;

            int msbid = (int)Bitscan.ReverseIndex32((uint)dest);
            uint bitmask = 0xFFFFFFFF >> (31 - msbid);

            for (; ; )
            {
                subsrc = (_src & bitmask);

                if (subsrc > _dest)
                    subsrc -= _dest;

                sumsub += subsrc;

                if (sumsub > _dest)
                    sumsub -= _dest;
            
                _src >>= msbid;

                if (_src == 0)
                    return sumsub;
            }          
        }
        public static uint Map(uint src, uint dest, uint bitmask, int msbid)
        {
            uint subsrc,
                sumsub = 0,
                _src = src;

            for (; ;)
            {
                subsrc = (_src & bitmask);

                if (subsrc > dest)
                    subsrc -= dest;

                sumsub += subsrc;

                if (sumsub > dest)
                    sumsub -= dest;

                _src >>= msbid;

                if (_src == 0)
                    return sumsub;
            }
        }
        public static uint Map(int src, int dest, uint bitmask, int msbid)
        {
            uint subsrc,  
                sumsub = 0, 
                _src = (uint)src, 
                _dest = (uint)dest;

            for (; ; )
            {
                subsrc = (_src & bitmask);

                if (subsrc > _dest)
                    subsrc -= _dest;

                sumsub += subsrc;

                if (sumsub > _dest)
                    sumsub -= _dest;

                _src >>= msbid;

                if (_src == 0)
                    return sumsub;
            }
        }
        public static uint Mask(int dest)
        {
            int msbid = (int)Bitscan.ReverseIndex32((uint)dest);
            return 0xFFFFFFFF >> (31 - msbid);
        }
        public static uint Mask(uint dest)
        {
            int msbid = (int)Bitscan.ReverseIndex32(dest);
            return  0xFFFFFFFF >> (31 - msbid);
        }
        public static int MsbId(int dest)
        {
            return (int)Bitscan.ReverseIndex32((uint)dest);
        }
        public static int MsbId(uint dest)
        {
            return (int)Bitscan.ReverseIndex32(dest);
        }

        public static ulong Map(ulong src, ulong dest)
        {
            int msbid = (int)Bitscan.ReverseIndex64(dest);
            ulong bitmask = 0xFFFFFFFFFFFFFFFF >> (63 - msbid);
            ulong subsrc, sumsub = 0, _src = src;

            for(; ; )
            {
                subsrc = (_src & bitmask);

                if (subsrc > dest)
                    subsrc -= dest;

                sumsub += subsrc;

                if (sumsub > dest)
                    sumsub -= dest;

                _src >>= msbid;

                if (_src == 0)
                    return sumsub;
            }                    
        }
        public static ulong Map(long src, long dest)
        {
            int msbid = (int)Bitscan.ReverseIndex64((ulong)dest);
            ulong bitmask = 0xFFFFFFFFFFFFFFFF >> (63 - msbid);
            ulong subsrc, sumsub = 0, _src = (ulong)src, _dest = (ulong)dest;
          
            for (; ; )
            {
                subsrc = (_src & bitmask);

                if (subsrc > _dest)
                    subsrc -= _dest;

                sumsub += subsrc;

                if (sumsub > _dest)
                    sumsub -= _dest;

                _src >>= msbid;

                if (_src == 0)
                    return sumsub;
            }
        }
        public static ulong Map(ulong src, ulong dest, ulong bitmask, int msbid)
        {
            ulong subsrc, sumsub = 0, _src = src;

            for (; ; )
            {
                subsrc = (_src & bitmask);

                if (subsrc > dest)
                    subsrc -= dest;

                sumsub += subsrc;

                if (sumsub > dest)
                    sumsub -= dest;

                _src >>= msbid;

                if (_src == 0)
                    return sumsub;
            }
        }
        public static ulong Map(long src, long dest, ulong bitmask, int msbid)
        {
            ulong subsrc, sumsub = 0, _src = (ulong)src, _dest = (ulong)dest;

            for (; ; )
            {
                subsrc = (_src & bitmask);

                if (subsrc > _dest)
                    subsrc -= _dest;

                sumsub += subsrc;

                if (sumsub > _dest)
                    sumsub -= _dest;

                _src >>= msbid;

                if (_src == 0)
                    return sumsub;
            }
        }
        public static ulong Mask(long dest)
        {
            int msbId = (int)Bitscan.ReverseIndex64((ulong)dest);
            return 0xFFFFFFFFFFFFFFFF >> (63 - msbId);
        }
        public static ulong Mask(ulong dest)
        {
            int msbId = (int)Bitscan.ReverseIndex64(dest);
            return 0xFFFFFFFFFFFFFFFF >> (63 - msbId);
        }
        public static int MsbId(long dest)
        {
            return (int)Bitscan.ReverseIndex64((ulong)dest);
        }
        public static int MsbId(ulong dest)
        {
            return (int)Bitscan.ReverseIndex64(dest);
        }
    }
}
