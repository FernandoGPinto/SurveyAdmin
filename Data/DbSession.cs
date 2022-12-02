using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyAdmin.Data
{
    public sealed class DbSession : IDisposable
    {
        public IDbConnection Connection { get; }
        public IDbTransaction Transaction { get; set; }

        public DbSession(IConfiguration config)
        {
            Connection = new SqlConnection(config.GetConnectionString("OCSStart_Connection"));
            Connection.Open();
        }

        public void Dispose() => Connection?.Dispose();
    }
}
