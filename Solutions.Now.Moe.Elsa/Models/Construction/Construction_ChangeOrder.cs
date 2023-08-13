using System.ComponentModel.DataAnnotations;

namespace Solutions.Now.Moe.Elsa.Models.Construction
{
    public class Construction_ChangeOrder
    {
        [Key]
        public int? serial { get; set; }
 
        public int? amount { get; set; }
        public int? tenderSerial { get; set; }
    }
}
