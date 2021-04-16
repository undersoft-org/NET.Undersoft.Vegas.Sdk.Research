using System.Instants.Treatment;
using System.Multemic;

namespace System.Instants
{
    public interface ISelection : IFigures
    {
 //       IFigure this[int index] { get; set; }
 //       object this[int index, string propertyName] { get; set; }
 //       object this[int index, int fieldId] { get; set; }

        IFigures Selection { get; set; }

        IFigures Collection { get; set; }      
    }
}