using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SurveyAdmin.Interfaces;
using SurveyAdmin.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyAdmin.Data
{
    public class PCDQuestionRepository : IPCDQuestionRepository
    {
        private readonly DbSession _session;

        public PCDQuestionRepository(DbSession session)
        {
            _session = session;
        }

        /// <summary>
        /// Gets all pcd questions, returned in a list.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<PCDQuestionDto>> GetPCDQuestionsAsync()
        {
            string sqlQuery = "SELECT [PCDQuestions].[Id], [Question], f.[Format] AS [QuestionFormat], [RecordUpdatedUTC], [RecordUpdatedBy], [Index], [IsGenericQuestion], [IsClientQuestion], [IsSQLQuery], [SQLQuery], [HasFurtherComment], [FurtherCommentId] FROM [PCDQuestions] JOIN [PCDQuestionFormats] f ON f.Id = [PCDQuestions].QuestionFormatId WHERE [IsEnabled] = '1' ORDER BY [Index] ASC";
            
            return await _session.Connection.QueryAsync<PCDQuestionDto>(sqlQuery, null, _session.Transaction);
        }

        /// <summary>
        /// Gets all pcd questions for the client id plus all generic pcd questions, returned in a list.
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<PCDQuestionDto>> GetPCDQuestionsAsync(string jobId)
        {
            string sqlQuery = "SELECT a.[Id], [Index], [Question], f.[Format] AS [QuestionFormat], [RecordUpdatedBy], [IsGenericQuestion], [IsClientQuestion], [IsSQLQuery], [SQLQuery], [HasFurtherComment], [FurtherCommentId] FROM (SELECT [PCDQuestions].[Id], [Question], [QuestionFormatId], [RecordUpdatedBy], [Index], [IsGenericQuestion], [IsClientQuestion], [IsSQLQuery], [SQLQuery], [HasFurtherComment], [FurtherCommentId], [IsEnabled] FROM [dbo].[PCDQuestions] INNER JOIN [dbo].[Job] j on j.Id = @JobId INNER JOIN [dbo].[Site] s on j.SiteId = s.Id INNER JOIN [dbo].[PCDQuestionsClients] p ON p.QuestionId = [PCDQuestions].Id AND p.ClientId = s.ClientId UNION SELECT [Id], [Question], [QuestionFormatId], [RecordUpdatedBy], [Index], [IsGenericQuestion], [IsClientQuestion], [IsSQLQuery], [SQLQuery], [HasFurtherComment], [FurtherCommentId], [IsEnabled] FROM [dbo].[PCDQuestions] WHERE [IsGenericQuestion] = '1') a JOIN [dbo].[PCDQuestionFormats] f ON f.Id = a.QuestionFormatId WHERE [IsEnabled] = '1' ORDER BY [Index] ASC";

            var result = await _session.Connection.QueryAsync<PCDQuestionDto>(sqlQuery, new { JobId = jobId }, _session.Transaction);

            // Iterate the result to check for SQL queries in the "SQLQuery" column.
            foreach (var record in result)
            {
                // If the value for this question comes from a sql query, run query and assign value to field "Value". This value will later be used to populate the input and set it to read only.
                if (record.IsSQLQuery == "True" && !String.IsNullOrWhiteSpace(record.SQLQuery))
                {
                    // Run query. All queries contain the parameter JobId.
                    var value = await _session.Connection.QueryAsync<PCDQuestionDto>(record.SQLQuery, new { JobId = jobId }, _session.Transaction);

                    // Populate "Value" field with query result.
                        record.Value = value.FirstOrDefault().Value.Trim();
                }
            }

            return result;
        }

        /// <summary>
        /// Gets non-generic pcd questions with a flag to indicate whether they are assigned to the specified client, returned in a list.
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<PCDQuestionAssignmentDto>> GetNonGenericPCDQuestionsAsync(string clientId)
        {
            string sqlQuery = "SELECT [Id] AS [QuestionId], [Index], [Question], CASE WHEN q.QuestionId IS NULL THEN CAST('0' AS BIT) ELSE CAST('1' AS BIT) END AS [Assigned] FROM [PCDQuestions] LEFT JOIN [PCDQuestionsClients] q ON q.QuestionId = [PCDQuestions].Id AND [ClientId] = @ClientId WHERE [IsGenericQuestion] = '0' AND [IsEnabled] = '1'";

            return await _session.Connection.QueryAsync<PCDQuestionAssignmentDto>(sqlQuery, new { ClientId = clientId }, _session.Transaction);
        }

        /// <summary>
        /// Inserts new pcd question.
        /// </summary>
        /// <param name="pcdQuestion"></param>
        /// <returns></returns>
        public async Task InsertQuestionAsync(PCDQuestion pcdQuestion)
        {
            // Use the Id_Sequence table to increment the value of Index, which is not an Identity column. This allows questions to be re-ordered.
            string sqlQuery = "INSERT INTO [PCDQuestions] ([Index], [Question], [QuestionFormatId], [RecordUpdatedBy], [IsGenericQuestion], [IsClientQuestion], [IsSQLQuery], [SQLQuery], [HasFurtherComment], [IsEnabled]) SELECT NEXT VALUE FOR dbo.Id_Sequence, @Question, @QuestionFormatId, @RecordUpdatedBy, @IsGenericQuestion, @IsClientQuestion, @IsSQLQuery, @SQLQuery, @HasFurtherComment, '1'";

            await _session.Connection.ExecuteAsync(sqlQuery, pcdQuestion, _session.Transaction);
        }

        /// <summary>
        /// Updates a pcd question.
        /// </summary>
        /// <param name="pcdQuestion"></param>
        /// <returns></returns>
        public async Task UpdateQuestionAsync(PCDQuestion pcdQuestion)
        {
            string sqlQuery = "UPDATE [PCDQuestions] SET [Question] = @Question, [Index] = @Index, [QuestionFormatId] = @QuestionFormatId, [RecordUpdatedBy] = @RecordUpdatedBy, [IsGenericQuestion] = @IsGenericQuestion, [IsClientQuestion] = @IsClientQuestion, [IsSQLQuery] = @IsSQLQuery, [SQLQuery] = @SQLQuery, [HasFurtherComment] = @HasFurtherComment WHERE [Id] = @Id";

            await _session.Connection.ExecuteAsync(sqlQuery, pcdQuestion, _session.Transaction);
        }

        /// <summary>
        /// Updates a selection of pcd questions.
        /// </summary>
        /// <param name="pcdQuestions"></param>
        /// <returns></returns>
        public async Task UpdateQuestionsAsync(List<PCDQuestion> pcdQuestions)
        {
            string sqlQuery = "UPDATE [PCDQuestions] SET [Question] = @Question, [Index] = @Index, [QuestionFormatId] = @QuestionFormatId, [RecordUpdatedBy] = @RecordUpdatedBy, [IsGenericQuestion] = @IsGenericQuestion, [IsClientQuestion] = @IsClientQuestion, [IsSQLQuery] = @IsSQLQuery, [SQLQuery] = @SQLQuery, [HasFurtherComment] = @HasFurtherComment WHERE [Id] = @Id";

            await _session.Connection.ExecuteAsync(sqlQuery, pcdQuestions, _session.Transaction);
        }

        /// <summary>
        /// Deletes a pcd question.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task DeleteQuestionAsync(Guid Id)
        {
            //string sqlQuery = "DELETE FROM [PCDQuestions] WHERE [Id] = @Id";

            string sqlQuery = "UPDATE [PCDQuestions] SET [IsEnabled] = '0' WHERE [Id] = @Id";

            await _session.Connection.ExecuteAsync(sqlQuery, new { Id = Id }, _session.Transaction);
        }

        /// <summary>
        /// Gets the corresponding Format Id for the Format provided.
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public async Task<Guid> GetFormatIdAsync(string format)
        {
            string sqlQuery = "SELECT [Id] FROM [PCDQuestionFormats] WHERE [Format] = @Format";

            var result = await _session.Connection.QueryAsync<PCDQuestionFormat>(sqlQuery, new { Format = format }, _session.Transaction);

            return result.FirstOrDefault().Id;
        }
    }
}
