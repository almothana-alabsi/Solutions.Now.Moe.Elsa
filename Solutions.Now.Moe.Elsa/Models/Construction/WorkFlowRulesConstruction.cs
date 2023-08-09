using System.ComponentModel.DataAnnotations;

namespace Solutions.Now.Moe.Elsa.Models.Construction
{
    public class WorkFlowRulesConstruction
    {
       
            [Key]
            public int? serial { get; set; }
            public int? workflow { get; set; }
            public int? step { get; set; }
            public string? username { get; set; }
            public string? screen { get; set; }

        
    }
}
