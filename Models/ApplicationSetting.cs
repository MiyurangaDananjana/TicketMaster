namespace TicketMaster.Models
{
    public class ApplicationSetting
    {
        public int Id { get; set; }
        public required string ColorCode { get; set; }
        public required string FrontSize { get; set; }
        public required string FontFamily { get; set; }

    }
}
