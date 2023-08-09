using System.ComponentModel.DataAnnotations;

namespace Solutions.Now.Moe.Elsa.Models
{
    public class projectStoppedTable
    {

        [Key]
        public int? serial { get; set; }
        public int? serialTender { get; set; }
        public int? SerialProject { get; set; }
    }
}
