/*************************************************************************************
    Copyright (c) 2020 Undersoft

    System.Multemic.Basedeck.TetraCount
              
    @author Darius Hanc                                                  
    @project NETStandard.Undersoft.SDK                      
    @version 0.7.1.r.d (Feb 7, 2020)                                            
    @licence MIT                                       
 *********************************************************************************/
namespace System.Multemic.Basedeck
{
    public struct TetraCount
    {
        public unsafe int this[int id]
        {
            get
            {
                fixed (TetraCount* a = &this)
                    return *&((int*)a)[id];
            }
            set
            {
                fixed (TetraCount* a = &this)
                    *&((int*)a)[id] = value;
            }
        }

        public unsafe int Increment(int id)
        {
            fixed (TetraCount* a = &this)
                return ++(*&((int*)a)[id]);
        }
        public unsafe int Decrement(int id)
        {
            fixed (TetraCount* a = &this)
                return --(*&((int*)a)[id]);
        }

        public unsafe void Reset(int id)
        {
            fixed (TetraCount* a = &this)
            {
                (*&((int*)a)[id]) = 0;
            }
        }

        public unsafe void ResetAll()
        {
            fixed (TetraCount* a = &this)
            {
                (*&((long*)a)[0]) = 0L;
                (*&((long*)a)[1]) = 0L;
            }
        }

        public int EvenPositiveCount;
        public int OddPositiveCount;
        public int EvenNegativeCount;
        public int OddNegativeCount;
    }

}
