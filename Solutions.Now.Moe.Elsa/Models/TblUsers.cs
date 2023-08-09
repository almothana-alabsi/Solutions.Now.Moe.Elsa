using System.ComponentModel.DataAnnotations;

namespace Solutions.Now.Moe.Elsa.Models
{
    public class TblUsers
    {
        [Key]
        public int serial { get; set; }
        public string username { get; set; }
        //public string? password { get; set; }
        //public int? userType { get; set; }
        //public string pic { get; set; }
        //public int? status { get; set; }
        public int? position { get; set; }
        public int? Permission { get; set; }
        //public string? nameAR { get; set; }
        //public string? nameEN { get; set; }
        //public string? nationalNO { get; set; }
        //public string? email { get; set; }
        //public string? phone { get; set; }
        //public string? statusNote { get; set; }
        public int? Administration { get; set; }
        public int? Directorate { get; set; }
        public int? Section { get; set; }
        //public string? apikey { get; set; }
        public int? Major { get; set; }
        public int? organization { get; set; }
        public int? Consultant { get; set; }
        public int? contractor { get; set; }
    }
}
