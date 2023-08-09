using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Solutions.Now.Moe.Elsa.Models
{
    public class VisitMember
    { 
        [Key]
        public int Serial { get; set; }
        public int? siteVisitSerial { get; set; }
        public string? name { get; set; }

    }
}
