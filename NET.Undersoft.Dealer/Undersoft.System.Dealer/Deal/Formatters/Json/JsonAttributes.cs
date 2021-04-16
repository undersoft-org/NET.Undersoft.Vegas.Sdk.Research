using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Dealer
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Delegate | AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
    public sealed class JsonIgnoreAttribute : Attribute
    {
        public JsonIgnoreAttribute()
        { }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Delegate | AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
    public sealed class JsonMemberAttribute : Attribute
    {
        public JsonModes SerialMode { get; set; } = JsonModes.All;

        public JsonMemberAttribute()
        { }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Delegate | AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
    public sealed class JsonArrayAttribute : Attribute
    {
        public JsonArrayAttribute()
        { }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Delegate | AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
    public sealed class JsonObjectAttribute : Attribute
    {
        public JsonObjectAttribute()
        { }
    }

    public enum JsonModes
    {
        All,
        KeyValue,
        Array        
    }

}
