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
    public class PCDSurveyResponseRepository : IPCDSurveyResponseRepository
    {
        private readonly DbSession _session;

        public PCDSurveyResponseRepository(DbSession session)
        {
            _session = session;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ViewPCDResponses>> GetSubmittedPCDsAsync(Guid clientId, DateTime? dateFrom, DateTime? dateTo)
        {
            string sqlQuery = "SELECT * FROM [View_PCDResponses] WHERE [ClientId] = @ClientId AND [DateTimeSubmittedUTC] >= @DateFrom AND [DateTimeSubmittedUTC] <= @DateTo";
            
            return await _session.Connection.QueryAsync<ViewPCDResponses>(sqlQuery, new { ClientId = clientId, DateFrom = dateFrom, DateTo = dateTo }, _session.Transaction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ViewPCDResponse>> GetPCDDetailsAsync(Guid jobId)
        {
            string sqlQuery = "SELECT [Question], [Response] FROM [dbo].[PCDSurveyResponses] INNER JOIN [dbo].[PCDQuestions] ON [dbo].[PCDQuestions].[Id] = [dbo].[PCDSurveyResponses].[QuestionId] WHERE [JobId] = @JobId ORDER BY [Index]";

            return await _session.Connection.QueryAsync<ViewPCDResponse>(sqlQuery, new { JobId = jobId }, _session.Transaction);
        }

        /// <summary>
        /// Inserts survey results into the PCDSurveyResponses table.
        /// </summary>
        /// <param name="surveyResults"></param>
        /// <returns></returns>
        public async Task InsertSurveyResults(PCDSurveyResults surveyResults)
        {
            // Convert results into a list of responses with respective job id.
            var responses = surveyResults.Responses.Select(s => new { s.QuestionId, s.Response, surveyResults.JobId });

            string sqlQuery = "INSERT INTO [PCDSurveyResponses] ([QuestionId], [JobId], [Response]) VALUES (@QuestionId, @JobId, @Response)";

            await _session.Connection.ExecuteAsync(sqlQuery, responses, _session.Transaction);
        }
    }
}
