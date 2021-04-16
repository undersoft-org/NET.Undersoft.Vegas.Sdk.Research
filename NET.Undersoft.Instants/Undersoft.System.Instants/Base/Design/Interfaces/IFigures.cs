using System.Multemic;
using System.Instants.Treatment;

namespace System.Instants
{
    public interface IFigures : IDeck<IFigure>, IFigure, IFigureFormatter
    {
        new IFigure this[int index] { get; set; }

        object this[int index, string propertyName] { get; set; }

        object this[int index, int fieldId] { get; set; }        

        FigureCard[] Cards { get; } 

        MemberRubrics Rubrics { get; set; }

        IFigure NewFigure();

        int Length { get; }

        Type FigureType { get; set; }

        int FigureSize { get; set; }

        Type FiguresType { get; set; }

        IFigures Exposition { get; set; }

        FigureFilter Filter { get; set; }

        FigureSort Sort { get; set; }

        Func<IFigure, bool> Query { get; set; }


    }
}