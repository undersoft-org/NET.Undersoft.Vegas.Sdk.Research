using System.Uniques;
using System.Linq;
using System.Reflection;
using Xunit;

namespace System.Instants
{   
    public class InstantFiguresTest
    {
        private InstantFigure str;
        private InstantFigures rtsq;
        private IFigures iRtseq;
        private IFigure iRts; 
        private IFigure InstantFigure_Compilation_Helper_Test(InstantFigure str, FieldsAndPropertiesModel fom)
        {
            IFigure rts = str.New();          

            for (int i = 1; i < str.Rubrics.Count; i++)
            {
                var r = str.Rubrics[i].RubricInfo;
                if (r.MemberType == MemberTypes.Field)
                {
                    var fi = fom.GetType().GetField(((FieldInfo)r).Name);
                    if (fi != null)
                        rts[r.Name] = fi.GetValue(fom);
                }
                if (r.MemberType == MemberTypes.Property)
                {
                    var pi = fom.GetType().GetProperty(((PropertyInfo)r).Name);
                    if (pi != null)
                        rts[r.Name] = pi.GetValue(fom);
                }
            }

            return rts;
           
        }

        [Fact]
        public void InstantFigures_NewObject_Test()
        {
            str = new InstantFigure(InstantFigureMocks.InstantFigure_MemberRubric_FieldsAndPropertiesModel(),
                                                                 "InstantFigure_MemberRubric_FieldsAndPropertiesModel");
            FieldsAndPropertiesModel fom = new FieldsAndPropertiesModel();
            iRts = InstantFigure_Compilation_Helper_Test(str, fom);

            rtsq = new InstantFigures(str, "InstantSequence_Compilation_Test");

            iRtseq = rtsq.New();

            object rcst = iRtseq.NewObject();

            Assert.NotNull(rcst);
           
        }

        [Fact]
        public void InstantFigures_NewFigure_Test()
        {
            str = new InstantFigure(InstantFigureMocks.InstantFigure_MemberRubric_FieldsAndPropertiesModel(),
                                                                 "InstantFigure_MemberRubric_FieldsAndPropertiesModel");
            FieldsAndPropertiesModel fom = new FieldsAndPropertiesModel();
            iRts = InstantFigure_Compilation_Helper_Test(str, fom);

            rtsq = new InstantFigures(str, "InstantSequence_Compilation_Test");

            iRtseq = rtsq.New();

            IFigure rcst = iRtseq.NewFigure();

            Assert.NotNull(rcst);
        }

        [Fact]
        public void InstantFigures_MutatorAndAccessorById_Test()
        {
            str = new InstantFigure(InstantFigureMocks.InstantFigure_MemberRubric_FieldsAndPropertiesModel(),
                                                                 "InstantFigure_MemberRubric_FieldsAndPropertiesModel");
            FieldsAndPropertiesModel fom = new FieldsAndPropertiesModel();
            iRts = InstantFigure_Compilation_Helper_Test(str, fom);

            rtsq = new InstantFigures(str, "InstantSequence_Compilation_Test");

            iRtseq = rtsq.New();

            iRtseq.Add(iRtseq.NewFigure());
            iRtseq[0, 4] = iRts[4];

            Assert.Equal(iRts[4], iRtseq[0, 4]);
            
        }

        [Fact]
        public void InstantFigures_MutatorAndAccessorByName_Test()
        {
            str = new InstantFigure(InstantFigureMocks.InstantFigure_MemberRubric_FieldsAndPropertiesModel(),
                                                                 "InstantFigure_MemberRubric_FieldsAndPropertiesModel");
            FieldsAndPropertiesModel fom = new FieldsAndPropertiesModel();
            iRts = InstantFigure_Compilation_Helper_Test(str, fom);

            rtsq = new InstantFigures(str, "InstantSequence_Compilation_Test");

            iRtseq = rtsq.New();

            iRtseq.Add(iRtseq.NewFigure());
            iRtseq[0, nameof(fom.Name)] = iRts[nameof(fom.Name)];

            Assert.Equal(iRts[nameof(fom.Name)], iRtseq[0, nameof(fom.Name)]);

        }

        [Fact]
        public void InstantFigures_SetRubrics_Test()
        {
            str = new InstantFigure(InstantFigureMocks.InstantFigure_MemberRubric_FieldsAndPropertiesModel(),
                                                                 "InstantFigure_MemberRubric_FieldsAndPropertiesModel");
            FieldsAndPropertiesModel fom = new FieldsAndPropertiesModel();
            iRts = InstantFigure_Compilation_Helper_Test(str, fom);

            rtsq = new InstantFigures(str, "InstantSequence_Compilation_Test");

            var rttab = rtsq.New();

            Assert.Equal(rttab.Rubrics, rtsq.Figure.Rubrics);
          
        }

        [Fact]
        public void InstantFigures_NewFigures_Test()
        {
            str = new InstantFigure(InstantFigureMocks.InstantFigure_MemberRubric_FieldsAndPropertiesModel(),
                                                                 "InstantFigure_MemberRubric_FieldsAndPropertiesModel");
            FieldsAndPropertiesModel fom = new FieldsAndPropertiesModel();
            iRts = InstantFigure_Compilation_Helper_Test(str, fom);

            rtsq = new InstantFigures(str, "InstantSequence_Compilation_Test");

            var rttab = rtsq.New();

            for (int i = 0; i < 10000; i++)
            {      
                rttab.Add((long)int.MaxValue + i, rttab.NewFigure());
            }

            for (int i = 9999; i > -1; i--)
            {                
                rttab[i] = rttab.Get(i + (long)int.MaxValue); 
            }

        }

    }
}
