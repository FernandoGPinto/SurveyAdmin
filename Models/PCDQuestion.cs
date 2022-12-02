using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyAdmin.Models
{
    public class PCDQuestion
    {
        public Guid Id { get; set; }
        public int Index { get; set; }
        public string Question { get; set; }
        public Guid QuestionFormatId { get; set; }
        public bool IsGenericQuestion { get; set; }
        public bool IsClientQuestion { get; set; }
        public bool IsSQLQuery { get; set; }
        public string SQLQuery { get; set; }
        public bool HasFurtherComment { get; set; }
        public Guid FurtherCommentId { get; set; }
        public string RecordUpdatedBy { get; set; }
    }

    /// <summary>
    /// Enables the correct comparison of two PCDQuestion objects.
    /// </summary>
    public class PCDQuestionComparer : IEqualityComparer <PCDQuestion>
    {
        public bool Equals(PCDQuestion q1, PCDQuestion q2)
        {
            return q1.Id == q2.Id && q1.HasFurtherComment == q2.HasFurtherComment && q1.Index == q2.Index && q1.IsClientQuestion == q2.IsClientQuestion && q1.IsGenericQuestion == q2.IsGenericQuestion && q1.IsSQLQuery == q2.IsSQLQuery && q1.Question == q2.Question && q1.QuestionFormatId == q2.QuestionFormatId && q1.RecordUpdatedBy == q2.RecordUpdatedBy && q1.SQLQuery == q2.SQLQuery;
        }

        public int GetHashCode(PCDQuestion obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
