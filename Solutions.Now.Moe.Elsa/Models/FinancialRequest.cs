using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Solutions.Now.Moe.Elsa.Models
{
    public class FinancialRequest
    {
        [Key]
        public int? serial { get; set; }
        public int? projectSerial { get; set; }
        public int? tenderSerial { get; set; }
        public string? PaymentNumber { get; set; }
        public int? phase { get; set; }
        public int? paymentValue { get; set; }
        public int? minorExpenses { get; set; }
        public decimal? totalValue { get; set; }
        public int? previousPayment { get; set; }
        public string? detailedFinancialClaimSchedule { get; set; }
        public string? approvalLetterForStage { get; set; }
        public string? referralDecision { get; set; }
        public string? contractAppendixNo4 { get; set; }
        public char? contractAppendixNo4b { get; set; }
        public string? otherDocuments { get; set; }
        public string? note { get; set; }
        public int? status { get; set; }
        public DateTime? submissionDate { get; set; }
        public int? step { get; set; }
        public string? assignedAccountant { get; set; }

    }
}
