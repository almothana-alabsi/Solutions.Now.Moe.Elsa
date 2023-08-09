using System.ComponentModel.DataAnnotations;

namespace Solutions.Now.Moe.Elsa.Models.Construction
{
    public class Construction_CommitteeMember
    {
        [Key]
        public int? serial { get; set; }
        public int? type { get; set; }
        public int? projectSerial { get; set; }
        public int? tenderSerial { get; set; }
        public int? masterSerial { get; set; }
        public string? userName { get; set; }
        public int? captain { get; set; }
        public int? section { get; set; }
    }
}
