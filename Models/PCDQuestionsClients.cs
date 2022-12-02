using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyAdmin.Models
{
    public class PCDQuestionsClients
    {
        public Guid QuestionId { get; set; }
        public Guid ClientId { get; set; }
    }
}
