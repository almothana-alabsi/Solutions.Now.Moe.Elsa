using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Solutions.Now.Moe.Elsa.Models.Construction.DTOs
{
    public class directOrderToTheContractorDTO
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

        [Display(Name = "RequestSender")]
        public string? RequestSender { get; set; }

        [Display(Name = "IsSecyionHead")]
        public int? IsSecyionHead { get; set; }

    }
}
