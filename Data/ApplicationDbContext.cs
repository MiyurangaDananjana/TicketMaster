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
    }
}
