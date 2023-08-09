
using Microsoft.EntityFrameworkCore;

namespace Solutions.Now.Moe.Elsa.Models
{
    public class DesignReviewDBContext : DbContext
    {
        public DesignReviewDBContext(DbContextOptions<DesignReviewDBContext> options) : base(options)
        {

        }
        public DbSet<InfoProject> InfoProject { get; set; }

    }
}
