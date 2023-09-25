using System.ComponentModel.DataAnnotations;

namespace Solutions.Now.Moe.Elsa.Models.Construction
{
    public class InvoicesPayment
    {
        [Key]
        public int? Serial { get; set; }
        public int? tenderSerial { get; set; }
        public int? status { get; set; }
        public string accountant { get; set; }
    }
}
