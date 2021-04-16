using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Collections.ObjectModel;

namespace System.Instants.Treatment
{
    [Serializable]
    public class FilterTerms : Collection<FilterTerm>, ICollection
    {     
        [NonSerialized] private IFigures trell;

        public FilterTerms()
        {        
        }
        public FilterTerms(IFigures figures)
        {
            Collection = figures;
        }

        public IFigures Collection
        { get { return trell; } set { trell = value; } }
      
        public List<FilterTerm> Get(List<string> ColumnNames)
        {
            return this.AsEnumerable().Where(c => ColumnNames.Contains(c.FilterRubric.RubricName)).ToList();
        }
        public List<FilterTerm> Get(int stage)
        {
            FilterStage filterStage = (FilterStage)Enum.ToObject(typeof(FilterStage), stage);
            return this.AsEnumerable().Where(c => filterStage.Equals(c.Stage)).ToList();
        }
        public bool Have(string RubricName)
        {
            return this.AsEnumerable().Where(c => c.RubricName == RubricName).Any();
        }
        public FilterTerm[] GetTerms(string RubricName)
        {
            return this.AsEnumerable().Where(c => c.RubricName == RubricName).ToArray();
        }
        public FilterTerms Clone()
        {
            FilterTerms ft = new FilterTerms();
            foreach(FilterTerm t in this)
            {
                FilterTerm _t = new FilterTerm(t.RubricName, t.Operand, t.Value, t.Logic, t.Stage);
                ft.Add(_t);
            }            
            return ft;
        }      

        #region ICollection
        public new int Add(FilterTerm value)
        {
            value.Collection = Collection;
            value.Index = ((IList)this).Add(value);
            return value.Index;
        }        
        public void AddRange(ICollection<FilterTerm> terms)
        {
            foreach (FilterTerm term in terms)
            {
                term.Collection = Collection;
                term.Index = Add(term);
            }
        }
        public void AddNewRange(ICollection<FilterTerm> terms)
        {
            bool diffs = false;
            if (Count != terms.Count)
            {
                diffs = true;
            }
            else
            {             
                foreach (FilterTerm term in terms)
                {
                    if(Have(term.RubricName))
                    {
                        int same = 0;
                        foreach (FilterTerm myterm in GetTerms(term.RubricName))
                        {
                            if (!myterm.Compare(term))
                                same++;
                        }
                        if(same != 0)
                        {
                            diffs = true;
                            break;
                        }
                    }
                    else
                    {
                        diffs = true;
                        break;
                    }
                }
            }

            if(diffs)
            {
                Clear();
                foreach (FilterTerm term in terms)
                    Add(term);
            }
        }
        public object AddNew()
        {
            return (object)((IBindingList)this).AddNew();
        }          
        public void RemoveRange(ICollection<FilterTerm> value)
        {
            foreach (FilterTerm term in value)            
                Remove(term);
        }
        public void SetRange(FilterTerm[] data)
        {
            for (int i = 0; i < data.Length; i++)
                this[i] = data[i];
        }     
        public int IndexOf(object value)
        {
            for (int i = 0; i < Count; i++)
                if (ReferenceEquals(this[i], value))    // Found it
                    return i;
            return -1;
        }
        public FilterTerm Find(FilterTerm data)
        {
            foreach (FilterTerm lDetailValue in this)
                if (lDetailValue == data)    // Found it
                    return lDetailValue;
            return null;    // Not found
        }      
        public void Reset()
        {
            this.Clear();
        }
        #endregion     
    }

    [Serializable]
    public class FilterTerm : ICloneable
    {
        public string valueTypeName;
        [NonSerialized] private Type valueType;
        [NonSerialized] private IFigures collection;
        public IFigures Collection
        {
            get
            {
                return collection;
            }
            set
            {
                collection = value;
                if (FilterRubric == null && value != null)
                {
                    MemberRubric[] filterRubrics = collection.Rubrics.AsValues()
                             .Where(c => c.RubricName == RubricName).ToArray();
                    if (filterRubrics.Length > 0)
                    {
                        FilterRubric = filterRubrics[0];
                        ValueType = FilterRubric.RubricType;
                    }
                }
            }
        }
        public MemberRubric FilterRubric
        { get; set; }
           

