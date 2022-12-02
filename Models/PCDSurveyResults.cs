using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyAdmin.Models
{
    public class PCDSurveyResults
    {
        public string JobId { get; set; }
        public string SurveyId { get; set; }
        public List<PCDSurveyResponse> Responses { get; set; }
    }
}
