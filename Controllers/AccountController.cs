using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TicketMaster.Data;
using TicketMaster.Models.DTOs;

public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly ApplicationDbContext _context;

    public AccountController(ILogger<AccountController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    public IActionResult Login(string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string email, string password, string returnUrl = null)
    {
        _logger.LogInformation("Login POST called - Email: {Email}, ReturnUrl: {ReturnUrl}", email ?? "null", returnUrl ?? "null");

        ViewData["ReturnUrl"] = returnUrl;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            _logger.LogWarning("Login attempt with empty email or password");
            ModelState.AddModelError("", "Email and password are required.");
            return View();
        }

        // Validate user against database
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            _logger.LogWarning("Login attempt for non-existent user: {Email}", email);
            ModelState.AddModelError("", "Invalid email or password.");
            return View();
        }

        if (!user.IsActive)
        {
            _logger.LogWarning("Login attempt for inactive user: {Email}", email);
            ModelState.AddModelError("", "Your account has been deactivated. Please contact administrator.");
            return View();
        }

        // Verify password using BCrypt
        if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            _logger.LogWarning("Invalid password attempt for user: {Email}", email);
            ModelState.AddModelError("", "Invalid email or password.");
            return View();
        }

        _logger.LogInformation("User validated successfully: {Email}", email);

        // Create claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("FullName", user.FullName),
            new Claim("IsAdmin", user.IsAdmin.ToString())
        };

        // Add role claims
        foreach (var userRole in user.UserRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
        }

        // Get user permissions from roles
        var permissions = await _context.UserRoles
            .Where(ur => ur.UserId == user.Id)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Code)
            .Distinct()
            .ToListAsync();

        foreach (var permission in permissions)
        {
            claims.Add(new Claim("Permission", permission));
        }

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        // Update last login time
        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("User signed in successfully: {Email}", email);

        if (Url.IsLocalUrl(returnUrl))
        {
            _logger.LogInformation("Redirecting to return URL: {ReturnUrl}", returnUrl);
            return Redirect(returnUrl);
        }

        _logger.LogInformation("Redirecting to Home/Index");
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return RedirectToAction("Login");
        }

        // Get all roles for the user
        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

        // Get all unique permissions from all roles
        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => new { rp.Permission.Code, rp.Permission.Name })
            .Distinct()
            .OrderBy(p => p.Name)
            .ToList();

        var userProfile = new UserDTO
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName ?? "",
            LastName = user.LastName ?? "",
            FullName = user.FullName,
            IsActive = user.IsActive,
            IsAdmin = user.IsAdmin,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            Roles = roles,
            Permissions = permissions.Select(p => $"{p.Name} ({p.Code})").ToList()
        };

        return View(userProfile);
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
