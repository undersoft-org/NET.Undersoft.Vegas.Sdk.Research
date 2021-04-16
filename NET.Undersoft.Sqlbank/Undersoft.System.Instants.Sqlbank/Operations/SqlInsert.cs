using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Data.SqlClient;
using System.Multemic;
using System.Globalization;

namespace System.Instants.Sqlbank
{
    public class SqlInsert
    {
        private SqlConnection _cn;

        public SqlInsert(SqlConnection cn)
        {
            _cn = cn;
        }
        public SqlInsert(string cnstring)
        {
            _cn = new SqlConnection(cnstring);
        }

        public Album<Album<IFigure>> Insert(IMultemic table, bool keysFromDeckis = false, bool buildMapping = false, bool updateKeys = false, string[] updateExcept = null, BulkPrepareType tempType = BulkPrepareType.Trunc)
        {
            try
            {
                IMultemic tab = table;
                if (tab.Any())
                {                    
                    IList<FieldMapping> nMaps = new List<FieldMapping>();
                    if (buildMapping)
                    {
                        SqlMapper imapper = new SqlMapper(tab, keysFromDeckis);
                    }
                    nMaps = tab.Rubrics.Mappings;
                    string dbName = _cn.Database;
                    SqlAdapter afad = new SqlAdapter(_cn);
                    afad.DataBulk(tab, tab.FigureType.Name, tempType, BulkDbType.TempDB);
                    _cn.ChangeDatabase(dbName);
                    var nSet = new Album<Album<IFigure>>();

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(@"  /* ----  SQL BANK START CMD ------ */  ");
                    foreach (FieldMapping nMap in nMaps)
                    {
                        sb.AppendLine(@"  /* ----  TABLE BULK START CMD ------ */  ");

                        MemberRubric[] ic = tab.Rubrics.AsValues().Where(c => nMap.ColumnOrdinal.Contains(c.FigureFieldId)).ToArray();
                        MemberRubric[] ik = tab.Rubrics.AsValues().Where(c => nMap.KeyOrdinal.Contains(c.FigureFieldId)).ToArray();

                        if (updateExcept != null)
                        {
                            ic = ic.Where(c => !updateExcept.Contains(c.RubricName)).ToArray();
                            ik = ik.Where(c => !updateExcept.Contains(c.RubricName)).ToArray();
                        }

                        string qry = BulkInsertQuery(dbName, tab.FigureType.Name, nMap.DbTableName, ic, ik, updateKeys).ToString();
                        sb.Append(qry);
                        sb.AppendLine(@"  /* ----  TABLE BULK END CMD ------ */  ");
                    }
                    sb.AppendLine(@"  /* ----  SQL BANK END CMD ------ */  ");

                    Album<Album<IFigure>> bIMultemic = afad.ExecuteInsert(sb.ToString(), tab, true);


                    if (nSet.Count == 0)
                        nSet = bIMultemic;
                    else
                        foreach (Album<IFigure> its in bIMultemic.AsValues())
                        {
                            if (nSet.Contains(its))
                            {
                                nSet[its].Put(its.AsValues());
                            }
                            else
                                nSet.Add(its);
                        }
                    sb.Clear();

                    return nSet;
                }
                else
                    return null;
            }
            catch (SqlException ex)
            {
                _cn.Close();
                throw new SqlInsertException(ex.ToString());
            }
        }

