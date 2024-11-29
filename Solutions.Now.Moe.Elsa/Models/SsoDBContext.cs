
using Microsoft.EntityFrameworkCore;

namespace Solutions.Now.Moe.Elsa.Models
{
    public class SsoDBContext : DbContext
    {
        public SsoDBContext(DbContextOptions<SsoDBContext> options) : base(options)
        {

        }
        public DbSet<TblUsers> TblUsers { get; set; }
        public DbSet<MasterData> MasterData { get; set; }
        public DbSet<SMSAndEmail_Audit> SMSAndEmail_Audit { get; set; }
  
    }
}
