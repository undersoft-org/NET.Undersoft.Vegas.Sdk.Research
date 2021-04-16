using System;
using System.Collections.Generic;
using System.Linq;
using System.Multemic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;

namespace System.Instants.Sqlbank
{
    public class SqlUpdate
    {
        private SqlConnection _cn;

        public SqlUpdate(SqlConnection cn)
        {
            _cn = cn;
        } 
        public SqlUpdate(string cnstring)
        {
            _cn = new SqlConnection(cnstring);
        }

        public Album<Album<IFigure>> Update(IMultemic table, bool keysFromDeck = false, bool buildMapping = false, bool updateKeys = false, string[] updateExcept = null, BulkPrepareType tempType = BulkPrepareType.Trunc)
        {
            try
            {
                IMultemic tab = table;
                if (tab.Count > 0)
                {                    
                    IList<FieldMapping> nMaps = new List<FieldMapping>();
                    if (buildMapping)
                    {
                        SqlMapper imapper = new SqlMapper(tab, keysFromDeck);
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

                        string qry = BulkUpdateQuery(dbName, tab.FigureType.Name, nMap.DbTableName, ic, ik, updateKeys).ToString();
                        sb.Append(qry);
                        sb.AppendLine(@"  /* ----  TABLE BULK END CMD ------ */  ");
                    }
                    sb.AppendLine(@"  /* ----  SQL BANK END CMD ------ */  ");

                    var bIMultemic = afad.ExecuteUpdate(sb.ToString(), tab, true);

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
                throw new SqlUpdateException(ex.ToString());
            }
        }
        public Album<Album<IFigure>> BatchUpdate(IMultemic table, bool keysFromDeck = false, bool buildMapping = false, bool updateKeys = false, string[] updateExcept = null, int batchSize = 250)
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
                foreach (IFigure ir in table)
                {
                    if (ir.GetType().DeclaringType != tab.FigureType)
                    {                       
                        if (buildMapping)
                        {
                            SqlMapper imapper = new SqlMapper(tab, keysFromDeck);
                        }
                        nMaps = tab.Rubrics.Mappings;
                    }

                    foreach (FieldMapping nMap in nMaps)
                    {
                        IDeck<int> co = nMap.ColumnOrdinal;
                        IDeck<int> ko = nMap.KeyOrdinal;
                        MemberRubric[] ic = tab.Rubrics.AsValues().Where(c => nMap.ColumnOrdinal.Contains(c.FigureFieldId)).ToArray();
                        MemberRubric[] ik = tab.Rubrics.AsValues().Where(c => nMap.KeyOrdinal.Contains(c.FigureFieldId)).ToArray();
                        if (updateExcept != null)
                        {
                            ic = ic.Where(c => !updateExcept.Contains(c.RubricName)).ToArray();
                            ik = ik.Where(c => !updateExcept.Contains(c.RubricName)).ToArray();
                        }

                        string qry = BatchUpdateQuery(ir, nMap.DbTableName, ic, ik, updateKeys).ToString();
                        sb.Append(qry);
                        count++;
                    }
                    if (count >= batchSize)
                    {
                        sb.AppendLine(@"  /* ----  DATA BATCH END CMD ------ */  ");
                        Album<Album<IFigure>> bIMultemic = afad.ExecuteUpdate(sb.ToString(), tab);
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

                Album<Album<IFigure>> rIMultemic = afad.ExecuteUpdate(sb.ToString(), tab, true);

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
                throw new SqlUpdateException(ex.ToString());
            }
        }
        public int  SimpleUpdate(IMultemic table, bool buildMapping = false, bool updateKeys = false, string[] updateExcept = null, int batchSize = 500)
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
                foreach (IFigure ir in table)
                {
                      if (ir.GetType().DeclaringType != tab.FigureType)
                    {                       
                        if (buildMapping)
                        {
                            SqlMapper imapper = new SqlMapper(tab);
                        }
                        nMaps = tab.Rubrics.Mappings;
                    }

                    foreach (FieldMapping nMap in nMaps)
                    {
                        IDeck<int> co = nMap.ColumnOrdinal;
                        IDeck<int> ko = nMap.KeyOrdinal;
                        MemberRubric[] ic = tab.Rubrics.AsValues().Where(c => nMap.ColumnOrdinal.Contains(c.FigureFieldId)).ToArray();
                        MemberRubric[] ik = tab.Rubrics.AsValues().Where(c => nMap.KeyOrdinal.Contains(c.FigureFieldId)).ToArray();
                        if (updateExcept != null)
                        {
                            ic = ic.Where(c => !updateExcept.Contains(c.RubricName)).ToArray();
                            ik = ik.Where(c => !updateExcept.Contains(c.RubricName)).ToArray();
                        }

                        string qry = BatchUpdateQuery(ir, nMap.DbTableName, ic, ik, updateKeys).ToString();
                        sb.Append(qry);
                        count++;
                    }
                    if (count >= batchSize)
                    {
                        sb.AppendLine(@"  /* ----  DATA BATCH EDataD CMD ------ */  ");
                        intSqlbank += afad.ExecuteUpdate(sb.ToString());
                        sb.Clear();
                        sb.AppendLine(@"  /* ----  SQL BANK START CMD ------ */  ");
                        count = 0;
                    }
                }
                sb.AppendLine(@"  /* ----  DATA BANK END CMD ------ */  ");

                intSqlbank += afad.ExecuteUpdate(sb.ToString());
                return intSqlbank;
            }
            catch (SqlException ex)
            {
                _cn.Close();
                throw new SqlUpdateException(ex.ToString());
            }
        }
   
