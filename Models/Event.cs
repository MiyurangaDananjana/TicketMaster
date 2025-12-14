using System.ComponentModel.DataAnnotations;

namespace TicketMaster.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Event name is required")]
        [StringLength(200, ErrorMessage = "Event name cannot exceed 200 characters")]
        public string EventName { get; set; }

        [Required(ErrorMessage = "Event date is required")]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Maximum tickets is required")]
        [Range(1, 10000, ErrorMessage = "Maximum tickets must be between 1 and 10,000")]
        public int MaxTickets { get; set; }

        public string? ImagePath { get; set; }
        public int CreatedTicketsCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Navigation property
        public ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();
    }
}
