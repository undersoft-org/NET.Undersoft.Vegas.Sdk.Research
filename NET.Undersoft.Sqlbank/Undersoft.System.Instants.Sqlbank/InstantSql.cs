using System.Linq;
using System.Multemic;
using System.Data.SqlClient;

namespace System.Instants.Sqlbank
{
    public class InstantSql
    {
        private SqlIdentity identity;
        private string cnString
        {
            get => identity.ConnectionString;
            set => identity.ConnectionString = value;
        }
        private SqlConnection sqlcn;       
        private SqlUpdate update;
        private SqlInsert insert;
        private SqlAccessor accessor;
        private SqlMutator mutator;
        private SqlMapper mapper;

        public InstantSql(string SqlConnectionString)
        {
            identity = new SqlIdentity();
            cnString = SqlConnectionString;
            sqlcn = new SqlConnection(cnString);
            Initialization();
        }
        public InstantSql(SqlConnection SqlDbConnection) 
            : this(SqlDbConnection.ConnectionString)
        {
        }
        public InstantSql(SqlIdentity sqlIdentity)          
        {
            identity = sqlIdentity;
            sqlcn = new SqlConnection(cnString);
            Initialization();
        }

        private void Initialization()
        {
            string dbName = sqlcn.Database;
            SqlSchemaBuild SchemaBuild = new SqlSchemaBuild(sqlcn);
            SchemaBuild.SchemaPrepare(BuildDbSchemaType.Schema);
            sqlcn.ChangeDatabase("tempdb");
            SchemaBuild.SchemaPrepare(BuildDbSchemaType.Temp);
            sqlcn.ChangeDatabase(dbName);
            accessor = new SqlAccessor();
            mutator = new SqlMutator(this);
        }

        public InstantMultemic GetInstant(string sqlQry, string tableName, IDeck<string> keyNames = null)
        {
            return accessor.GetBank(cnString, sqlQry, tableName, keyNames);
        }

        public Album<Album<IFigure>> SaveInstant(IMultemic cards, bool renew)
        {
            return mutator.SetBank(cnString, cards, renew);
        }

        public Album<Album<IFigure>> 
                   Update(IMultemic cards, bool keysFromDeck = false, bool buildMapping = false, bool updateKeys = false, string[] updateExcept = null, BulkPrepareType tempType = BulkPrepareType.Trunc)
        {
            if(update == null)
                update = new SqlUpdate(sqlcn);
            return update.Update(cards, keysFromDeck, buildMapping, updateKeys, updateExcept, tempType);
        }
        public Album<Album<IFigure>> 
                   BatchUpdate(IMultemic cards, bool keysFromDeck = false, bool buildMapping = false, bool updateKeys = false, string[] updateExcept = null)
        {
            if (update == null)
                update = new SqlUpdate(sqlcn);
            return update.BatchUpdate(cards, keysFromDeck, buildMapping, updateKeys, updateExcept);
        }
        public int SimpleUpdate(IMultemic cards, bool buildMapping = false, bool updateKeys = false, string[] updateExcept = null)
        {
            if (update == null)
                update = new SqlUpdate(sqlcn);
            return update.SimpleUpdate(cards, buildMapping, updateKeys, updateExcept);
        }

        public Album<Album<IFigure>> 
                   Insert(IMultemic cards, bool keysFromDeck = false, bool buildMapping = false, BulkPrepareType tempType = BulkPrepareType.Trunc)
        {
            if (insert == null)
                insert = new SqlInsert(sqlcn);
            return insert.Insert(cards, keysFromDeck, buildMapping, false, null, tempType);
        }
        public Album<Album<IFigure>> 
                   BatchInsert(IMultemic cards, bool buildMapping)
        {
            if (insert == null)
                insert = new SqlInsert(sqlcn);
            return insert.BatchInsert(cards, buildMapping);
        }
        public int SimpleInsert(IMultemic cards, bool buildMapping)
        {
            if (insert == null)
                insert = new SqlInsert(sqlcn);
            return insert.SimpleInsert(cards, buildMapping);
        }
        public int SimpleInsert(IMultemic cards)
        {
            if (insert == null)
                insert = new SqlInsert(sqlcn);
            return insert.SimpleInsert(cards);
        }

        public int Execute(string query)
        {
            SqlCommand cmd = sqlcn.CreateCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = query;
            return cmd.ExecuteNonQuery();
        }

        public IMultemic Mapper(IMultemic cards, bool keysFromDeck = false, string[] dbTableNames = null, string tablePrefix = "")
        {
            mapper = new SqlMapper(cards, keysFromDeck, dbTableNames, tablePrefix);
            return mapper.CardsMapped;
        }
    }
 
    public class SqlException : Exception
    {
        public SqlException(string message)
            : base(message)
        {

        }
    }
}
