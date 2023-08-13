using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Solutions.Now.Moe.Elsa.Models.Construction.DTOs
{
    public class changeOrderTypeDto
    {
        [Display(Name = "resultF")]
        public bool? resultF { get; set; }
        [Display(Name = "resultT")]
        public bool? resultT { get; set; }
        [Display(Name = "result")]
        public bool? result { get; set; }
    }
}
