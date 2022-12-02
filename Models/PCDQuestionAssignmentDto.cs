using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyAdmin.Models
{
    public class PCDQuestionAssignmentDto
    {
        public Guid QuestionId { get; set; }
        public int Index { get; set; }
        public string Question { get; set; }
        public bool Assigned { get; set; }

        public PCDQuestionAssignmentDto() { }

        public PCDQuestionAssignmentDto(PCDQuestionAssignmentDto previousAssignment)
        {
            QuestionId = previousAssignment.QuestionId;
            Index = previousAssignment.Index;
            Question = previousAssignment.Question;
            Assigned = previousAssignment.Assigned;
        }
    }
}