        public Album<Album<IFigure>> BatchInsert(IMultemic table, int batchSize = 1000)
        {
            try
            {
                IMultemic tab = table;
                IList<FieldMapping> nMaps = new List<FieldMapping>();
                SqlAdapter afad = new SqlAdapter(_cn);
                StringBuilder sb = new StringBuilder();
                var nSet = new Album<Album<IFigure>>();
                sb.AppendLine(@"  /* ----  SQL BANK START CMD ------ */  ");
                int count = 0;
                foreach (IFigure ir in tab)
                {
                   
                    foreach (FieldMapping nMap in nMaps)
                    {
                        MemberRubric[] ic = tab.Rubrics.AsValues().Where(c => nMap.ColumnOrdinal.Contains(c.FigureFieldId)).ToArray();
                        MemberRubric[] ik = tab.Rubrics.AsValues().Where(c => nMap.KeyOrdinal.Contains(c.FigureFieldId)).ToArray();                   

                        string qry = BatchInsertQuery(ir, nMap.DbTableName, ic, ik).ToString();
                        sb.Append(qry);
                        count++;
                    }
                    if (count >= batchSize)
                    {
                        sb.AppendLine(@"  /* ----  DATA BATCH END CMD ------ */  ");
                        var bIMultemic = afad.ExecuteInsert(sb.ToString(), tab);
                        if (nSet.Count == 0)
                            nSet = bIMultemic;
                        else
                            foreach (Album<IFigure> its in bIMultemic.AsValues())
                            {
                                if (nSet.Contains(its))
                                {
                                    nSet[its].Put(its.AsValues());
                                }
                                else
                                    nSet.Add(its);
                            }
                        sb.Clear();
                        sb.AppendLine(@"  /* ----  SQL BANK START CMD ------ */  ");
                        count = 0;
                    }
                }
                sb.AppendLine(@"  /* ----  DATA BANK END CMD ------ */  ");

                var rIMultemic = afad.ExecuteInsert(sb.ToString(), tab);

                if (nSet.Count == 0)
                    nSet = rIMultemic;
                else
                    foreach (Album<IFigure> its in rIMultemic.AsValues())
                    {
                        if (nSet.Contains(its))
                        {
                            nSet[its].Put(its.AsValues());
                        }
                        else
                            nSet.Add(its);
                    }

                return nSet;
            }
            catch (SqlException ex)
            {
                _cn.Close();
                throw new SqlInsertException(ex.ToString());
            }
        }
        public Album<Album<IFigure>> BatchInsert(IMultemic table, bool buildMapping, int batchSize = 1000)
        {
            try
            {
                IMultemic tab = table;
                IList<FieldMapping> nMaps = new List<FieldMapping>();
                SqlAdapter afad = new SqlAdapter(_cn);
                StringBuilder sb = new StringBuilder();
                var nSet = new Album<Album<IFigure>>();
                sb.AppendLine(@"  /* ----  SQL BANK START CMD ------ */  ");
                int count = 0;
                foreach (IFigure ir in tab)
                {

                    foreach (FieldMapping nMap in nMaps)
                    {
                        MemberRubric[] ic = tab.Rubrics.AsValues().Where(c => nMap.ColumnOrdinal.Contains(c.FigureFieldId)).ToArray();
                        MemberRubric[] ik = tab.Rubrics.AsValues().Where(c => nMap.KeyOrdinal.Contains(c.FigureFieldId)).ToArray();

                        string qry = BatchInsertQuery(ir, nMap.DbTableName, ic, ik).ToString();
                        sb.Append(qry);
                        count++;
                    }
                    if (count >= batchSize)
                    {
                        sb.AppendLine(@"  /* ----  DATA BATCH END CMD ------ */  ");
                        var bIMultemic = afad.ExecuteInsert(sb.ToString(), tab);
                        if (nSet.Count == 0)
                            nSet = bIMultemic;
                        else
                            foreach (Album<IFigure> its in bIMultemic.AsValues())
                            {
                                if (nSet.Contains(its))
                                {
                                    nSet[its].Put(its.AsValues());
                                }
                                else
                                    nSet.Add(its);
                            }
                        sb.Clear();
                        sb.AppendLine(@"  /* ----  SQL BANK START CMD ------ */  ");
                        count = 0;
                    }
                }
                sb.AppendLine(@"  /* ----  DATA BANK END CMD ------ */  ");

                var rIMultemic = afad.ExecuteInsert(sb.ToString(), tab);

                if (nSet.Count == 0)
                    nSet = rIMultemic;
                else
                    foreach (Album<IFigure> its in rIMultemic.AsValues())
                    {
                        if (nSet.Contains(its))
                        {
                            nSet[its].Put(its.AsValues());
                        }
                        else
                            nSet.Add(its);
                    }

                return nSet;
            }
            catch (SqlException ex)
            {
                _cn.Close();
                throw new SqlInsertException(ex.ToString());
            }
        }

