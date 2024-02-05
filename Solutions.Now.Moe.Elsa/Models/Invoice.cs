using System.ComponentModel.DataAnnotations;

namespace Solutions.Now.Moe.Elsa.Models
{
    public class Invoice 
    {
        [Key]
        public int? Serial { get; set; }
        public int? projectSerial { get; set; }
        public int? tenderSerial { get; set; }
        public decimal? valueOfPrebidPayments { get; set; }

        public decimal? valueOfPreprojectPayments { get; set; }

        public decimal? stageFeeRatio { get; set; }

        public int? stage { get; set; }

        public decimal? paymentValue { get; set;}

        public decimal? paymentValueAfterBooking { get; set;}

        public int? numberOfDaysOfJustifiedDelay { get;set; }

        public decimal? paymentValueAfterDisbandingUpon { get; set; }

        public int? status { get; set; }
    } 
}
