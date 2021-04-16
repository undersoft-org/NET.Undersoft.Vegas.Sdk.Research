
namespace System.Instants
{
    public class InstantSelection
    {
        private Type compiledType;
        public IFigures Collection;
        public FigureSelection Selection;
        public Type SelectionType;

        public string TypeName;
        public FigureSelection New()
        {
            FigureSelection o = (FigureSelection)SelectionType.New();
            o.Collection = Collection;
            o.Selection = (IFigures)Collection.FiguresType.New();
            return o;
        }       

        public InstantSelection(IFigures collection) : this(collection, null) { }
        public InstantSelection(IFigures collection, string typeName)
        {
            TypeName = typeName;
            Collection = collection;
            InstantSelectionCompiler rsb = new InstantSelectionCompiler(this);
            compiledType = rsb.CompileFigureType(TypeName);

            Selection = (FigureSelection)compiledType.New();
            Selection.Collection = collection;
            Selection.Selection = (IFigures)Collection.FiguresType.New();
            SelectionType = Selection.GetType();
        }      

        //private object[] activateSelection()
        //{
        //    if (Collection.FigureType.BaseType == typeof(ValueType))
        //        return ((IList)Summon.New(Collection.FigureType.MakeArrayType(), Length)).Cast<object>().ToArray();
        //    else
        //        return ((object[])Summon.New(Collection.FigureType.MakeArrayType(), Length));
        //}
    }
}