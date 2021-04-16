using System.Multemic;

namespace System.Instants
{
    public class InstantFigures
     {
        private Type compiledType;  
        public InstantFigure Figure; 
        public IFigures Collection;
        public Type FiguresType;
        public bool SafeThread;

        public string TypeName;

        public object NewObj()
        {
            object o = FiguresType.New();
            ((IFigures)o).Rubrics = CloneRubrics();
            ((IFigures)o).FigureType = Figure.FigureType;
            ((IFigures)o).FigureSize = Figure.FigureSize;
            ((IFigures)o).FiguresType = FiguresType;
            return o;
        }
        public IFigures New()
        {
            IFigures tab = (IFigures)FiguresType.New(); 
            tab.Rubrics = CloneRubrics();
            tab.FigureType = Figure.FigureType;
            tab.FigureSize = Figure.FigureSize;
            tab.FiguresType = FiguresType;
            return tab;
        }

        public InstantFigures(InstantFigure figure, bool safeThread = true) : this(figure, null, safeThread)
        {          
        }
        public InstantFigures(InstantFigure figure, string typeName, bool safeThread = true)
        {
            SafeThread = safeThread;
            TypeName = typeName;
            Figure = figure;
            InstantFiguresCompiler rsb = new InstantFiguresCompiler(this);
            compiledType = rsb.CompileFigureType(TypeName);

            FiguresType = compiledType.New().GetType();
            Collection = New();
            Collection.FiguresType = FiguresType;
        }

        private MemberRubrics CloneRubrics()
        {
            var rbrcs = new MemberRubrics();
            rbrcs.KeyRubrics = new MemberRubrics();
            foreach (var rbrc in Figure.Rubrics.AsValues())
                rbrcs.Add(new MemberRubric(rbrc));
            return rbrcs;
        }
    }
}