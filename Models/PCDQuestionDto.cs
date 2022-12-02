using System;
using System.Collections.Generic;
using System.Text;

namespace SurveyAdmin.Models
{
    public class PCDQuestionDto
    {
        public Guid Id { get; set; }
        public int Index { get; set; }
        public string Question { get; set; }
        public string QuestionFormat { get; set; }
        public string IsGenericQuestion { get; set; }
        public string IsClientQuestion { get; set; }
        public string IsSQLQuery { get; set; }
        public string SQLQuery { get; set; }
        public string HasFurtherComment { get; set; }
        public Guid FurtherCommentId { get; set; }
        public string RecordUpdatedBy { get; set; }
        public string Value { get; set; }
    }
}
