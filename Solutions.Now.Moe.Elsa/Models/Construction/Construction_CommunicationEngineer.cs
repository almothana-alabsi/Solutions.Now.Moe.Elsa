﻿using System.ComponentModel.DataAnnotations;

namespace Solutions.Now.Moe.Elsa.Models.Construction
{
    public class Construction_CommunicationEngineer
    {
        [Key]
        public int? serial { get; set; }
        public int? tenderSerial { get; set; }
    }
}
