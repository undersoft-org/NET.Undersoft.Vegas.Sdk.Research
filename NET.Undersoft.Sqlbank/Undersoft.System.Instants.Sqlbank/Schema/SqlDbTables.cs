using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace System.Instants.Sqlbank
{
    public class DbTables
    {
        private object holder = new object();
        public object Holder
        {
            get
            {
                return holder;
            }
        }

        public DbTables()
        {
            tables = new List<DbTable>();
        }
        private List<DbTable> tables;
        public List<DbTable> List
        {
            get
            {
                return
                    tables;
            }
            set
            {
                tables.AddRange(value.Where(c => !this.Have(c.TableName)).ToList());
            }
        }

        public void Add(DbTable table) { if (!this.Have(table.TableName)) { tables.Add(table); } }
        public void AddRange(List<DbTable> _tables) { tables.AddRange(_tables.Where(c => !this.Have(c.TableName)).ToList()); }
        public void Remove(DbTable table) { tables.Remove(table); }
        public void RemoveAt(int index) { tables.RemoveAt(index); }
        public bool Have(string TableName)
        {
            return tables.Where(t => t.TableName == TableName).Any();
        }
        public void Clear() { tables.Clear(); }

        public DbTable this[string TableName]
        {
            get
            {
                return tables.Where(c => TableName == c.TableName).First();
            }
        }
        public DbTable this[int TableIndex]
        {
            get
            {
                return tables[TableIndex];
            }
        }
        public DbTable GetDbTable(string TableName)
        {
            return tables.Where(c => TableName == c.TableName).First();
        }
        public List<DbTable> GetDbTables(List<string> TableNames)
        {
            return tables.Where(c => TableNames.Contains(c.TableName)).ToList();
        }

    }

    public class DbTable
    {
        public string TableName { get; set; }
        public DbTable()
        {
        }
        public DbTable(string tableName)
        {
            TableName = tableName;

        }

        public DbColumns DataDbColumns { get; set; }

        private DbColumn[] dbPrimaryKey;         
        public DbColumn[] DbPrimaryKey
        {
            get
            {
                return dbPrimaryKey;
            }
            set
            {
                dbPrimaryKey = value;
            }
        }

        public List<MemberRubric> GetColumnsForDataTable
        {
            get
            {
                return DataDbColumns.List.Select(c =>
                    new MemberRubric(new FieldRubric(c.RubricType, c.ColumnName, c.DbColumnSize, c.DbOrdinal) { RubricSize = c.DbColumnSize })
                    {
                        FigureFieldId = c.DbOrdinal - 1,
                        IsAutoincrement = c.isAutoincrement,
                        IsDBNull = c.isDBNull,
                        IsIdentity = c.isIdentity                        
                    }).ToList();
            }
        }
        public MemberRubric[] GetKeyForDataTable
        {
            get
            {
                return DbPrimaryKey.Select(c =>
                                                 new MemberRubric(new FieldRubric(c.RubricType, c.ColumnName, c.DbColumnSize, c.DbOrdinal) { RubricSize = c.DbColumnSize })
                                                 {
                                                     FigureFieldId = c.DbOrdinal - 1,
                                                     IsAutoincrement = c.isAutoincrement,
                                                     IsDBNull = c.isDBNull,
                                                     IsIdentity = c.isIdentity
                                                 }).ToArray();
            }
        }
    }



}
