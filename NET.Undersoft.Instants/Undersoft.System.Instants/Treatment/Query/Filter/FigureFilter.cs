using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace System.Instants.Treatment
{
    [Serializable]
    public class FigureFilter
    {
        [NonSerialized] private FilterTerms termsBuffer;
        [NonSerialized] private FilterTerms termsReducer;
        [NonSerialized] private FilterExpression expression;
        [NonSerialized] private IFigures collection;
        [NonSerialized] public Func<IFigure, bool> Evaluator;

        public IFigures Collection
        { get { return collection; } set { collection = value; } }
        public FilterTerms Reducer
        { get; set; }
        public FilterTerms Terms
        { get; set; }

        public FigureFilter(IFigures collection)
        {
            Collection = collection;
            expression = new FilterExpression();
            Reducer = new FilterTerms(collection);
            Terms = new FilterTerms(collection);
            termsBuffer = expression.Conditions;
            termsReducer = new FilterTerms(collection);
        }
        
        public Expression<Func<IFigure, bool>> GetExpression(int stage = 1)
        {
            termsReducer.Clear();
            termsReducer.AddRange(Reducer.AsEnumerable().Concat(Terms.AsEnumerable()).ToArray());
            expression.Conditions = termsReducer;
            termsBuffer = termsReducer;
            return expression.CreateExpression(stage);
        }

        public IFigure[] Filter(int stage = 1)
        {
            termsReducer.Clear();
            termsReducer.AddRange(Reducer.AsEnumerable().Concat(Terms.AsEnumerable()).ToArray());
            expression.Conditions = termsReducer;
            termsBuffer = termsReducer;
            return Collection.AsEnumerable().AsQueryable().Where(expression.CreateExpression(stage).Compile()).ToArray();
        }
        public IFigure[] Filter(ICollection<IFigure> toFilter, int stage = 1)
        {
            termsReducer.Clear();
            termsReducer.AddRange(Reducer.AsEnumerable().Concat(Terms.AsEnumerable()).ToArray());
            expression.Conditions = termsReducer;
            termsBuffer = termsReducer;
            return toFilter.AsQueryable().Where(expression.CreateExpression(stage).Compile()).ToArray();
        }       
    }
}
