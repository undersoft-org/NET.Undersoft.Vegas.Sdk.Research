using System;
using System.Multemic;
using System.Collections.Generic;
using System.Linq;

namespace System.Instants.Sqlbank
{
    
    public class SqlMapper
    {
        public IMultemic CardsMapped { get; set; }       
        public SqlMapper(IMultemic table, bool KeysFromDeckis = false, string[] dbTableNames = null, string tablePrefix = "")
        {
            try
            {
                bool mixedMode = false;
                string tName = "", dbtName = "", prefix = tablePrefix;
                List<string> dbtNameMixList = new List<string>();
                if (dbTableNames != null)
                {
                    foreach (string dbTableName in dbTableNames)
                        if (DbHand.Schema.DataDbTables.Have(dbTableName))
                            dbtNameMixList.Add(dbTableName);
                    if (dbtNameMixList.Count > 0)
                        mixedMode = true;
                }
                IMultemic t = table;
                tName = t.FigureType.Name;
                if (!mixedMode)
                {
                    if (!DbHand.Schema.DataDbTables.Have(tName))
                    {
                        if (DbHand.Schema.DataDbTables.Have(prefix + tName))
                            dbtName = prefix + tName;
                    }
                    else
                        dbtName = tName;
                    if (!string.IsNullOrEmpty(dbtName))
                    {
                        if (!KeysFromDeckis)
                        {
                            Album<int> colOrdinal = new Album<int>(t.Rubrics.AsValues().Where(c => DbHand.Schema.DataDbTables[dbtName].DataDbColumns.Have(c.RubricName) &&
                                                                                                    !DbHand.Schema.DataDbTables[dbtName].DbPrimaryKey.Select(pk => pk.ColumnName)
                                                                                                         .Contains(c.RubricName)).Select(o => o.FigureFieldId));
                            Album<int> keyOrdinal = new Album<int>(t.Rubrics.AsValues().Where(c => DbHand.Schema.DataDbTables[dbtName].DbPrimaryKey.Select(pk => pk.ColumnName)
                                                                                                         .Contains(c.RubricName)).Select(o => o.FigureFieldId));
                            FieldMapping iSqlbankMap = new FieldMapping(dbtName, keyOrdinal, colOrdinal);
                            if (t.Rubrics.Mappings == null)
                                t.Rubrics.Mappings = new FieldMappings();
                            t.Rubrics.Mappings.Add(iSqlbankMap.DbTableName, iSqlbankMap);
                        }
                        else
                        {
                            Album<int> colOrdinal = new Album<int>(t.Rubrics.AsValues().Where(c => DbHand.Schema.DataDbTables[dbtName].DataDbColumns.Have(c.RubricName) &&
                                                                                                          !c.IsKey).Select(o => o.FigureFieldId));
                            Album<int> keyOrdinal = new Album<int>(t.Rubrics.AsValues().Where(c => DbHand.Schema.DataDbTables[dbtName].DataDbColumns.Have(c.RubricName) &&
                                                                                                          c.IsKey).Select(o => o.FigureFieldId));
                            FieldMapping iSqlbankMap = new FieldMapping(dbtName, keyOrdinal, colOrdinal);
                            if (t.Rubrics.Mappings == null)
                                t.Rubrics.Mappings = new FieldMappings();
                            t.Rubrics.Mappings.Add(iSqlbankMap.DbTableName, iSqlbankMap);
                        }
                    }
                }
                else
                {
                    if (!KeysFromDeckis)
                    {
                        foreach (string dbtNameMix in dbtNameMixList)
                        {
                            dbtName = dbtNameMix;
                            Album<int> colOrdinal = new Album<int>(t.Rubrics.AsValues().Where(c => DbHand.Schema.DataDbTables[dbtName].DataDbColumns.Have(c.RubricName) &&
                                                                                                          !DbHand.Schema.DataDbTables[dbtName].DbPrimaryKey.Select(pk => pk.ColumnName)
                                                                                                        .Contains(c.RubricName)).Select(o => o.FigureFieldId));
                            Album<int> keyOrdinal = new Album<int>((t.Rubrics.AsValues().Where(c => DbHand.Schema.DataDbTables[dbtName].DbPrimaryKey.Select(pk => pk.ColumnName)
                                                                                                         .Contains(c.RubricName)).Select(o => o.FigureFieldId)));
                            if (keyOrdinal.Count == 0)
                                keyOrdinal = new Album<int>(t.Rubrics.KeyRubrics.AsValues().Where(c => DbHand.Schema.DataDbTables[dbtName].DataDbColumns.Have(c.RubricName)).Select(o => o.FigureFieldId));
                            FieldMapping iSqlbankMap = new FieldMapping(dbtName, keyOrdinal, colOrdinal);
                            if (t.Rubrics.Mappings == null)
                                t.Rubrics.Mappings = new FieldMappings();
                            t.Rubrics.Mappings.Add(iSqlbankMap.DbTableName, iSqlbankMap);
                        }
                    }
                    else
                    {
                        foreach (string dbtNameMix in dbtNameMixList)
                        {
                            dbtName = dbtNameMix;
                            Album<int> colOrdinal = new Album<int>(t.Rubrics.AsValues().Where(c => DbHand.Schema.DataDbTables[dbtName].DataDbColumns.Have(c.RubricName) &&
                                                                                                          !c.IsKey).Select(o => o.FigureFieldId));
                            Album<int> keyOrdinal = new Album<int>(t.Rubrics.AsValues().Where(c => DbHand.Schema.DataDbTables[dbtName].DataDbColumns.Have(c.RubricName) &&
                                                                                                          c.IsKey).Select(o => o.FigureFieldId));
                            if (keyOrdinal.Count == 0)
                                keyOrdinal = new Album<int>(t.Rubrics.KeyRubrics.AsValues().Where(c => DbHand.Schema.DataDbTables[dbtName].DataDbColumns.Have(c.RubricName)).Select(o => o.FigureFieldId));
                            FieldMapping iSqlbankMap = new FieldMapping(dbtName, keyOrdinal, colOrdinal);
                            if (t.Rubrics.Mappings == null)
                                t.Rubrics.Mappings = new FieldMappings();
                            t.Rubrics.Mappings.Add(iSqlbankMap.DbTableName, iSqlbankMap);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new SqlMapperException(ex.ToString());
            }
            CardsMapped = table;

        }

        public class SqlMapperException : Exception
        {
            public SqlMapperException(string message)
                : base(message)
            {

            }
        }

    }
}
