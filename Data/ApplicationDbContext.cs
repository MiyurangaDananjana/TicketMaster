using Microsoft.EntityFrameworkCore;
using TicketMaster.Models;

namespace TicketMaster.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
      : base(options)
        {
        }

        public DbSet<Invitation> Invitations { get; set; }

        public DbSet<InvitationWithPoint> InvitationsWithPoint { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<ApplicationSetting> ApplicationSettings { get; set; }

        public DbSet<Issued> Issueds { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Issued PK
            modelBuilder.Entity<Issued>()
                .HasKey(i => i.UserCode);

            // Invitation → Issued
            modelBuilder.Entity<Invitation>()
                .HasOne(i => i.IssuedUser)
                .WithMany(u => u.Invitations)
                .HasForeignKey(i => i.Issued)            // FK in Invitation
                .HasPrincipalKey(u => u.UserCode)        // PK in Issued
                .OnDelete(DeleteBehavior.Cascade);       // Cascade delete

        }


    }

}
