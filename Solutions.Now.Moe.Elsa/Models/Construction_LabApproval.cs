using System.ComponentModel.DataAnnotations;

namespace Solutions.Now.Moe.Elsa.Models
{
    public class Construction_LabApproval
    {
        [Key]
        public int? serial { get; set; }
        public int? tenderSerial { get; set; }
    }
}
