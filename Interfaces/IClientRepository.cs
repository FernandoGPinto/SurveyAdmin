using SurveyAdmin.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SurveyAdmin.Interfaces
{
    public interface IClientRepository
    {
        Task<IEnumerable<Client>> GetClientsAsync();
    }
}
