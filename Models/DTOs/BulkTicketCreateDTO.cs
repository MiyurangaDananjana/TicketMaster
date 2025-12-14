namespace TicketMaster.Models.DTOs
{
    public class BulkTicketCreateDTO
    {
        public int EventId { get; set; }
        public int Quantity { get; set; }
        public string InviterName { get; set; }
        public string InvitationType { get; set; }
        public string Issued { get; set; }
    }
}
