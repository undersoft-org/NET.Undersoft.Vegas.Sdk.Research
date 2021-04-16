using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.ComponentModel;
using System.Text;

using System.Data;

namespace System.Instants.Mathline
{
    public delegate void Reckoner();

    [Serializable]
    public class CompilerContext
    {
        [NonSerialized] int pass = 0;
        [NonSerialized] int paramCount;
        [NonSerialized] int indexVariableCount;      
        [NonSerialized] int[] indexVariables;
        [NonSerialized] IMultemic[] paramTables = new IMultemic[10];

        public CompilerContext()
        {
            indexVariableCount = 0;         
        }

        public int Add(IMultemic v)
        {
            int index = GetIndexOf(v);
            if (index < 0)
            {
                paramTables[paramCount] = v;
                return indexVariableCount + paramCount++;
            }
            return index;
        }

        public int GetIndexOf(IMultemic v)
        {
            for (int i = 0; i < paramCount; i++)
                if (paramTables[i] == v) return indexVariableCount + i;
            return -1;
        }
        public int GetSubIndexOf(IMultemic v)
        {
            for (int i = 0; i < paramCount; i++)
                if (paramTables[i] == v) return indexVariableCount + i + paramCount;
            return -1;
        }

        public int GetBufforIndexOf(IMultemic v)
        {
            for (int i = 0; i < paramCount; i++)
                if (paramTables[i] == v) return indexVariableCount + i + paramCount + 1;
            return -1;
        }

        public int Count
        {
            get { return paramCount; }
        }

        public IMultemic[] ParamCards
        {
            get { return paramTables; }
        }

        public static void GenLocalLoad(ILGenerator g, int a)
        {
            switch (a)
            {
                case 0: g.Emit(OpCodes.Ldloc_0); break;
                case 1: g.Emit(OpCodes.Ldloc_1); break;
                case 2: g.Emit(OpCodes.Ldloc_2); break;
                case 3: g.Emit(OpCodes.Ldloc_3); break;
                default:
                    g.Emit(OpCodes.Ldloc, a);
                    break;
            }
        }
        public static void GenLocalStore(ILGenerator g, int a)
        {
            switch (a)
            {
                case 0: g.Emit(OpCodes.Stloc_0); break;
                case 1: g.Emit(OpCodes.Stloc_1); break;
                case 2: g.Emit(OpCodes.Stloc_2); break;
                case 3: g.Emit(OpCodes.Stloc_3); break;
                default:
                    g.Emit(OpCodes.Stloc, a);
                    break;
            }
        }

        public bool IsFirstPass()
        {
            return pass == 0;
        }

        public void NextPass()
        {
            pass++;
            // local variables array
            indexVariables = new int[indexVariableCount];
            for (int i = 0; i < indexVariableCount; i++)
                indexVariables[i] = i;
        }

        // index access by variable number		
        public int GetIndexVariable(int number)
        {
            return indexVariables[number];
        }

        public void SetIndexVariable(int number, int value)
        {
            indexVariables[number] = value;
        }

        public int AllocIndexVariable()
        {
            return indexVariableCount++;
        }

        public void GenerateLocalInit(ILGenerator g)
        {
            // declare indexes
            for (int i = 0; i < indexVariableCount; i++)
                g.DeclareLocal(typeof(int));

            // declare parameters
            string paramFieldName = "DataParameters";

            for (int i = 0; i < paramCount; i++)
                g.DeclareLocal(typeof(IMultemic));

            for (int i = 0; i < paramCount; i++)
                g.DeclareLocal(typeof(IFigure));

            g.DeclareLocal(typeof(double));

            // load the parameters from parameters array
            for (int i = 0; i < paramCount; i++)
            {
                // simple this.paramTables[i]
                g.Emit(OpCodes.Ldarg_0); //this
                g.Emit(OpCodes.Ldfld, typeof(CombinedReckoner).GetField(paramFieldName));
                g.Emit(OpCodes.Ldc_I4, i);
                g.Emit(OpCodes.Ldelem_Ref);
                g.Emit(OpCodes.Stloc, indexVariableCount + i);
            }
        }
    }
}
