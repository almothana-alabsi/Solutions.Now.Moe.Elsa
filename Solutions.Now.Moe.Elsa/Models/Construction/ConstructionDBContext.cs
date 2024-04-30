using Microsoft.EntityFrameworkCore;
using Solutions.Now.Moe.Elsa.Activities;
using Solutions.Now.Moe.Elsa.Activities.Construction;
using Solutions.Now.Moe.Elsa.Models.Construction.DTOs;

namespace Solutions.Now.Moe.Elsa.Models.Construction
{
    public class ConstructionDBContext : DbContext
    {
        public ConstructionDBContext(DbContextOptions<ConstructionDBContext> options) : base(options)
        {

        }
        public DbSet<WorkFlowRulesConstruction> WorkFlowRules { get; set; }
        public DbSet<Construction_CommitteeMember> CommitteeMember { get; set; }
        public DbSet<Construction_delegateDirectorEducationWithEngineerPermissions> DelegateDirectorEducationWithEngineerPermissions { get; set; }
        public DbSet<Tender> Tender { get; set; }
        public DbSet<Construction_CommunicationEngineer> CommunicationEngineers { get; set; }
        public DbSet<AdoptionMandatory> AdoptionMaterials { get; set; }
        public DbSet<Construction_SupervisionCommittee> SupervisionCommittee { get; set; }
        public DbSet<Construction_SiteHandOver> SiteHandOver { get; set; }
        public DbSet<Construction_DirectOrderToTheContractor> DirectOrderToTheContractor { get; set; }
        public DbSet<Construction_RaiseSurveyors> RaiseSurveyors { get; set; }
        public DbSet<Construction_InsurancePolicy> InsurancePolicy { get; set; }
        public DbSet<Construction_Staff> Staff { get; set; }
        public DbSet<Construction_ChangeOrder> ChangeOrder{get;set;} 
        public DbSet<Construction_TenderAdvancePaymentRequest> TenderAdvancePaymentRequest { get; set; }
        public DbSet<Construction_ApprovalOfDesignMixtures> ApprovalOfDesignMixtures { get; set; }
        public DbSet<Construction_LabApproval> LabApproval { get; set; }
        public DbSet<Construction_officialBooks> officialBooks { get; set; }
        public DbSet<Construction_ProceduresForSubmittingSiteMemorandum> ProceduresForSubmittingSiteMemorandum { get; set; }
        public DbSet<Construction_InitialReceiptDB> TakeOver { get; set; }

        public DbSet<Construction_tenderCancellationProcedures> tenderCancellationProcedures { get; set; }
        public DbSet<Construction_tenderProjectResume> tenderProjectResume { get; set; }
        public DbSet<Construction_tenderProjectStop> tenderProjectStop { get; set; }
        public DbSet<Construction_detailsOfTakeOverCommittee> detailsOfTakeOverCommittee { get; set; }
        public DbSet<Construction_DailyWorkProgressReport> DailyWorkProgressReport { get; set; }
        public DbSet<Construction_partialTakeOver> partialTakeOver { get; set; }
        public DbSet<Construction_Non_complianceWithActionsCorrectiveActionsDB> MatchingCorrectiveAction { get; set; }
        public DbSet<Construction_finalDeliveryDB> FinalReceiptWork { get; set; }
        public DbSet<Construction_detailsOfFinalReceiptCommittee> detailsOfFinalReceiptCommittee { get; set; }
        public DbSet<Construction_detailsOfTakeOverCommitteeofshortcomming> detailsOfTakeOverCommitteeofshortcomming { get; set; }  
        public DbSet<Construction_SubmissionApprovalQualityControlProjectSamples> SubmissionApprovalQualityControlProjectSamples { get; set; }
        public DbSet<InvoicesPayment> InvoicesPayment { get; set; }
        public DbSet<Construction_MonthlyReport> WorkScheduleMonthlyReport { get; set; }
        public DbSet<Construction_releasereservations> Releasereservations { get; set; }
        public DbSet<Construction_WorkScheduleDB> WorkSchedule { get; set; }
        public DbSet<Construction_WorkScheduleModifyDB> WorkScheduleModify { get; set; }

        public DbSet<Construction_Timedemands> Timedemands { get; set; }    
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Construction");
        }


    }
}
