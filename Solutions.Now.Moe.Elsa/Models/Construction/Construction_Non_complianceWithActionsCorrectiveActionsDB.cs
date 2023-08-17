using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Solutions.Now.Moe.Elsa.Models.Construction.DTOs
{
    public class Construction_Non_complianceWithActionsCorrectiveActionsDB
    {
        [Key]
        public int? serial { get; set; }
        public int? tenderSerial { get; set; }
        public int? tenderSupervisor { get; set; }
    }
}
