using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Linq;
using System.Uniques;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Globalization;

namespace System.Instants
{   
   
    public interface IMemberRubric
    {     
        string RubricName { get; set; }
        Type RubricType { get; set; }
        int RubricId { get; set; }
        int RubricSize { get; set; }
        int RubricOffset { get; set; }
        bool Visible { get; set; }
        bool Editable { get; set; }
        object[] RubricAttributes { get; set; }
    }
}
