using System.Multemic;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

namespace System.Instants.Sqlbank
{
    public class SqlAccessor
    {
        public SqlAccessor()
        {
        }

        public InstantMultemic GetBank(string sqlConnectString, 
                                          string sqlQry, 
                                          string tableName, 
                                          IDeck<string> keyNames)
        {
            try
            {
                if (DbHand.Schema == null|| DbHand.Schema.DbTables.Count == 0)
                {
                    InstantSql sqb = new InstantSql(sqlConnectString);
                }
                SqlAdapter sqa = new SqlAdapter(sqlConnectString);

                try
                {
                        return sqa.ExecuteInject(sqlQry, tableName, keyNames);
                }
                catch(Exception ex)
                {
                    throw new SqlException(ex.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object GetSqlDataTable(object parameters)
        {
            try
            {
                Dictionary<string, object> param = new Dictionary<string, object>((Dictionary<string, object>)parameters);
                string sqlQry = param["SqlQuery"].ToString();
                string sqlConnectString = param["ConnectionString"].ToString();

                DataTable Table = new DataTable();
                SqlConnection sqlcn = new SqlConnection(sqlConnectString);
                sqlcn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sqlQry, sqlcn);
                adapter.Fill(Table);
                return Table;
            }
            catch (Exception ex)
            {
                return null;
            }
        }         
        public DataTable GetSqlDataTable(SqlCommand cmd)
        {
            try
            {

                DataTable Table = new DataTable();
                if (cmd.Connection.State == ConnectionState.Closed)
                    cmd.Connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(Table);
                return Table;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public DataTable GetSqlDataTable(string qry, SqlConnection cn)
        {
            try
            {
                DataTable Table = new DataTable();
                if (cn.State == ConnectionState.Closed)
                    cn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(qry, cn);
                adapter.Fill(Table);
                return Table;
            }
            catch (Exception ex)
            {
                return null;
            }
        }       
    }
}
