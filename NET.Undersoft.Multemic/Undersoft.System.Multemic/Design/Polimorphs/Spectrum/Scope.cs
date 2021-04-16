/*********************************************************************************       
    Copyright (c) 2020 Undersoft

    System.Multemic.vEBTreeLevel
    
    @authors Darius Hanc & PhD Radek Rudek 
    @project NETStandard.Undersoft.SDK                                    
    @version 0.8.D (Feb 7, 2020)                                           
    @licence MIT
 **********************************************************************************/
namespace System.Multemic
{
    public abstract class Scope
    {
        public abstract int Size { get; }

        public abstract int IndexMin { get; }

        public abstract int IndexMax { get; }

        public abstract int Next(int baseOffset, int offsetFactor, int indexOffset, int x);

        public abstract int Next(int x);

        public abstract int Previous(int baseOffset, int offsetFactor, int indexOffset, int x);

        public abstract int Previous(int x);

        public abstract bool Contains(int baseOffset, int offsetFactor, int indexOffset, int x);

        public abstract bool Contains(int x);

        public abstract void Add(int baseOffset, int offsetFactor, int indexOffset, int x);

        public abstract void Add(int x);

        public abstract void FirstAdd(int baseOffset, int offsetFactor, int indexOffset, int x);

        public abstract void FirstAdd(int x);

        public abstract bool Remove(int baseOffset, int offsetFactor, int indexOffset, int x);

        public abstract bool Remove(int x);
    }

}