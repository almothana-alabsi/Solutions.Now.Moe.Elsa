using System.ComponentModel.DataAnnotations;
using System;

namespace Solutions.Now.Moe.Elsa.Models
{
    public class ChangeOrder
    {
        [Key]
        public int? Serial { get; set; }
        public int? ProjectSerial { get; set; }
        public int? tenderSerial { get; set; }
       
    }
}
