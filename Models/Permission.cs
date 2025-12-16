namespace TicketMaster.Models
{
    public class Permission
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty; // e.g., "tickets.create", "users.manage"
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // e.g., "Tickets", "Users", "Events"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
