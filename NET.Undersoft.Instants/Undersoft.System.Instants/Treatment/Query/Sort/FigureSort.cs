using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Data;

namespace System.Instants.Treatment
{
    [Serializable]
    public class FigureSort
    {
        public IFigures Collection;

        public SortTerms Terms;

        public FigureSort(IFigures figures)
        {
            Collection = figures;
            Terms = new SortTerms(figures);
        }

    }
}
