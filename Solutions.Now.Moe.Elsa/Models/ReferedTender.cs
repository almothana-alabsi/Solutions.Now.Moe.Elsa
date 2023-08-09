using System.ComponentModel.DataAnnotations;

namespace Solutions.Now.Moe.Elsa.Models
{
    public class ReferedTender
    {  [Key]
        public int? Serial { get; set; }
        public int? status { get; set; }
        public string? assignedEngineer { get; set; }
        public string? maintenanceEngineer { get; set; }
        public int? step { get; set; }   
        public int? Consultant { get; set; }

    }
}
