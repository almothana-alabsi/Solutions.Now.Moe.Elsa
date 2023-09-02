using Microsoft.EntityFrameworkCore;
using Solutions.Now.Moe.Elsa.Activities;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Construction");
        }


    }
}
