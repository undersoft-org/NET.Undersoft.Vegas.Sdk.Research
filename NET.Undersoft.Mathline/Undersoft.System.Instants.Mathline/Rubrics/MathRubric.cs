using System;
using System.Extract;
using System.Linq;
using System.Uniques;
using System.Collections;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Reflection.Emit;

namespace System.Instants.Mathline
{
    public class MathRubric : IUnique
    {
        #region Private NonSerialized
        [NonSerialized] private MemberRubric memberRubric;
        [NonSerialized] private CombinedReckoner reckoner;
        [NonSerialized] private CombinedFormula formula;
        [NonSerialized] private Mathline formuler;
        [NonSerialized] private MathRubrics formulaRubrics;
        [NonSerialized] private MathRubrics mathlineRubrics;
        [NonSerialized] private SubMathline subFormuler;

        #endregion

        public MathRubric(MathRubrics rubrics, MemberRubric rubric)
        {
            memberRubric = rubric;
            mathlineRubrics = rubrics;
            SystemSerialCode = rubric.SystemSerialCode;
        }

        #region Mathline Formula

        public int FigureFieldId { get => memberRubric.FigureFieldId; }
        public int ReckonOrdinal { get; set; }
        public Type RubricType { get => memberRubric.RubricType; }
        public string RubricName { get => memberRubric.RubricName; }

        public Mathline CloneMathline()
        {
            return formuler.Clone();
        }
        public Mathline NewMathline()
        {
            return Formuler = new Mathline(this);
        }
        public Mathline GetMathline()
        {
            if (!ReferenceEquals(Formuler, null))
                return Formuler;
            else
            {
                formulaRubrics = new MathRubrics(mathlineRubrics);
                return Formuler = new Mathline(this);
            }
        }

        public Mathline Formuler
        { get { return formuler; } set { formuler = value; } }

        public MathRubrics MathlineRubrics
        { get { return mathlineRubrics; } set { mathlineRubrics = value; } }
        public MathRubrics FormulaRubrics
        { get { return formulaRubrics; } set { formulaRubrics = value; } }
        public MathRubric  FormulaRubric
        { get { return this; } }

        public SubMathline  SubFormuler
        { get; set; }
        public Formula      RightFormula
        { get; set; }

        public bool PartialMathline { get; set; }

        public CombinedReckoner Reckoner
        { get { return reckoner; } set { reckoner = value; } }

        public LeftFormula Reckon()
        {
            if (reckoner != null)
            {
                Reckoner reckon = new Reckoner(reckoner.Reckon);
                reckon();
            }
            return formula.lexpr;
        }

        public Formula Formula
        {
            get
            {
                return (!ReferenceEquals(formula, null)) ? formula : null;
            }
            set
            {
                if (!ReferenceEquals(value, null))
                {
                    formula = value.Prepare(Formuler[this.memberRubric.RubricName]);
                }
            }
        }        

        public CombinedReckoner CombineReckoner()
        {
            if (reckoner == null)
                reckoner = formula.CombineReckoner(formula);

            return reckoner;
        }

        private MathRubric assignRubric(MathRubric erubric)
        {
            if (!FormulaRubrics.Contains(erubric))
            {
                if (!MathlineRubrics.MathlineRubrics.Contains(erubric))
                {
                    MathlineRubrics.MathlineRubrics.Add(erubric);
                }

                if (erubric.FigureFieldId == FormulaRubric.FigureFieldId && 
                    !MathlineRubrics.FormulaRubrics.Contains(erubric))
                    MathlineRubrics.FormulaRubrics.Add(erubric);

                FormulaRubrics.Add(erubric);
            }
            return erubric;
        }
        public MathRubric AssignRubric(int ordinal)
        {
            if (FormulaRubrics == null)
                FormulaRubrics = new MathRubrics(mathlineRubrics);

            MathRubric erubric = null;
            MemberRubric rubric = MathlineRubrics.Rubrics[ordinal];
            if (rubric != null)
            {
                erubric = new MathRubric(MathlineRubrics, rubric);
                assignRubric(erubric);
            }
            return erubric;
        }
        public MathRubric AssignRubric(string name)
        {
            if (FormulaRubrics == null)
                FormulaRubrics = new MathRubrics(mathlineRubrics);

            MathRubric erubric = null;
            MemberRubric rubric = MathlineRubrics.Rubrics[name];
            if (rubric != null)
            {
                erubric = new MathRubric(MathlineRubrics, rubric);
                assignRubric(erubric);
            }
            return erubric;
        }

        private MathRubric removeRubric(MathRubric erubric)
        {
            if (!FormulaRubrics.Contains(erubric))
            {
                FormulaRubrics.Remove(erubric);

                if (!MathlineRubrics.MathlineRubrics.Contains(erubric))
                    MathlineRubrics.MathlineRubrics.Remove(erubric);

                if (!ReferenceEquals(Formuler, null) && 
                    !MathlineRubrics.FormulaRubrics.Contains(erubric))
                {
                    MathlineRubrics.Rubrics.Remove(erubric);
                    Formuler = null;
                    Formula = null;
                }
            }
            return erubric;
        }
        public MathRubric RemoveRubric(string name)
        {
            MathRubric erubric = null;
            MemberRubric rubric = MathlineRubrics.Rubrics[name];
            if (rubric != null)
            {
                erubric = MathlineRubrics[rubric];
                removeRubric(erubric);
            }
            return erubric;
        }
        public MathRubric RemoveRubric(int ordinal)
        {
            MathRubric erubric = null;
            MemberRubric rubric = MathlineRubrics.Rubrics[ordinal];
            if (rubric != null)
            {
                erubric = MathlineRubrics[rubric];
                removeRubric(erubric);
            }
            return erubric;
        }
       
        #endregion

        public IUnique Empty => Ussn.Empty;

        public long KeyBlock { get => SystemSerialCode.KeyBlock; set => SystemSerialCode.KeyBlock = value; }

        public byte[] GetBytes()
        {
            return SystemSerialCode.GetBytes();
        }

        public byte[] GetKeyBytes()
        {
            return SystemSerialCode.GetKeyBytes();
        }

        public void SetHashKey(long value)
        {
            SystemSerialCode.KeyBlock = value;
        }

        public long GetHashKey()
        {
            return SystemSerialCode.KeyBlock;
        }

        public bool Equals(IUnique other)
        {
            return KeyBlock == other.KeyBlock;
        }

        public int CompareTo(IUnique other)
        {
            return (int)(KeyBlock - other.KeyBlock);
        }

        public Ussn SystemSerialCode;

    }

}
