using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Solutions.Now.Moe.Elsa.Models
{
    public class ProjectsTender
    {
        [Key]
        public int? Serial { get; set; }
        public int? requestSerial { get; set; }
        public int? tenderSerial { get; set; }
        public int? stage { get; set; }




    }
}
