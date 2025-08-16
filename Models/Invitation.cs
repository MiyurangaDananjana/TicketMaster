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

        // Navigation property
        public Issued IssuedUser { get; set; }

    }
}