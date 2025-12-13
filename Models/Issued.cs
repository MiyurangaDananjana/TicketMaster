using System.ComponentModel.DataAnnotations;
using TicketMaster.Models;

namespace TicketMaster.Models
{
    public class Issued
    {
        [Key] // Primary Key
        public string UserCode { get; set; }

        public required string Name { get; set; }

        public string Status { get; set; } = "True";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();
    }

}