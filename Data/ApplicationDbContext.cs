using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using TicketMaster.Models;

namespace TicketMaster.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<InvitationWithPoint> InvitationsWithPoint { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ApplicationSetting> ApplicationSettings { get; set; }
        public DbSet<Issued> Issueds { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // PostgreSQL: Use lowercase table names (convention)
            modelBuilder.Entity<Invitation>().ToTable("invitations");
            modelBuilder.Entity<InvitationWithPoint>().ToTable("invitations_with_point");
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<ApplicationSetting>().ToTable("application_settings");
            modelBuilder.Entity<Issued>().ToTable("issueds");
            modelBuilder.Entity<Event>().ToTable("events");
            modelBuilder.Entity<Role>().ToTable("roles");
            modelBuilder.Entity<Permission>().ToTable("permissions");
            modelBuilder.Entity<UserRole>().ToTable("user_roles");
            modelBuilder.Entity<RolePermission>().ToTable("role_permissions");

            // Issued PK
            modelBuilder.Entity<Issued>()
                .HasKey(i => i.UserCode);

            // Invitation → Issued
            modelBuilder.Entity<Invitation>()
                .HasOne(i => i.IssuedUser)
                .WithMany(u => u.Invitations)
                .HasForeignKey(i => i.Issued)
                .HasPrincipalKey(u => u.UserCode)
                .OnDelete(DeleteBehavior.Cascade);

            // Event → Invitation (one-to-many)
            modelBuilder.Entity<Invitation>()
                .HasOne(i => i.Event)
                .WithMany(e => e.Invitations)
                .HasForeignKey(i => i.EventId)
                .OnDelete(DeleteBehavior.SetNull);

            // User → UserRole (one-to-many)
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Role → UserRole (one-to-many)
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Role → RolePermission (one-to-many)
            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Permission → RolePermission (one-to-many)
            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraints
            modelBuilder.Entity<Permission>()
                .HasIndex(p => p.Code)
                .IsUnique();

            modelBuilder.Entity<Role>()
                .HasIndex(r => r.Name)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // PostgreSQL-specific configurations for decimal/numeric columns
            // Add these based on your entity models if you have decimal properties
            // Example:
            // modelBuilder.Entity<Invitation>()
            //     .Property(i => i.Price)
            //     .HasColumnType("decimal(18,2)");

            // Seed default admin user, roles, and permissions
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Use a fixed date for seed data to prevent EF from generating UPDATE statements
            var seedDate = new DateTime(2025, 12, 14, 0, 0, 0, DateTimeKind.Utc);

            // Seed Permissions
            var permissions = new[]
            {
                new Permission { Id = 1, Name = "Manage Tickets", Code = "tickets.manage", Description = "Create, view, edit, and delete tickets", Category = "Tickets", CreatedAt = seedDate },
                new Permission { Id = 2, Name = "View Tickets", Code = "tickets.view", Description = "View tickets only", Category = "Tickets", CreatedAt = seedDate },
                new Permission { Id = 3, Name = "Manage Events", Code = "events.manage", Description = "Create, view, edit, and delete events", Category = "Events", CreatedAt = seedDate },
                new Permission { Id = 4, Name = "View Events", Code = "events.view", Description = "View events only", Category = "Events", CreatedAt = seedDate },
                new Permission { Id = 5, Name = "Manage Users", Code = "users.manage", Description = "Create, view, edit, and delete users", Category = "Users", CreatedAt = seedDate },
                new Permission { Id = 6, Name = "Manage Designs", Code = "designs.manage", Description = "Upload and manage invitation designs", Category = "Designs", CreatedAt = seedDate },
                new Permission { Id = 7, Name = "Manage Issuers", Code = "issuers.manage", Description = "Create and manage issuers", Category = "Issuers", CreatedAt = seedDate },
                new Permission { Id = 8, Name = "View Reports", Code = "reports.view", Description = "View and export reports", Category = "Reports", CreatedAt = seedDate },
                new Permission { Id = 9, Name = "Manage Settings", Code = "settings.manage", Description = "Manage application settings", Category = "Settings", CreatedAt = seedDate },
                new Permission { Id = 10, Name = "Verify Invitations", Code = "invitations.verify", Description = "Search and verify invitation codes", Category = "Tickets", CreatedAt = seedDate }
            };
            modelBuilder.Entity<Permission>().HasData(permissions);

            // Seed Roles
            var adminRole = new Role { Id = 1, Name = "Administrator", Description = "Full system access", CreatedAt = seedDate };
            var managerRole = new Role { Id = 2, Name = "Manager", Description = "Can manage tickets and events", CreatedAt = seedDate };
            var operatorRole = new Role { Id = 3, Name = "Operator", Description = "Can view and create tickets", CreatedAt = seedDate };
            var checkerRole = new Role { Id = 4, Name = "Checker", Description = "Can only search and verify invitation codes", CreatedAt = seedDate };

            modelBuilder.Entity<Role>().HasData(adminRole, managerRole, operatorRole, checkerRole);

            // Seed Admin User (password: admin123)
            var adminUser = new User
            {
                Id = 1,
                Email = "admin@gmail.com",
                Password = BCrypt.Net.BCrypt.HashPassword("admin123"),
                FirstName = "System",
                LastName = "Administrator",
                IsActive = true,
                IsAdmin = true,
                CreatedAt = seedDate
            };
            modelBuilder.Entity<User>().HasData(adminUser);

            // Assign Admin Role to Admin User
            modelBuilder.Entity<UserRole>().HasData(new UserRole { Id = 1, UserId = 1, RoleId = 1, AssignedAt = seedDate });

            // Assign all permissions to Administrator role
            for (int i = 1; i <= permissions.Length; i++)
            {
                modelBuilder.Entity<RolePermission>().HasData(new RolePermission { Id = i, RoleId = 1, PermissionId = i, AssignedAt = seedDate });
            }

            // Assign permissions to Manager role
            modelBuilder.Entity<RolePermission>().HasData(
                new RolePermission { Id = 11, RoleId = 2, PermissionId = 1, AssignedAt = seedDate }, // tickets.manage
                new RolePermission { Id = 12, RoleId = 2, PermissionId = 3, AssignedAt = seedDate }, // events.manage
                new RolePermission { Id = 13, RoleId = 2, PermissionId = 6, AssignedAt = seedDate }, // designs.manage
                new RolePermission { Id = 14, RoleId = 2, PermissionId = 7, AssignedAt = seedDate }, // issuers.manage
                new RolePermission { Id = 15, RoleId = 2, PermissionId = 8, AssignedAt = seedDate }  // reports.view
            );

            // Assign permissions to Operator role
            modelBuilder.Entity<RolePermission>().HasData(
                new RolePermission { Id = 16, RoleId = 3, PermissionId = 2, AssignedAt = seedDate }, // tickets.view
                new RolePermission { Id = 17, RoleId = 3, PermissionId = 4, AssignedAt = seedDate }  // events.view
            );

            // Assign permissions to Checker role - only invitation verification
            modelBuilder.Entity<RolePermission>().HasData(
                new RolePermission { Id = 18, RoleId = 4, PermissionId = 10, AssignedAt = seedDate }  // invitations.verify
            );
        }
    }
}