        public int SimpleInsert(IMultemic table, int batchSize = 1000)
        {
            try
            {
                IMultemic tab = table;
                IList<FieldMapping> nMaps = new List<FieldMapping>();
                SqlAdapter afad = new SqlAdapter(_cn);
                StringBuilder sb = new StringBuilder();
                int intSqlbank = 0;
                sb.AppendLine(@"  /* ----  SQL BANK START CMD ------ */  ");
                int count = 0;
                foreach (IFigure ir in tab)
                {
                 
                    foreach (FieldMapping nMap in nMaps)
                    {
                        MemberRubric[] ic = tab.Rubrics.AsValues().Where(c => nMap.ColumnOrdinal.Contains(c.FigureFieldId)).ToArray();
                        MemberRubric[] ik = tab.Rubrics.AsValues().Where(c => nMap.KeyOrdinal.Contains(c.FigureFieldId)).ToArray();                    

                        string qry = BatchInsertQuery(ir, nMap.DbTableName, ic, ik).ToString();
                        sb.Append(qry);
                        count++;
                    }
                    if (count >= batchSize)
                    {
                        sb.AppendLine(@"  /* ----  DATA BATCH END CMD ------ */  ");
                        intSqlbank += afad.ExecuteInsert(sb.ToString());
                      
                        sb.Clear();
                        sb.AppendLine(@"  /* ----  SQL BANK START CMD ------ */  ");
                        count = 0;
                    }
                }
                sb.AppendLine(@"  /* ----  DATA BANK END CMD ------ */  ");

                intSqlbank += afad.ExecuteInsert(sb.ToString());
                             
                return intSqlbank;
            }
            catch (SqlException ex)
            {
                _cn.Close();
                throw new SqlInsertException(ex.ToString());
            }
        }
        public int SimpleInsert(IMultemic table, bool buildMapping, int batchSize = 1000)
        {
            try
            {
               IMultemic tab = table;
                IList<FieldMapping> nMaps = new List<FieldMapping>();
                SqlAdapter afad = new SqlAdapter(_cn);
                StringBuilder sb = new StringBuilder();
                int intSqlbank = 0;
                sb.AppendLine(@"  /* ----  SQL BANK START CMD ------ */  ");
                int count = 0;
                foreach (IFigure ir in tab)
                {                   

                    foreach (FieldMapping nMap in nMaps)
                    {;
                        MemberRubric[] ic = tab.Rubrics.AsValues().Where(c => nMap.ColumnOrdinal.Contains(c.FigureFieldId)).ToArray();
                        MemberRubric[] ik = tab.Rubrics.AsValues().Where(c => nMap.KeyOrdinal.Contains(c.FigureFieldId)).ToArray();

                        string qry = BatchInsertQuery(ir, nMap.DbTableName, ic, ik).ToString();
                        sb.Append(qry);
                        count++;
                    }
                    if (count >= batchSize)
                    {
                        sb.AppendLine(@"  /* ----  DATA BATCH END CMD ------ */  ");
                      
                        intSqlbank += afad.ExecuteInsert(sb.ToString());

                        sb.Clear();
                        sb.AppendLine(@"  /* ----  SQL BANK START CMD ------ */  ");
                        count = 0;
                    }
                }
                sb.AppendLine(@"  /* ----  DATA BANK END CMD ------ */  ");

                intSqlbank += afad.ExecuteInsert(sb.ToString());
                return intSqlbank;
            }
            catch (SqlException ex)
            {
                _cn.Close();
                throw new SqlInsertException(ex.ToString());
            }
        }

