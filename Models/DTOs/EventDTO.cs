namespace TicketMaster.Models.DTOs
{
    public class EventDTO
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public string? Description { get; set; }
        public int MaxTickets { get; set; }
        public int CreatedTicketsCount { get; set; }
        public string? ImagePath { get; set; }
        public bool IsActive { get; set; }

        // Calculated properties
        public int RemainingTickets => MaxTickets - CreatedTicketsCount;
        public bool CanCreateMore => RemainingTickets > 0;
        public double ProgressPercentage => MaxTickets > 0 ? (CreatedTicketsCount * 100.0 / MaxTickets) : 0;
    }
}
