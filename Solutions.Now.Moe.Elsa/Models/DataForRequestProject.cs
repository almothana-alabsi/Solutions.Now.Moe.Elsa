using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Solutions.Now.Moe.Elsa.Models
{
    public class DataForRequestProject
    {
        [Display(Name = "requestSerial")]
        public int? requestSerial { get; set; }

        [Display(Name = "userName")]
        public string? userName { get; set; }

        [Display(Name = "name")]
        public List<string?> name { get; set; }

        [Display(Name = "steps")]
        public List<int?> steps { get; set; }

        [Display(Name = "major")]
        public int? major { get; set; }

        [Display(Name = "Screens")]
        public List<string?> Screens { get; set; }

        [Display(Name = "stage")]
        public int? stage { get; set; }

        [Display(Name = "RequestSender")]
        public string? RequestSender { get; set; }

        [Display(Name = "requestType")]
        public int? requestType { get; set; }

    }
}
