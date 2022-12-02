using SurveyAdmin.Models;
using System.Threading.Tasks;

namespace SurveyAdmin.Interfaces
{
    public interface IPCDQuestionsClientsRepository
    {
        Task InsertQuestionAssignments(PCDQuestionsClients[] questionAssignments);

        Task DeleteQuestionAssignments(PCDQuestionsClients[] questionAssignments);
    }
}
