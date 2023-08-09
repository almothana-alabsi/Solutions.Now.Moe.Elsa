using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Solutions.Now.Moe.Elsa.Models
{
    public class ApprovalHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? serial { get; set; }
        [Column(TypeName = "int")]
        public int? requestSerial { get; set; }
        [Column(TypeName = "int")]
        public int? requestType { get; set; }
        public DateTime? createdDate { get; set; }
        public string? actionBy { get; set; }
        public DateTime? actionDate { get; set; }
        public DateTime? expireDate { get; set; }
        [Column(TypeName = "int")]
        public int? status { get; set; }
        public string? note { get; set; }
        public string? URL { get; set; }
        [Column(TypeName = "int")]
        public int? seen { get; set; }
        public string? Form { get; set; }
        [Column(TypeName = "int")]
        public int? step { get; set; }
        [Column(TypeName = "int")]
        public int? ActionDetails { get; set; }
        public string? Attachements { get; set; }
        public int? refSerial { get; set; }
        public string? createdBy { get; set; }
        public int? Stage { get; set; }
    }
}
