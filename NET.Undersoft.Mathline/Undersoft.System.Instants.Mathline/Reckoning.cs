using System.Linq;

namespace System.Instants.Mathline
{
    public class Reckoning
    {
        private MathRubrics reckoning;

        public Reckoning(IMultemic data)
        {
            reckoning = new MathRubrics(data);
        }

        public Mathline this[string name]
        {
            get
            {
               return GetMathline(name);
            }
        }
        public Mathline this[MemberRubric rubric]
        {
            get
            {
                return GetMathline(rubric);
            }
        }

        public Mathline GetMathline(string name)
        {
            MemberRubric rubric = null;
            if (reckoning.Rubrics.TryGet(name, out rubric))
            {
                MathRubric mathrubric = null;
                if (reckoning.MathlineRubrics.TryGet(name, out mathrubric))
                    return mathrubric.GetMathline();
                return reckoning.Put(rubric.Name, new MathRubric(reckoning, rubric)).Value.GetMathline();
            }
            return null;
        }
        public Mathline GetMathline(MemberRubric rubric)
        {
            return GetMathline(rubric.Name);
        }

        public IMultemic Reckon()
        {
            reckoning.Combine();
            reckoning.AsValues().Where(p => !p.PartialMathline).OrderBy(p => p.ReckonOrdinal).Select(p => p.Reckon()).ToArray();
            return reckoning.Data;
        }
    }
}
