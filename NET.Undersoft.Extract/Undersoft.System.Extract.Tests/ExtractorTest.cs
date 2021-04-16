using System.Uniques;
using System.Runtime.InteropServices;
using System.Instants;
using System.Linq;
using System.Reflection;
using Xunit;

namespace System.Extract
{   
    public class ExtractorTest
    {
        #region Parameters
        byte[] source = new byte[ushort.MaxValue];
        byte[] dest = new byte[ushort.MaxValue];
        byte[] structBytes = null;
        byte[] structBytes2 = null;
        InstantFigure str = null;
        InstantMultemic table = null;
        IMultemic rctab = null;
        IFigure rcobj = null;
        #endregion

        public ExtractorTest()
        {
            Random r = new Random();
            r.NextBytes(source);

            str = new InstantFigure(InstantFigureMocks.InstantFigure_MemberRubric_FieldsAndPropertiesModel(),
                                                      "InstantFigure_MemberRubric_FieldsAndPropertiesModel_ValueType");
            FieldsAndPropertiesModel fom = new FieldsAndPropertiesModel();
            table = new InstantMultemic(str, "InstantMultemic_Compilation_Test");
            rctab = table.New();

            rcobj = rctab.NewFigure();

            foreach (var rubric in str.Rubrics.AsValues())
            {
                if (rubric.FigureFieldId > -1)
                {                   
                    var field = fom.GetType().GetField(rubric.FigureField.Name,
                        BindingFlags.NonPublic | BindingFlags.Instance);
                    if (field == null)
                        field = fom.GetType().GetField(rubric.RubricName);
                    if (field == null)
                    {
                        var prop = fom.GetType().GetProperty(rubric.RubricName);
                        if(prop != null)
                            rcobj[rubric.FigureFieldId] = prop.GetValue(fom);
                    }
                    else
                        rcobj[rubric.FigureFieldId] = field.GetValue(fom);   
                    
                }
            }

            for (int i = 0; i < 1000; i++)
            {
                IFigure nrcstr = rctab.NewFigure();
                nrcstr.ValueArray = rcobj.ValueArray;
                rctab.Add(i, nrcstr);
            }

            structBytes = new byte[rctab.FigureSize];
            structBytes2 = new byte[rctab.FigureSize];

            rcobj.StructureTo(ref structBytes, 0);
        }

        [Fact] public unsafe void Extractor_CopyBlock_ByteArray_UInt_Test()
        {
            Random r = new Random();
            r.NextBytes(source);
            dest.Initialize();

            Extractor.CopyBlock(dest, 0, source, 0, source.Length);
            bool equal = dest.BlockEqual(source);
            Assert.True(equal);
        }
        [Fact] public unsafe void Extractor_CopyBlock_Pointer_UInt_Test()
        {
            Random r = new Random();
            r.NextBytes(source);
            dest.Initialize();

            fixed (byte* psrc = source, pdst = dest)
            {
                Extractor.CopyBlock(pdst, 0, psrc, 0, source.Length);
                bool equal = dest.BlockEqual(source);
                Assert.True(equal);
            }
        }
        [Fact] public unsafe void Extractor_CopyBlock_ByteArray_Ulong_Test()
        {
            Random r = new Random();
            r.NextBytes(source);
            dest.Initialize();

            Extractor.CopyBlock(dest, (ulong)0, source, (ulong)0, (ulong)source.Length);
            bool equal = dest.BlockEqual(source);
            Assert.True(equal);
        }
        [Fact] public unsafe void Extractor_CopyBlock_Pointer_Ulong_Test()
        {
            Random r = new Random();
            r.NextBytes(source);
            dest.Initialize();

            Extractor.CopyBlock(dest, 0, source, 0, source.Length);
            bool equal = dest.BlockEqual(source);
            Assert.True(equal);
        }

        [Fact] public unsafe void Extractor_BytesToStruct_FromType_Test()
        {
            object os = rctab.NewObject();
            Extractor.BytesToStructure(structBytes, os, 0);
            bool equal = rcobj.StructureEqual(os);
            Assert.True(equal);
        }
        [Fact] public unsafe void Extractor_PointerToNewStruct_Type_Test()
        {
            fixed (byte* b = structBytes)
            {
                object os = Extractor.PointerToStructure(b, rctab.FigureType, 0);
                bool equal = rcobj.StructureEqual(os);
                Assert.True(equal);
            }
        }
        [Fact] public unsafe void Extractor_PointerToStruct_Test()
        {
            fixed (byte* b = structBytes)
            {
                object os = rctab.NewObject();
                Extractor.PointerToStructure(b, os);
                bool equal = rcobj.StructureEqual(os);
                Assert.True(equal);
            }
        }
        [Fact] public unsafe void Extractor_StructToBytesArray_Test()
        {
            byte[] b = rcobj.GetStructureBytes();
            bool equal = b.BlockEqual(structBytes);           
            Assert.True(equal);
            object ro = rcobj;
            structBytes2 = new byte[rcobj.GetSize()];
            Extractor.StructureToBytes(ro, ref structBytes2, 0);
            bool equal2 = structBytes2.BlockEqual(structBytes);
            Assert.True(equal2);

        }
        [Fact] public unsafe void Extractor_StructToPointer_Test()
        {          

            GCHandle gcptr = GCHandle.Alloc(structBytes, GCHandleType.Pinned);
            byte* ptr = (byte*)gcptr.AddrOfPinnedObject();

            rcobj.StructureTo(ptr);

            rcobj["Id"] = 88888;
            rcobj["Name"] = "Zmiany";

            rcobj.StructureTo(ptr);

            rcobj["Id"] = 5555555;
            rcobj["Name"] = "Zm342";

            rcobj.StructureFrom(ptr);

            Assert.Equal(88888, rcobj["Id"]);


        }
        [Fact] public unsafe void Extractor_StructModel_Test()
        {

            StructModel[] structure = new StructModel[] { new StructModel(83948930), new StructModel(45453), new StructModel(5435332) };
            structure[0].Alias = "FirstAlias";
            structure[0].Name = "FirstName";
            structure[1].Alias = "SecondAlia";
            structure[1].Name = "SecondName";
            structure[2].Alias = "ThirdAlias";
            structure[2].Name = "ThirdName";

            StructModels structures = new StructModels(structure);

            int size = Marshal.SizeOf(structure[0]);

            byte* pserial = ExtractOperation.ValueStructureToPointer(structure[0]);

            StructModel structure2 = new StructModel();
            ValueType o = structure2;

            ExtractOperation.PointerToValueStructure(pserial, o, 0);

            structure2 = (StructModel)o;

            structure2.Alias = "FirstChange";

        }

    }
}
