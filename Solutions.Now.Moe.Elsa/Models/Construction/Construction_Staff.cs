﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Solutions.Now.Moe.Elsa.Models.Construction
{
    public class Construction_Staff
    {
        [Key]
        public int? serial { get; set; }
        public int? tenderSerial { get; set; }
        public int? projectSerial { get; set; }
        public int? majorStaffRequestsSerial { get; set; }
    }
}
