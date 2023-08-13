using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Solutions.Now.Moe.Elsa.Models.Construction
{
    public class Construction_partial_Receipt_Works_DB
    {
        [Key]
        public int? serial { get; set; }
        public int? tenderSerial { get; set; }
    }
}
