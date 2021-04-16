using System.Reflection;
using System.Threading.Tasks;

namespace System.Instants
{ 
    public interface IDeputy : IUnique
    {
        MethodInfo Info { get; set; }
        ParameterInfo[] Parameters { get; set; }
        object[] ParameterValues { get; set; }

        object Execute(params object[] parameters);
    }
}
