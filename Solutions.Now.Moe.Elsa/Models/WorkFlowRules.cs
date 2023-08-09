using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Solutions.Now.Moe.Elsa.Models
{
    public class WorkFlowRules
    {
        [Key]
       // [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? serial { get; set; }
        public int? workflow { get; set; }
        public int? step { get; set; }
        public string? username { get; set; }
        public int? type { get; set; }
        public int? position { get; set; }
        public string? screen { get; set; }

    }
}
