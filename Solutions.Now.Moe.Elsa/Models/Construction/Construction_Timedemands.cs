using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;

namespace Solutions.Now.Moe.Elsa.Models.Construction
{
    public class Construction_Timedemands
    {
        [Key]
        public int serial{ get; set; }
       public int tenderSerial { get; set; }

    }
}
