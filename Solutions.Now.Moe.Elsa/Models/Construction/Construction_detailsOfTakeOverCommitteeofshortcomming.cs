using System;
using System.ComponentModel.DataAnnotations;

namespace Solutions.Now.Moe.Elsa.Models.Construction
{
    public class Construction_detailsOfTakeOverCommitteeofshortcomming
    {
        [Key]
        public int? serial { get; set; }
        public int? tenderSerial { get; set; }  
        public int takeOverSerial {  get; set; } 
        public int projectSerial { get; set; }      
        public DateTime? contractorResponsibilityEndDate { get; set; } 

    }
}
