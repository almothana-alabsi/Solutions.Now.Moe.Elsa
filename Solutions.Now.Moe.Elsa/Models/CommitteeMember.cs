using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Solutions.Now.Moe.Elsa.Models
{
    public class CommitteeMember
    {
        [Key]
        public int? Serial { get; set; }
        public int? committeeSerial { get; set; }
        public string? userName { get; set; }
        public int? capten { get; set; }
        public int? major { get; set; }


    }
}
