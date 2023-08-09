using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Solutions.Now.Moe.Elsa.Models
{
    public class ResumeProjectWork
    {
        [Key]
        public int? Serial { get; set; }
        public int? TenderSerial { get; set; }
        public int? ProjectSerial { get; set; }
    }
}