        public StringBuilder BatchUpdateQuery(IFigure card, string tableName, MemberRubric[] columns, MemberRubric[] keys, bool updateKeys = true)
        {
            StringBuilder sb = new StringBuilder();
            string tName = tableName;
            IFigure ir = card;
            object[] ia = ir.ValueArray;
            MemberRubric[] ic = columns;
            MemberRubric[] ik = keys;

            sb.AppendLine(@"  /* ---- DATA BANK START ITEM CMD ------ */  ");
            sb.Append("UPDATE " + tName + " SET ");
            bool isUpdateCol = false;
            string delim = "";
            int c = 0;
            for (int i = 0; i < columns.Length; i++)
            {
                if (columns[i].RubricName.ToLower() == "updated")
                    isUpdateCol = true;
                if (ia[columns[i].FigureFieldId] != DBNull.Value)
                {
                    if (c > 0)
                        delim = ",";
                    sb.AppendFormat(CultureInfo.InvariantCulture, 
                                    @"{0}[{1}] = {2}{3}{2}", 
                                    delim,
                                    columns[i].RubricName,
                                    (columns[i].RubricType == typeof(string) ||
                                    columns[i].RubricType == typeof(DateTime)) ? "'" : "",
                                    (columns[i].RubricType != typeof(string)) ?
                                    Convert.ChangeType(ia[columns[i].FigureFieldId], columns[i].RubricType) :
                                    ia[columns[i].FigureFieldId].ToString().Replace("'","''")
                                    );
                    c++;
                }
            }
          
            if (DbHand.Schema.DataDbTables[tableName].DataDbColumns.Have("updated") && !isUpdateCol)
                sb.AppendFormat(CultureInfo.InvariantCulture, ", [updated] = '{0}'", DateTime.Now);

            if (updateKeys)
            {
                if (columns.Length > 0)
                    delim = ",";
                else
                    delim = "";
                c = 0;
                for (int i = 0; i < keys.Length; i++)
                {
                    
                    if (ia[keys[i].FigureFieldId] != DBNull.Value)
                    {
                        if (c > 0)
                            delim = ",";
                        sb.AppendFormat(CultureInfo.InvariantCulture, 
                                        @"{0}[{1}] = {2}{3}{2}",  
                                        delim,
                                        keys[i].RubricName,
                                        (keys[i].RubricType == typeof(string) ||
                                        keys[i].RubricType == typeof(DateTime)) ? "'" : "",
                                        (keys[i].RubricType != typeof(string)) ?
                                        Convert.ChangeType(ia[keys[i].FigureFieldId], keys[i].RubricType) :
                                        ia[keys[i].FigureFieldId].ToString().Replace("'", "''")
                                        );
                        c++;
                    }
                }
            }
            sb.AppendLine(" OUTPUT inserted.*, deleted.* ");
            delim = "";
            c = 0;
            for (int i = 0; i < keys.Length; i++)
            {
                if (i > 0)
                    delim = " AND ";
                else
                    delim = " WHERE ";
                if (ia[keys[i].FigureFieldId] != DBNull.Value)
                {
                    if (c > 0)
                        delim = " AND ";
                    else
                        delim = " WHERE ";
                    sb.AppendFormat(CultureInfo.InvariantCulture, 
                                    @"{0} [{1}] = {2}{3}{2}", 
                                    delim,
                                    keys[i].RubricName,
                                    (keys[i].RubricType == typeof(string) ||
                                    keys[i].RubricType == typeof(DateTime)) ? "'" : "", 
                                    (ia[keys[i].FigureFieldId] != DBNull.Value) ?
                                    (keys[i].RubricType != typeof(string)) ?
                                    Convert.ChangeType(ia[keys[i].FigureFieldId], keys[i].RubricType) :
                                    ia[keys[i].FigureFieldId].ToString().Replace("'", "''") : ""
                                    );
                    c++;
                }
            }
            sb.AppendLine("");
            sb.AppendLine(@"  /* ----  SQL BANK END ITEM CMD ------ */  ");
            return sb;
        }
        public StringBuilder BulkUpdateQuery(string DBName, string buforName, string tableName, MemberRubric[] columns, MemberRubric[] keys, bool updateKeys = true)
        {
            StringBuilder sb = new StringBuilder();
            string bName = buforName;
            string tName = tableName;
            MemberRubric[] ic = columns;
            MemberRubric[] ik = keys;
            string dbName = DBName; 
            sb.AppendLine(@"  /* ---- DATA BANK START ITEM CMD ------ */  ");
            sb.AppendFormat(@"UPDATE [{0}].[dbo].[" + tName + "] SET ", dbName);
            bool isUpdateCol = false;
            string delim = "";
            int c = 0;
            for (int i = 0; i < columns.Length; i++)
            {

                if (columns[i].RubricName.ToLower() == "updated")
                    isUpdateCol = true;

                if (c > 0)
                    delim = ",";
                sb.AppendFormat(CultureInfo.InvariantCulture, 
                                @"{0}[{1}] =[S].[{2}]", 
                                delim,
                                columns[i].RubricName,
                                columns[i].RubricName
                                );
                c++;
            }       

            if (updateKeys)
            {
                if (columns.Length > 0)
                    delim = ",";
                else
                    delim = "";
                c = 0;
                for (int i = 0; i < keys.Length; i++)
                {
                        if (c > 0)
                            delim = ",";
                        sb.AppendFormat(CultureInfo.InvariantCulture, 
                                        @"{0}[{1}] = [S].[{2}]", 
                                        delim,
                                        keys[i].RubricName,
                                        keys[i].RubricName
                                        );
                        c++;
                }
            }
            sb.AppendLine(" OUTPUT inserted.*, deleted.* ");
            sb.AppendFormat(" FROM [tempdb].[dbo].[{0}] AS S INNER JOIN [{1}].[dbo].[{2}] AS T ", bName, dbName, tName );
            delim = "";
            c = 0;
            for (int i = 0; i < keys.Length; i++)
            {
                if (i > 0)
                    delim = " AND ";
                else
                    delim = " ON ";

                sb.AppendFormat(CultureInfo.InvariantCulture, 
                                @"{0} [T].[{1}] = [S].[{2}]", 
                                delim,
                                keys[i].RubricName,
                                keys[i].RubricName
                                );
                c++;
            }
            sb.AppendLine("");
            sb.AppendLine(@"  /* ----  SQL BANK END ITEM CMD ------ */  ");
            return sb;
        }

        public class SqlUpdateException : Exception
        {
            public SqlUpdateException(string message)
                : base(message)
            {

            }
        }

    }
}
