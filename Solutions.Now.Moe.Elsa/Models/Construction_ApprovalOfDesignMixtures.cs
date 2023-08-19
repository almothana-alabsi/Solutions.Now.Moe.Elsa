using System.ComponentModel.DataAnnotations;

namespace Solutions.Now.Moe.Elsa.Models
{
    public class Construction_ApprovalOfDesignMixtures
    {
        [Key]
        public int? serial { get; set; }
        public int? tenderSerial { get; set; }
    }
}
