using System.ComponentModel.DataAnnotations;

namespace Solutions.Now.Moe.Elsa.Models
{
    public class Committee
    {
        [Key]
        public int? Serial { get; set; }
        public int? committeeType { get; set; }
        public int? ProjectSerial { get; set; }
        public int? TenderSerial { get; set; }
        public int? status { get; set; }

    }
}
