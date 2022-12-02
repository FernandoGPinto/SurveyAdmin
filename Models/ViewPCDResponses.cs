using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyAdmin.Models
{
    public class ViewPCDResponses
    {
        public Guid Id { get; set; }
        public string JobNumber { get; set; }
        public string SiteName { get; set; }
        public string Name { get; set; }
        public string CountManager { get; set; }
        public string Productivity { get; set; }
        public string FinalFinish { get; set; }
        public string SalesFloorLength { get; set; }
        public string GrossAccuracy { get; set; }
        public string TimeKeeping { get; set; }
        public string DressCode { get; set; }
        public string Equipment { get; set; }
        public string Procedures { get; set; }
        public string Communication { get; set; }
        public string Accuracy { get; set; }
        public string Tidiness { get; set; }
        public string WrapUp { get; set; }
        public string OverallPerception { get; set; }
        public string StoreComment { get; set; }
        public DateTime DateTimeSubmittedUTC { get; set; }
    }
}
