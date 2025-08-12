using System.ComponentModel.DataAnnotations;

namespace TicketMaster.Models
{
    public class InvitationWithPoint
    {
        public int Id { get; set; }

        // Unique generated code for this invitation
        public string Issued { get; set; }

        // Uploaded image file path (relative URL, e.g. "/uploads/abc123.png")
        public string ImagePath { get; set; }

        // Coordinates selected on the image
        public int CoordinateX { get; set; }
        public int CoordinateY { get; set; }

        // Optional: Invitation Type or prefix (if needed)
        public string InvitationType { get; set; }
    }
}
