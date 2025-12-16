using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketMaster.Data;
using TicketMaster.Models;
using TicketMaster.Models.DTOs;

namespace TicketMaster.Controllers
{
    [Authorize(Policy = "users.manage")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ApplicationDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Admin/Index (Dashboard)
        public async Task<IActionResult> Index()
        {
            var model = new
            {
                TotalUsers = await _context.Users.CountAsync(),
                ActiveUsers = await _context.Users.CountAsync(u => u.IsActive),
                InactiveUsers = await _context.Users.CountAsync(u => !u.IsActive),
                AdminUsers = await _context.Users.CountAsync(u => u.IsAdmin),
                TotalRoles = await _context.Roles.CountAsync(),
                TotalPermissions = await _context.Permissions.CountAsync(),
                TotalEvents = await _context.Events.CountAsync(),
                ActiveEvents = await _context.Events.CountAsync(e => e.EventDate >= DateTime.UtcNow),
                TotalInvitations = await _context.Invitations.CountAsync(),
                VerifiedInvitations = await _context.Invitations.CountAsync(i => i.IsVerified),
                UnverifiedInvitations = await _context.Invitations.CountAsync(i => !i.IsVerified),
                RecentUsers = await _context.Users
                    .OrderByDescending(u => u.CreatedAt)
                    .Take(5)
                    .Select(u => new { u.Email, u.FirstName, u.LastName, u.CreatedAt })
                    .ToListAsync()
            };

            return View(model);
        }

        // GET: Admin/ManageUsers
        public async Task<IActionResult> ManageUsers()
        {
            var users = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .OrderByDescending(u => u.CreatedAt)
                .Select(u => new UserDTO
                {
                    Id = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    IsActive = u.IsActive,
                    IsAdmin = u.IsAdmin,
                    CreatedAt = u.CreatedAt,
                    LastLoginAt = u.LastLoginAt,
                    Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
                })
                .ToListAsync();

            return View(users);
        }

        // GET: Admin/CreateUser
        public async Task<IActionResult> CreateUser()
        {
            ViewBag.Roles = await _context.Roles.ToListAsync();
            return View();
        }

        // POST: Admin/CreateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserDTO model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = await _context.Roles.ToListAsync();
                return View(model);
            }

            try
            {
                // Check if email already exists
                if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email already exists.");
                    ViewBag.Roles = await _context.Roles.ToListAsync();
                    return View(model);
                }

                // Create new user
                var user = new User
                {
                    Email = model.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    IsActive = true,
                    IsAdmin = model.IsAdmin,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Assign roles
                if (model.RoleIds != null && model.RoleIds.Any())
                {
                    foreach (var roleId in model.RoleIds)
                    {
                        _context.UserRoles.Add(new UserRole
                        {
                            UserId = user.Id,
                            RoleId = roleId,
                            AssignedAt = DateTime.UtcNow
                        });
                    }
                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = $"User '{user.Email}' created successfully!";
                return RedirectToAction("ManageUsers");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                TempData["ErrorMessage"] = "An error occurred while creating the user.";
                ViewBag.Roles = await _context.Roles.ToListAsync();
                return View(model);
            }
        }

