using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Solutions.Now.Moe.Elsa.Models
{
    public class SiteVisit
    {
        [Key]
        public int? Serial { get; set; }
        public int? ProjectSerial { get; set; }
        public int? tenderSerial { get; set; }
        public int? status { get; set; }
        public string? RequestBy { get; set; }
        public DateTime? visitDate { get; set; }
    }
}
