namespace TicketMaster.Models
{
    public class Invitation
    {
        public int Id { get; set; }
        public string InviterName { get; set; }
        public string InvitationType { get; set; }

        // Foreign Key → Issued.UserCode
        public string Issued { get; set; }

        public string UniqCode { get; set; }
        public string ImagePath { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        // Foreign Key → Event.Id (nullable for backward compatibility)
        public int? EventId { get; set; }

        // Navigation properties
        public Issued IssuedUser { get; set; }
        public Event? Event { get; set; }

    }
}