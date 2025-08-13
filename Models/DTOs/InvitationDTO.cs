namespace TicketMaster.Models.DTOs
{
    public class InvitationDTO
    {
        public int Id { get; set; }
        public string InviterName { get; set; }
        public string InvitationType { get; set; }
        public string Issued { get; set; }
        public string UniqCode { get; set; }

        public string UserCategory { get; set; } = "Guest"; // Default value
        public List<ImagesDTO> Images { get; set; }
    }
}
