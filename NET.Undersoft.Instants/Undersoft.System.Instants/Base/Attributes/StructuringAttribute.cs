using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace System.Instants
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class StructuringAttribute : Attribute
    {
        public UnmanagedType Value;

        public StructuringAttribute(UnmanagedType unmanaged)
        {            
            Value = unmanaged;       
        }
        

        public int SizeConst;

        public UnmanagedType ArraySubType;

    }
}
