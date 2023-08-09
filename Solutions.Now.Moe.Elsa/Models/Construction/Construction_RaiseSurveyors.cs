using System.ComponentModel.DataAnnotations;
using System;

namespace Solutions.Now.Moe.Elsa.Models.Construction
{
    public class Construction_RaiseSurveyors
    {
        [Key]
        public int? serial { get; set; }
        public int? tenderSerial { get; set; }
        public int? projectSerial { get; set; }
        public string? surveyCommittee { get; set; }
        public int? retinalMatchingWithContractualSchemes { get; set; }
    }
}
