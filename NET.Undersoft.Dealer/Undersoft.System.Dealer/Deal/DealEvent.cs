using System.Instants;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Dealer
{
    [Serializable]
    public class DealEvent : InstantDeputy
    {

        public DealEvent(string MethodName, string TargetClassName, params object[] parameters) : base(TargetClassName, MethodName)
        {
            base.ParameterValues = parameters;
        }
        public DealEvent(string MethodName, object TargetClassObject, params object[] parameters) : base(TargetClassObject, MethodName)
        {
            base.ParameterValues = parameters;
        }
    }
}


