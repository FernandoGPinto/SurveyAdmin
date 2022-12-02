using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyAdmin.Models
{
    public class PCDQuestionFormat
    {
        public Guid Id { get; set; }
        public string Format { get; set; }
    }
}
