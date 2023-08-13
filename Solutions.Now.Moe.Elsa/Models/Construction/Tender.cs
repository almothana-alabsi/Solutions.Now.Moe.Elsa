using System.ComponentModel.DataAnnotations;

namespace Solutions.Now.Moe.Elsa.Models.Construction
{
    public class Tender
    {
        [Key]
        public int? tenderSerial { get; set; }
        public int? tenderContracter1 { get; set; }
        public int? tenderSupervisor { get; set; }
        public string? tendersDepartmentEngineer { get; set; }

        public decimal? tenderAmountUponAssignment { get; set; }
    }
}
