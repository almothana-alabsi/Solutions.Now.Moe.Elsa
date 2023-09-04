using System;
using System.ComponentModel.DataAnnotations;


namespace Solutions.Now.Moe.Elsa.Models.Construction
{
    public class Construction_detailsOfTakeOverCommittee
    {
        [Key]
        public int? serial { get; set; }

        public int? projectSerial { get; set; }
        public int? tenderSerial { get; set; }
        public int? takeOverSerial  { get; set; } 
        public int? takeOverType  { get; set; } 
        public string? takeOverAttachment  { get; set; }
        public DateTime? actualOccupancyDetectionDate  { get; set; }
        public int? shortcomingsAndWorksThatMustBeCompleted  { get; set; }
        public int? typeOfShortcomings  { get; set; }
        public decimal? rebatesValue  { get; set; }
        public decimal? reservationsValue  { get; set; }
        public string? Note  { get; set; }
        public DateTime? submissionDate  { get; set; }
        public int? durationForMissingDataContractor  { get; set; }
        public string? businessReceiptCertificate  { get; set; }
        public DateTime? dateCompletionRemainingWorkAndReleaseOfReservations  { get; set; }
        public DateTime? dateOfReceiptAndStartOfBusiness  { get; set; }
        public DateTime? contractorResponsibilityEndDate  { get; set; }
        public DateTime? constructionWarrantyStartDateTenYears  { get; set; }
        public DateTime? constructionWarrantyExpirationDate  { get; set; }
       public string?letterCompletingShortcomingsFromContractor  { get; set; }
       public int? typerequest  { get; set; }
    }
}
