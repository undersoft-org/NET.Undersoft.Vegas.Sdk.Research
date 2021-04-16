using System.Uniques;
using System.Linq;
using System.Reflection;
using Xunit;

namespace System.Instants.Mathline
{   
    public class MathlineTest
    {
        private InstantFigure instFig;
        private InstantMultemic instMtic;
        private Reckoning rck;
        private IMultemic spcMtic;

        private IFigure Mathline_Figure_Helper_Test(IMultemic str, MathlineMockModel fom)
        {
            IFigure rts = str.NewFigure();          

            for (int i = 1; i < str.Rubrics.Count; i++)
            {
                var r = str.Rubrics[i].RubricInfo;
                if (r.MemberType == MemberTypes.Field)
                {
                    var fi = fom.GetType().GetField(((FieldInfo)((MemberRubric)r).RubricInfo).Name);
                    if (fi != null)
                        rts[r.Name] = fi.GetValue(fom);
                }
                if (r.MemberType == MemberTypes.Property)
                {
                    var pi = fom.GetType().GetProperty(((PropertyInfo)((MemberRubric)r).RubricInfo).Name);
                    if (pi != null)
                        rts[r.Name] = pi.GetValue(fom);
                }
            }

            return rts;
           
        }

        public MathlineTest()
        {
            instFig = new InstantFigure(typeof(MathlineMockModel).GetMembers().Select(m => m.MemberType == MemberTypes.Field ? m :
                                                             m.MemberType == MemberTypes.Property ? m :
                                                             null).Where(p => p != null).ToArray(),
                                                             "InstantFigure_MemberRubric_FieldsAndPropertiesModel");
           
            instMtic = new InstantMultemic(instFig, "InstantMultemic_Mathline_Test");

            spcMtic = instMtic.New();

            MathlineMockModel fom = new MathlineMockModel();

            for (int i = 0; i < 250 * 1000; i++)
            {
                var f = Mathline_Figure_Helper_Test(spcMtic, fom);
                f["NetPrice"] = (double)f["NetPrice"] + i;
                f["NetCost"] = (double)f["NetPrice"] / 2;
                spcMtic.Add((long)int.MaxValue + i, f);
            }            

        }

        [Fact]
        public void Mathline_Reckoning_Formula_Test()
        {
            rck = new Reckoning(spcMtic);

            Mathline ml = rck.GetMathline("SellNetPrice");

            ml.Formula = (ml["NetPrice"] * (ml["SellFeeRate"] / 100)) + ml["NetPrice"];

            Mathline ml2 = rck.GetMathline("SellGrossPrice");

            ml2.Formula = ml * ml2["TaxRate"];

            rck.Reckon(); // first with formula compilation

            rck.Reckon(); // second when formula compiled
        }

        [Fact]
        public void Mathline_Reckoning_LogicOnStack_Formula_Test()
        {
            rck = new Reckoning(spcMtic);

            Mathline ml = rck.GetMathline("SellNetPrice");

            ml.Formula = (((ml["NetCost"] < 10) | (ml["NetCost"] > 50)) * (ml["NetPrice"] * (ml["SellFeeRate"] / 100)) + ml["NetPrice"]) +
                          ((ml["NetCost"] > 10) & (ml["NetCost"] < 50)) * ml["NetPrice"];

            Mathline ml2 = rck.GetMathline("SellGrossPrice");

            ml2.Formula = ml * ml2["TaxRate"];

            rck.Reckon(); // first with formula compilation

            rck.Reckon(); // second when formula compiled
        }
    }
}
