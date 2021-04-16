using System.Uniques;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Extract;
using Xunit;

namespace System.Instants
{   
    public class InstantFigureTest
    {
        public InstantFigureTest() { }

        private IFigure InstantFigure_Compilation_Helper_Test(InstantFigure str, FieldsOnlyModel fom)
        {
            IFigure rts = str.New();
            fom.Id = 202;
            rts[0] = 202;
            Assert.Equal(fom.Id, rts[0]);
            rts["Id"] = 404;
            Assert.NotEqual(fom.Id, rts[nameof(fom.Id)]);
            rts[nameof(fom.Name)] = fom.Name;
            Assert.Equal(fom.Name, rts[nameof(fom.Name)]);
            rts.SystemSerialCode = new Ussn(DateTime.Now.ToBinary());
            string hexTetra = rts.SystemSerialCode.ToString();
            Ussn ssn = new Ussn(hexTetra);
            Assert.Equal(ssn, rts.SystemSerialCode);

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

            for (int i = 1; i < str.Rubrics.Count; i++)
            {
                var r = str.Rubrics[i].RubricInfo;
                if (r.MemberType == MemberTypes.Field)
                {
                    var fi = fom.GetType().GetField(((FieldInfo)r).Name);
                    if (fi != null)
                        Assert.Equal(rts[r.Name], fi.GetValue(fom));
                }
                if (r.MemberType == MemberTypes.Property)
                {
                    var pi = fom.GetType().GetProperty(((PropertyInfo)r).Name);
                    if (pi != null)
                        Assert.Equal(rts[r.Name], pi.GetValue(fom));
                }
            }
            return rts;
        }
        private IFigure InstantFigure_Compilation_Helper_Test(InstantFigure str, PropertiesOnlyModel fom)
        {
            IFigure rts = str.New();
            fom.Id = 202;
            rts[0] = 202;
            Assert.Equal(fom.Id, rts[0]);
            rts["Id"] = 404;
            Assert.NotEqual(fom.Id, rts[nameof(fom.Id)]);
            rts[nameof(fom.Name)] = fom.Name;
            Assert.Equal(fom.Name, rts[nameof(fom.Name)]);
            rts.SystemSerialCode = new Ussn(DateTime.Now.ToBinary());
            string hexTetra = rts.SystemSerialCode.ToString();
            Ussn ssn = new Ussn(hexTetra);
            Assert.Equal(ssn, rts.SystemSerialCode);

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

            for (int i = 1; i < str.Rubrics.Count; i++)
            {
                var r = str.Rubrics[i].RubricInfo;
                if (r.MemberType == MemberTypes.Field)
                {
                    var fi = fom.GetType().GetField(((FieldInfo)r).Name);
                    if (fi != null)
                        Assert.Equal(rts[r.Name], fi.GetValue(fom));
                }
                if (r.MemberType == MemberTypes.Property)
                {
                    var pi = fom.GetType().GetProperty(((PropertyInfo)r).Name);
                    if (pi != null)
                        Assert.Equal(rts[r.Name], pi.GetValue(fom));
                }
            }
            return rts;
        }
        private object  InstantFigure_Compilation_Helper_Test(InstantFigure str, FieldsAndPropertiesModel fom)
        {
            object rts = str.NewObject();
            fom.Id = 202;
            ((IFigure)rts)[0] = 202;
            Assert.Equal(fom.Id, ((IFigure)rts)[0]);
            ((IFigure)rts)["Id"] = 404;
            Assert.NotEqual(fom.Id, ((IFigure)rts)[nameof(fom.Id)]);
            ((IFigure)rts)[nameof(fom.Name)] = fom.Name;
            Assert.Equal(fom.Name, ((IFigure)rts)[nameof(fom.Name)]);
            ((IFigure)rts).SystemSerialCode = new Ussn(DateTime.Now.ToBinary());
            string hexTetra = ((IFigure)rts).SystemSerialCode.ToString();
            Ussn ssn = new Ussn(hexTetra);
            Assert.Equal(ssn, ((IFigure)rts).SystemSerialCode);

            for (int i = 1; i < str.Rubrics.Count; i++)
            {
                var r = str.Rubrics[i].RubricInfo;
                if (r.MemberType == MemberTypes.Field)
                {
                    var fi = fom.GetType().GetField(((FieldInfo)r).Name);
                    if (fi != null)
                        ((IFigure)rts)[r.Name] = fi.GetValue(fom);
                }
                if (r.MemberType == MemberTypes.Property)
                {
                    var pi = fom.GetType().GetProperty(((PropertyInfo)r).Name);
                    if (pi != null)
                        ((IFigure)rts)[r.Name] = pi.GetValue(fom);
                }
            }

