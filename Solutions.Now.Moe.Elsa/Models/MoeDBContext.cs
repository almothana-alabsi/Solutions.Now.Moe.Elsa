
using Microsoft.EntityFrameworkCore;
using Solutions.Now.Moe.Elsa.Activities;
using Solutions.Now.Moe.Elsa.Models.Construction;

namespace Solutions.Now.Moe.Elsa.Models
{
    public class MoeDBContext : DbContext
    {
        public MoeDBContext(DbContextOptions<MoeDBContext> options) : base(options)
        {

        }

        public DbSet<WorkFlowRules> WorkFlowRules { get; set; }
        public DbSet<ApprovalHistory> ApprovalHistory { get; set; }
        public DbSet<Committee> Committee { get; set; }
        public DbSet<CommitteeMember> CommitteeMember { get; set; }
        public DbSet<ProjectCancellation> ProjectCancellation { get; set; }
        public DbSet<ReferedTender> ReferedTender { get; set; }
        public DbSet<VisitMember> visitMember { get; set; }
        public DbSet<SiteVisit> SiteVisit { get; set; }
        public DbSet<ProjectsTender> ProjectsTender {get; set;}
        public DbSet<projectStoppedTable> ProjectStoppedTable { get; set; }
        public DbSet<ResumeProjectWork> ResumeProjectWork { get; set; }
        public DbSet<FinancialRequest> FinancialRequest { get; set; }
        public DbSet<Construction_Timedemands> Timedemands { get; set; }

        public DbSet<Invoice> Invoice { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.HasDefaultSchema("DesignReview");
            }

    }
}
