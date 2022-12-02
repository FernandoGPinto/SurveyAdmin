using SurveyAdmin.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SurveyAdmin.Interfaces
{
    public interface IPCDQuestionRepository
    {
        Task<IEnumerable<PCDQuestionDto>> GetPCDQuestionsAsync();

        Task<IEnumerable<PCDQuestionDto>> GetPCDQuestionsAsync(string jobId);

        Task<IEnumerable<PCDQuestionAssignmentDto>> GetNonGenericPCDQuestionsAsync(string clientId);

        Task InsertQuestionAsync(PCDQuestion pcdQuestion);

        Task UpdateQuestionAsync(PCDQuestion pcdQuestion);

        Task UpdateQuestionsAsync(List<PCDQuestion> pcdQuestions);

        Task DeleteQuestionAsync(Guid Id);

        Task<Guid> GetFormatIdAsync(string format);
    }
}
