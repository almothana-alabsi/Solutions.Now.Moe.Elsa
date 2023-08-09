using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Solutions.Now.Moe.Elsa.Models
{
    public class OutputActivityData
    {
        [Display(Name = "requestSerial")]
        public int? requestSerial { get; set; }
        [Display(Name = "refRequestSerial")]
        public int? refRequestSerial { get; set; }
        [Display(Name = "steps")]
        public IList<int?> steps { get; set; }
        [Display(Name = "names")]
        public IList<string?> names { get; set; }
        [Display(Name = "screen")]
        public IList<string?> Screens { get; set; }
        [Display(Name = "sendTo")]
        public int? sendTo { get; set; }
        [Display(Name = "requester")]
        public string? requester { get; set; }
        [Display(Name = "major")]
        public int? major { get; set; }

    }
}