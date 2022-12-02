using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyAdmin.Models
{
    public class PCDSurveyResponse
    {
        public Guid QuestionId { get; set; }
        public string Response { get; set; }
    }
}
