using SurveyAdmin.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace SurveyAdmin.Interfaces
{
    public interface IPCDSurveyResponseRepository
    {
        Task<IEnumerable<ViewPCDResponses>> GetSubmittedPCDsAsync(Guid clientId, DateTime? dateFrom, DateTime? dateTo);

        Task<IEnumerable<ViewPCDResponse>> GetPCDDetailsAsync(Guid jobId);

        Task InsertSurveyResults(PCDSurveyResults surveyResults);
    }
}