        public StringBuilder BatchInsertQuery(IFigure card, string tableName, MemberRubric[] columns, MemberRubric[] keys, bool updateKeys = true)
        {
            StringBuilder sbCols = new StringBuilder(), sbVals = new StringBuilder(), sbQry = new StringBuilder();
            string tName = tableName;
            IFigure ir = card;
            object[] ia = ir.ValueArray;
            MemberRubric[] ic = columns;
            MemberRubric[] ik = keys;

            sbCols.AppendLine(@"  /* ---- DATA BANK START DataTEM CMD ------ */  ");
            sbCols.Append("INSERT INTO " + tableName + " (");
            sbVals.Append(@") OUTPUT inserted.* VALUES (");
            bool isUpdateCol = false;
            string delim = "";
            int c = 0;
            for (int i = 0; i < columns.Length; i++)
            {


                if (columns[i].RubricName.ToLower() == "updated")
                    isUpdateCol = true;
                if (ia[columns[i].FigureFieldId] != DBNull.Value && !columns[i].IsIdentity)
                {
                    if (c > 0)
                        delim = ",";
                    sbCols.AppendFormat(CultureInfo.InvariantCulture, @"{0}[{1}]", delim,
                                                                                   columns[i].RubricName                                                                                         
                                                                                   );
                    sbVals.AppendFormat(CultureInfo.InvariantCulture, @"{0} {1}{2}{1}", delim,
                                                                                        (columns[i].RubricType == typeof(string) ||
                                                                                        columns[i].RubricType == typeof(DateTime)) ? "'" : "",
                                                                                        (columns[i].RubricType != typeof(string)) ?
                                                                                        Convert.ChangeType(ia[columns[i].FigureFieldId], columns[i].RubricType) :
                                                                                        ia[columns[i].FigureFieldId].ToString().Replace("'", "''")
                                                                                        );
                    c++;
                }
            }

            if (DbHand.Schema.DataDbTables[tableName].DataDbColumns.Have("updated") && !isUpdateCol)
            {
                sbCols.AppendFormat(CultureInfo.InvariantCulture, ", [updated]", DateTime.Now);
                sbVals.AppendFormat(CultureInfo.InvariantCulture, ", '{0}'", DateTime.Now);
            }          
                if (columns.Length > 0)
                    delim = ",";
                else
                    delim = "";
                c = 0;
                for (int i = 0; i < keys.Length; i++)
                {

                    if (ia[keys[i].FigureFieldId] != DBNull.Value && !keys[i].IsIdentity)
                    {
                        if (c > 0)
                            delim = ",";
                    sbCols.AppendFormat(CultureInfo.InvariantCulture, @"{0}[{1}]",  delim,
                                                                                    keys[i].RubricName
                                                                                    );
                    sbVals.AppendFormat(CultureInfo.InvariantCulture, @"{0} {1}{2}{1}", delim,
                                                                                        (keys[i].RubricType == typeof(string) ||
                                                                                        keys[i].RubricType == typeof(DateTime)) ? "'" : "",
                                                                                        (keys[i].RubricType != typeof(string)) ?
                                                                                        Convert.ChangeType(ia[keys[i].FigureFieldId], keys[i].RubricType) :
                                                                                        ia[keys[i].FigureFieldId].ToString().Replace("'", "''")
                                                                                        );
                    c++;
                    }
                }
            sbQry.Append(sbCols.ToString() + sbVals.ToString() + ") ");
            sbQry.AppendLine(@"  /* ----  SQL BANK END ITEM CMD ------ */  ");
            return sbQry;
        }
        public StringBuilder BulkInsertQuery(string DBName, string buforName, string tableName, MemberRubric[] columns, MemberRubric[] keys, bool updateKeys = true)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sbv = new StringBuilder();
            string bName = buforName;
            string tName = tableName;
            MemberRubric[] rubrics = keys.Concat(columns).ToArray();           
            string dbName = DBName;
            sb.AppendLine(@"  /* ---- DATA BANK START ITEM CMD ------ */");
            sb.AppendFormat(@"INSERT INTO [{0}].[dbo].[" + tName + "] (", dbName);
            sbv.Append(@"SELECT ");
            bool isUpdateCol = false;
            string delim = "";
            int c = 0;
            for (int i = 0; i < rubrics.Length; i++)
            {

                if (rubrics[i].RubricName.ToLower() == "updated")
                    isUpdateCol = true;

                if (c > 0)
                    delim = ",";
                sb.AppendFormat(CultureInfo.InvariantCulture, @"{0}[{1}]", delim, rubrics[i].RubricName);
                sbv.AppendFormat(CultureInfo.InvariantCulture, @"{0}[S].[{1}]", delim, rubrics[i].RubricName);
                c++;
            }
            sb.AppendFormat(CultureInfo.InvariantCulture, @") OUTPUT inserted.* {0}", sbv.ToString());
            sb.AppendFormat(" FROM [tempdb].[dbo].[{0}] AS S ", bName, dbName, tName);
            sb.AppendLine("");
            sb.AppendLine(@"  /* ----  SQL BANK END ITEM CMD ------ */  ");
            sbv.Clear();
            return sb;
        }
    }

    public class SqlInsertException : Exception
    {
        public SqlInsertException(string message)
            : base(message)
        {

        }
    }
}