            for (int i = 1; i < str.Rubrics.Count; i++)
            {
                var r = str.Rubrics[i].RubricInfo;
                if (r.MemberType == MemberTypes.Field)
                {
                    var fi = fom.GetType().GetField(((FieldInfo)r).Name);
                    if (fi != null)
                        Assert.Equal(((IFigure)rts)[r.Name], fi.GetValue(fom));
                }
                if (r.MemberType == MemberTypes.Property)
                {
                    var pi = fom.GetType().GetProperty(((PropertyInfo)r).Name);
                    if (pi != null)
                        Assert.Equal(((IFigure)rts)[r.Name], pi.GetValue(fom));
                }
            }
            return rts;
        }
        private void    InstantFigure_Compilation_Helper_Test(InstantFigure str, IFigure figure)
        {
            IFigure rts = str.New();
            object[] values = rts.ValueArray;
            rts.ValueArray = figure.ValueArray;
            for (int i = 0; i < values.Length; i++)
                Assert.Equal(figure[i], rts.ValueArray[i]);
            byte[] serie = rts.GetBytes();
        }

        [Fact] public void InstantFigure_Memberinfo_FieldsOnlyModel_Compilation_Test()
        {
            InstantFigure referenceType = new InstantFigure(InstantFigureMocks.InstantFigure_Memberinfo_FieldsOnlyModel(),
                                                                 "InstantFigure_Memberinfo_FieldsOnlyModel_Reference");
            FieldsOnlyModel fom = new FieldsOnlyModel();
            InstantFigure_Compilation_Helper_Test(referenceType, fom);

            InstantFigure valueType = new InstantFigure(InstantFigureMocks.InstantFigure_Memberinfo_FieldsOnlyModel(),
                                                               "InstantFigure_Memberinfo_FieldsOnlyModel_ValueType", FigureMode.ValueType);
            fom = new FieldsOnlyModel();
            InstantFigure_Compilation_Helper_Test(valueType, fom);
        }
        [Fact] public void InstantFigure_MemberRubric_FieldsOnlyModel_Compilation_Test()
        {
            InstantFigure referenceType = new InstantFigure(InstantFigureMocks.InstantFigure_MemberRubric_FieldsOnlyModel(),
                                                                "InstantFigure_MemberRubric_FieldsOnlyModel_Reference");
            FieldsOnlyModel fom = new FieldsOnlyModel();
            InstantFigure_Compilation_Helper_Test(referenceType, fom);

            InstantFigure valueType = new InstantFigure(InstantFigureMocks.InstantFigure_Memberinfo_FieldsOnlyModel(),
                                                             "InstantFigure_MemberRubric_FieldsOnlyModel_ValueType", FigureMode.ValueType);
            fom = new FieldsOnlyModel();
            InstantFigure_Compilation_Helper_Test(valueType, fom);
        }
        [Fact] public void InstantFigure_Memberinfo_PropertiesOnlyModel_Compilation_Test()
        {
            InstantFigure str = new InstantFigure(InstantFigureMocks.InstantFigure_Memberinfo_PropertiesOnlyModel(),
                                                                "InstantFigure_Memberinfo_PropertiesOnlyModel_Reference");
            PropertiesOnlyModel fom = new PropertiesOnlyModel();
            InstantFigure_Compilation_Helper_Test(str, fom);

            InstantFigure valueType = new InstantFigure(InstantFigureMocks.InstantFigure_Memberinfo_PropertiesOnlyModel(),
                                                          "InstantFigure_Memberinfo_PropertiesOnlyModel_ValueType", FigureMode.ValueType);
            fom = new PropertiesOnlyModel();
            InstantFigure_Compilation_Helper_Test(valueType, fom);

        }
        [Fact] public void InstantFigure_MemberRubric_PropertiesOnlyModel_Compilation_Test()
        {
            InstantFigure referenceType = new InstantFigure(InstantFigureMocks.InstantFigure_MemberRubric_PropertiesOnlyModel(),
                                                                "InstantFigure_MemberRubric_PropertiesOnlyModel_Reference");
            PropertiesOnlyModel fom = new PropertiesOnlyModel();
            InstantFigure_Compilation_Helper_Test(referenceType, fom);

            InstantFigure valueType = new InstantFigure(InstantFigureMocks.InstantFigure_MemberRubric_PropertiesOnlyModel(),
                                                        "InstantFigure_MemberRubric_PropertiesOnlyModel_ValueType", FigureMode.ValueType);
            fom = new PropertiesOnlyModel();
            InstantFigure_Compilation_Helper_Test(valueType, fom);

        }
        [Fact] public void InstantFigure_Memberinfo_FieldsAndPropertiesModel_Compilation_Test()
        {
            InstantFigure referenceType = new InstantFigure(InstantFigureMocks.InstantFigure_Memberinfo_FieldsAndPropertiesModel(),
                                                                "InstantFigure_Memberinfo_FieldsAndPropertiesModel_Reference");
            FieldsAndPropertiesModel fom = new FieldsAndPropertiesModel();
            InstantFigure_Compilation_Helper_Test(referenceType, fom);

            InstantFigure valueType = new InstantFigure(InstantFigureMocks.InstantFigure_MemberRubric_FieldsAndPropertiesModel(),
                                                  "InstantFigure_Memberinfo_FieldsAndPropertiesModel_ValueType", FigureMode.ValueType);
            fom = new FieldsAndPropertiesModel();
            InstantFigure_Compilation_Helper_Test(valueType, fom);

        }
        [Fact] public void InstantFigure_MemberRubric_FieldsAndPropertiesModel_Compilation_Test()
        {
            InstantFigure referenceType = new InstantFigure(InstantFigureMocks.InstantFigure_MemberRubric_FieldsAndPropertiesModel(),
                                                                "InstantFigure_MemberRubric_FieldsAndPropertiesModel_Reference");
            FieldsAndPropertiesModel fom = new FieldsAndPropertiesModel();
            InstantFigure_Compilation_Helper_Test(referenceType, fom);

            InstantFigure valueType = new InstantFigure(InstantFigureMocks.InstantFigure_MemberRubric_FieldsAndPropertiesModel(),
                                               "InstantFigure_MemberRubric_FieldsAndPropertiesModel_ValueType", FigureMode.ValueType);
            fom = new FieldsAndPropertiesModel();
            InstantFigure_Compilation_Helper_Test(valueType, fom);

        }
        [Fact] public void InstantFigure_ValueArray_GetSet_Test()
        {
            InstantFigure referenceType = new InstantFigure(InstantFigureMocks.InstantFigure_MemberRubric_FieldsAndPropertiesModel(),
                                               "InstantFigure_ValueArray_FieldsAndPropertiesModel_Reference");
            FieldsAndPropertiesModel fom = new FieldsAndPropertiesModel();           ;
            InstantFigure_Compilation_Helper_Test(referenceType, (IFigure)InstantFigure_Compilation_Helper_Test(referenceType, fom));

            InstantFigure valueType = new InstantFigure(InstantFigureMocks.InstantFigure_MemberRubric_FieldsAndPropertiesModel(),
                                               "InstantFigure_ValueArray_FieldsAndPropertiesModel_ValueType", FigureMode.ValueType);
            fom = new FieldsAndPropertiesModel();           
            InstantFigure_Compilation_Helper_Test(valueType, (IFigure)InstantFigure_Compilation_Helper_Test(valueType, fom));

        }
        [Fact] public unsafe void InstantFigure_ExtractOperations_Test()
        {

            InstantFigure referenceType = new InstantFigure(InstantFigureMocks.InstantFigure_MemberRubric_FieldsAndPropertiesModel(),
                                                                              "InstantFigure_MemberRubric_FieldsAndPropertiesModel_Reference");
            FieldsAndPropertiesModel fom = new FieldsAndPropertiesModel();
            object rts = InstantFigure_Compilation_Helper_Test(referenceType, fom);

            IntPtr pserial = rts.GetStructureIntPtr();
            object rts2 = referenceType.NewObject();
            pserial.ToStructure(rts2);

            byte[] bserial = rts2.GetBytes();
            object rts3 = referenceType.NewObject();
            bserial.ToStructure(rts3);

            object rts4 = referenceType.NewObject();
            rts4.StructureFrom(bserial);

            InstantFigure valueType = new InstantFigure(InstantFigureMocks.InstantFigure_MemberRubric_FieldsAndPropertiesModel(),
                                                                          "InstantFigure_MemberRubric_FieldsAndPropertiesModel_ValueType", 
                                                                           FigureMode.ValueType);
            fom = new FieldsAndPropertiesModel();
            object vts = InstantFigure_Compilation_Helper_Test(valueType, fom);
            ValueType v = (ValueType)vts;

            IntPtr pserial2 = vts.GetStructureIntPtr();

            object vts2 = valueType.NewObject();
            ValueType v2 = (ValueType)vts2;
            vts2 = pserial2.ToStructure(vts2);

            byte[] bserial2 = vts.GetBytes();
            object vts3 = valueType.NewObject();
            fixed(byte* b = bserial2)
            vts3 = Extractor.PointerToStructure(b, vts3);

            object vts4 = valueType.NewObject();
            vts4 = vts4.StructureFrom(pserial2);

            Marshal.FreeHGlobal((IntPtr)pserial2);

        }       
    }
}
