using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Solutions.Now.Moe.Elsa.Models
{
    public class StagesDataForRequestProject
    {
        [Display(Name = "requestSerial")]
        public int? requestSerial { get; set; }

        [Display(Name = "stage")]
        public int? stage { get; set; }
    }
}
