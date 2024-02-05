using System.ComponentModel.DataAnnotations;

namespace Solutions.Now.Moe.Elsa.Models.Construction
{
    public class Construction_officialBooks
    {
        [Key]
        public int? serial { get; set; }
        public int? tenderSerial { get; set; }
        public int? projectSerial { get; set; }
    }
}
