namespace TicketMaster.Models
{
    public class Invitation
    {
        public int Id { get; set; }
        public string InviterName { get; set; } 
        public string InvitationType { get; set; }
        public string Issued { get; set; }
        public string UniqCode { get; set; }
        public string ImagePath { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string UserCategory { get; set; } = "Guest"; // Default value
    }
}
