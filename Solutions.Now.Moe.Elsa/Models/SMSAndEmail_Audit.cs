using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace Solutions.Now.Moe.Elsa.Models
{
    public class SMSAndEmail_Audit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int serial { get; set; }
        public string? email { get; set; }
        public string? username { get; set; }
        public string? message { get; set; }
        public int? requestSerial { get; set; }
        public int? requestType { get; set; }
        public DateTime? actionDate { get; set; }
        public bool? isSuccess { get; set; }
        public int? organisation { get; set; }
        public string? phoneNumber { get; set; }
    }
}