        public FilterTerm()
        {
            Stage = FilterStage.First;
        }
        public FilterTerm(IFigures figures)
        {
            Stage = FilterStage.First ;
            collection = figures;
        }
        public FilterTerm(string filterColumn, string operand, object value, string logic = "And", int stage = 1)
        {
            RubricName = filterColumn;
            OperandType tempOperand1;
            Enum.TryParse(operand, true, out tempOperand1);
            Operand = tempOperand1;
            Value = value;
            LogicType tempLogic;
            Enum.TryParse(logic, true, out tempLogic);
            Logic = tempLogic;          
            Stage = (FilterStage)Enum.ToObject(typeof(FilterStage), stage);

        }
        public FilterTerm(string filterColumn, OperandType operand, object value, LogicType logic = LogicType.And, FilterStage stage = FilterStage.First)
        {
            RubricName = filterColumn;
            Operand = operand;
            Value = value;
            Logic = logic;
            Stage = stage;

        }
        public FilterTerm(IFigures figures, string filterColumn, string operand, object value, string logic = "And", int stage = 1)
        {
            RubricName = filterColumn;
            OperandType tempOperand1;
            Enum.TryParse(operand, true, out tempOperand1);
            Operand = tempOperand1;
            Value = value;
            LogicType tempLogic;
            Enum.TryParse(logic, true, out tempLogic);
            Logic = tempLogic;
            collection = figures;
            if (figures != null)
            {
                MemberRubric[] filterRubrics = collection.Rubrics.AsValues().Where(c => c.RubricName == RubricName).ToArray();
                if (filterRubrics.Length > 0)
                {
                    FilterRubric = filterRubrics[0]; ValueType = FilterRubric.RubricType;
                }
            }
            Stage = (FilterStage)Enum.ToObject(typeof(FilterStage), stage);
        }
        public FilterTerm(MemberRubric filterColumn, OperandType operand, object value, LogicType logic = LogicType.And, FilterStage stage = FilterStage.First)
        {
            Operand = operand;
            Value = value;
            Logic = logic;
            ValueType = filterColumn.RubricType;
            RubricName = filterColumn.RubricName;
            FilterRubric = filterColumn;           
            Stage = stage;
        }

        [DisplayName("Pos")]
        public int Index { get; set; }
        public string RubricName { get; set; }
        public Type ValueType
        {
            get
            {
                if (valueType == null && valueTypeName != null)
                    valueType = Type.GetType(valueTypeName);
                return valueType;
            }
            set
            {
                valueType = value;
                valueTypeName = value.FullName;
            }
        }    
        public OperandType Operand { get; set; }
        public object Value { get; set; }
        public LogicType Logic { get; set; }
        public FilterStage Stage { get; set; } = FilterStage.First;

        public string OperandString(OperandType _operand)
        {
            string operandString = "";
            switch (_operand)
            {
                case OperandType.Equal:
                    operandString = "=";
                    break;
                case OperandType.EqualOrMore:
                    operandString = ">=";
                    break;
                case OperandType.More:
                    operandString = ">";
                    break;
                case OperandType.EqualOrLess:
                    operandString = "<=";
                    break;
                case OperandType.Less:
                    operandString = "<";
                    break;
                case OperandType.Like:
                    operandString = "like";
                    break;
                case OperandType.NotLike:
                    operandString = "!like";
                    break;
                default:
                    operandString = "=";
                    break;
            }
            return operandString;
        }
        public OperandType OperandEnum(string operandString)
        {
            OperandType _operand = OperandType.None;
            switch (operandString)
            {
                case "=":
                    _operand = OperandType.Equal;
                    break;
                case ">=":
                    _operand = OperandType.EqualOrMore;
                    break;
                case ">":
                    _operand = OperandType.More;
                    break;
                case "<=":
                    _operand = OperandType.EqualOrLess;
                    break;
                case "<":
                    _operand = OperandType.Less;
                    break;
                case "like":
                    _operand = OperandType.Like;
                    break;
                case "!like":
                    _operand = OperandType.NotLike;
                    break;
                default:
                    _operand = OperandType.None;
                    break;
            }
            return _operand;
        }

        public bool Compare(FilterTerm term)
        {
            if (RubricName != term.RubricName)
                return false;
            if (!Value.Equals(term.Value))
                return false;
            if (!Operand.Equals(term.Operand))
                return false;
            if (!Stage.Equals(term.Stage))
                return false;
            if (!Logic.Equals(term.Logic))
                return false;

            return true;
        }

        public object Clone()
        {
            FilterTerm clone = (FilterTerm)this.MemberwiseClone();
            clone.FilterRubric = FilterRubric;
            return clone;
        }
        public FilterTerm Clone(object value)
        {
            FilterTerm clone = (FilterTerm)this.MemberwiseClone();
            clone.FilterRubric = FilterRubric;
            clone.Value = value;
            return clone;
        }

    }

    [Serializable]
    public enum OperandType
    {
        Equal,
        EqualOrMore,
        EqualOrLess,
        More,
        Less,
        Like,
        NotLike,
        Contains,
        None
    }

    [Serializable]
    public enum LogicType
    {
        And,
        Or
    }

    [Serializable]
    public enum FilterStage
    {
        None,
        First,
        Second,
        Third
    }

    


}
