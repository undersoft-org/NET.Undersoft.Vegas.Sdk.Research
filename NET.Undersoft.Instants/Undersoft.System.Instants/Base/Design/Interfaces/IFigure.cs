using System.Uniques;

namespace System.Instants
{
    public interface IFigure : IUnique
    {       
        object this[string propertyName] { get; set; }

        object this[int fieldId] {
            get;
            set; }

        object[] ValueArray { get; set; }

        Ussn SystemSerialCode { get; set; }
    }   
}