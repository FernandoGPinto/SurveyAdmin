using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SurveyAdmin.Interfaces;
using SurveyAdmin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyAdmin.Data
{
    public class ClientRepository : IClientRepository
    {
        private readonly DbSession _session;

        public ClientRepository(DbSession session)
        {
            _session = session;
        }

        /// <summary>
        /// Gets all clients, returned in a list.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Client>> GetClientsAsync()
        {
            string sqlQuery = "SELECT * FROM [Client] ORDER BY [Name] ASC";

            return await _session.Connection.QueryAsync<Client>(sqlQuery, _session.Transaction);
        }
    }
}