        // GET: Admin/EditUser/5
        public async Task<IActionResult> EditUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("ManageUsers");
            }

            var model = new UpdateUserDTO
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsActive = user.IsActive,
                IsAdmin = user.IsAdmin,
                RoleIds = user.UserRoles.Select(ur => ur.RoleId).ToList()
            };

            ViewBag.Roles = await _context.Roles.ToListAsync();
            return View(model);
        }

        // POST: Admin/EditUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(UpdateUserDTO model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = await _context.Roles.ToListAsync();
                return View(model);
            }

            try
            {
                var user = await _context.Users
                    .Include(u => u.UserRoles)
                    .FirstOrDefaultAsync(u => u.Id == model.Id);

                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToAction("ManageUsers");
                }

                // Check if email is taken by another user
                if (await _context.Users.AnyAsync(u => u.Email == model.Email && u.Id != model.Id))
                {
                    ModelState.AddModelError("Email", "Email already exists.");
                    ViewBag.Roles = await _context.Roles.ToListAsync();
                    return View(model);
                }

                // Update user
                user.Email = model.Email;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.IsActive = model.IsActive;
                user.IsAdmin = model.IsAdmin;

                // Update roles
                _context.UserRoles.RemoveRange(user.UserRoles);
                if (model.RoleIds != null && model.RoleIds.Any())
                {
                    foreach (var roleId in model.RoleIds)
                    {
                        _context.UserRoles.Add(new UserRole
                        {
                            UserId = user.Id,
                            RoleId = roleId,
                            AssignedAt = DateTime.UtcNow
                        });
                    }
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"User '{user.Email}' updated successfully!";
                return RedirectToAction("ManageUsers");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                TempData["ErrorMessage"] = "An error occurred while updating the user.";
                ViewBag.Roles = await _context.Roles.ToListAsync();
                return View(model);
            }
        }

        // POST: Admin/ToggleUserStatus
        [HttpPost]
        public async Task<IActionResult> ToggleUserStatus(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToAction("ManageUsers");
                }

                user.IsActive = !user.IsActive;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"User '{user.Email}' is now {(user.IsActive ? "Active" : "Inactive")}.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling user status");
                TempData["ErrorMessage"] = "An error occurred while updating user status.";
            }

            return RedirectToAction("ManageUsers");
        }

        // POST: Admin/DeleteUser
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToAction("ManageUsers");
                }

                // Prevent deleting the last admin user
                if (user.IsAdmin)
                {
                    var adminCount = await _context.Users.CountAsync(u => u.IsAdmin && u.IsActive);
                    if (adminCount <= 1)
                    {
                        TempData["ErrorMessage"] = "Cannot delete the last admin user.";
                        return RedirectToAction("ManageUsers");
                    }
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"User '{user.Email}' deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                TempData["ErrorMessage"] = "An error occurred while deleting the user.";
            }

            return RedirectToAction("ManageUsers");
        }

        // POST: Admin/ResetPassword
        [HttpPost]
        public async Task<IActionResult> ResetPassword(int userId, string newPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(newPassword))
                {
                    TempData["ErrorMessage"] = "Password cannot be empty.";
                    return RedirectToAction("ManageUsers");
                }

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToAction("ManageUsers");
                }

                user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Password for '{user.Email}' reset successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password");
                TempData["ErrorMessage"] = "An error occurred while resetting password.";
            }

            return RedirectToAction("ManageUsers");
        }

        // GET: Admin/ManageRoles
        public async Task<IActionResult> ManageRoles()
        {
            var roles = await _context.Roles
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .OrderBy(r => r.Name)
                .ToListAsync();

            ViewBag.AllPermissions = await _context.Permissions
                .OrderBy(p => p.Category)
                .ThenBy(p => p.Name)
                .ToListAsync();

            return View(roles);
        }

        // POST: Admin/UpdateRolePermissions
        [HttpPost]
        public async Task<IActionResult> UpdateRolePermissions(int roleId, List<int> permissionIds)
        {
            try
            {
                var role = await _context.Roles
                    .Include(r => r.RolePermissions)
                    .FirstOrDefaultAsync(r => r.Id == roleId);

                if (role == null)
                {
                    TempData["ErrorMessage"] = "Role not found.";
                    return RedirectToAction("ManageRoles");
                }

                // Remove existing permissions
                _context.RolePermissions.RemoveRange(role.RolePermissions);

                // Add new permissions
                if (permissionIds != null && permissionIds.Any())
                {
                    foreach (var permissionId in permissionIds)
                    {
                        _context.RolePermissions.Add(new RolePermission
                        {
                            RoleId = roleId,
                            PermissionId = permissionId,
                            AssignedAt = DateTime.UtcNow
                        });
                    }
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Permissions for role '{role.Name}' updated successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role permissions");
                TempData["ErrorMessage"] = "An error occurred while updating permissions.";
            }

            return RedirectToAction("ManageRoles");
        }
    }
}
