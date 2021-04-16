using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Instants.Sqlbank
{
    public class DbColumns
    {
        public DbColumns()
        {
            list = new List<DbColumn>();
        }
        private List<DbColumn> list;
        public List<DbColumn> List
        {
            get
            {
                return list;
            }
            set
            {
                list.AddRange(value.Where(c => !this.Have(c.ColumnName)).ToList());
            }
        }
        

        public void Add(DbColumn column) { if (!this.Have(column.ColumnName)) List.Add(column); }
        public void AddRange(List<DbColumn> _columns) { list.AddRange(_columns.Where(c => !this.Have(c.ColumnName)).ToList()); }
        public void Remove(DbColumn column) { list.Remove(column); }
        public void RemoveAt(int index) { list.RemoveAt(index); }
        public void Clear() { List.Clear(); }
        public bool Have(string ColumnName)
        {
            return list.Where(c => c.ColumnName == ColumnName).Any();
        }

        public DbColumn this[string ColumnName]
        {
            get
            {
                return list.Where(c => ColumnName == c.ColumnName).First();
            }
        }
        public DbColumn this[int Ordinal]
        {
            get
            {
                return list.Where(c => Ordinal == c.DbOrdinal).First();
            }
        }
        public DbColumn GetDbColumn(string ColumnName)
        {
            return list.Where(c => c.ColumnName == ColumnName).First();
        }
        public DbColumn[] GetDbColumns(List<string> ColumnNames)
        {
            return list.Where(c => ColumnNames.Contains(c.ColumnName)).ToArray();
        }
        public List<MemberRubric> GetRubrics(string ColumnNames)
        {
            return list.Where(c => ColumnNames == c.ColumnName).SelectMany(r => r.Rubrics).ToList();
        }
        public List<MemberRubric> GetRubrics(List<string> ColumnNames)
        {
            return list.Where(c => ColumnNames.Contains(c.ColumnName)).SelectMany(r => r.Rubrics).ToList();
        }
    }

    public class DbColumn
    {
        public DbColumn()
        {
            isDBNull = false;
            isIdentity = false;
            isKey = false;
            isAutoincrement = false;
            MaxLength = -1;         
        }
        public string ColumnName { get; set; }
        public int DbOrdinal { get; set; }
        public Type RubricType { get; set; }
        public int DbColumnSize { get; set; }       
        public bool isDBNull { get; set; }
        public bool isIdentity { get; set; }
        public bool isKey { get; set; }
        public bool isAutoincrement { get; set; }
        public int MaxLength { get; set; }
        public List<MemberRubric> Rubrics { get; set; }
    }

 
}


