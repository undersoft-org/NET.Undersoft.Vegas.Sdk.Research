using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;

namespace System.Instants.Sqlbank
{
    public class SqlRenew
    {
        private SqlConnection _cn;

        public SqlRenew(SqlConnection cn)
        {
            _cn = cn;
        }
        public SqlRenew(string cnstring)
        {
            _cn = new SqlConnection(cnstring);
        }      

        public class SqlRenewException : Exception
        {
            public SqlRenewException(string message)
                : base(message)
            {

            }
        }

    }
}
