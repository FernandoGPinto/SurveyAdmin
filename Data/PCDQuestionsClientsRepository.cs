using Dapper;
using SurveyAdmin.Interfaces;
using SurveyAdmin.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyAdmin.Data
{
    public class PCDQuestionsClientsRepository : IPCDQuestionsClientsRepository
    {
        private readonly DbSession _session;

        public PCDQuestionsClientsRepository(DbSession session)
        {
            _session = session;
        }

        /// <summary>
        /// Inserts question assignments into the PCDQuestionsClients junction table.
        /// </summary>
        /// <param name="questionAssignments"></param>
        /// <returns></returns>
        public async Task InsertQuestionAssignments(PCDQuestionsClients[] questionAssignments)
        {
            string sqlQuery = "INSERT INTO [PCDQuestionsClients] ([QuestionId], [ClientId]) VALUES (@QuestionId, @ClientId)";

            await _session.Connection.ExecuteAsync(sqlQuery, questionAssignments, _session.Transaction);
        }

        /// <summary>
        /// Deletes question assignments from the PCDQuestionsClients junction table.
        /// </summary>
        /// <param name="questionAssignments"></param>
        /// <returns></returns>
        public async Task DeleteQuestionAssignments(PCDQuestionsClients[] questionAssignments)
        {
            string sqlQuery = "DELETE FROM [PCDQuestionsClients] WHERE [QuestionId] = @QuestionId AND [ClientId] = @ClientId";

            await _session.Connection.ExecuteAsync(sqlQuery, questionAssignments, _session.Transaction);
        }
    }
}
