using System.ComponentModel.DataAnnotations;
using System;

namespace Solutions.Now.Moe.Elsa.Models.Construction
{
    public class Construction_ProceduresForSubmittingSiteMemorandum
    {
        [Key]
        public int? serial { get; set; }
        public int? tenderSerial { get; set; }
        public int? projectSerial { get; set; }
        public DateTime? dateOfSubmission { get; set; }
        public string attachments { get; set; }
        public string actionBy { get; set; }
        public string actionTo { get; set; }
        public string subject { get; set; }
        public string images { get; set; }
        public int? status { get; set; }
        public string reply { get; set; }

        public int isChecked { get; set; }
}
}
