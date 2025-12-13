using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;

    public AccountController(ILogger<AccountController> logger)
    {
        _logger = logger;
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

        // TODO: Replace with your user validation logic
        if (IsValidUser(email, password))
        {
            _logger.LogInformation("User validated successfully: {Email}", email);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.Email, email),
                // Add more claims if needed
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,  // Or false based on RememberMe checkbox
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            _logger.LogInformation("User signed in successfully: {Email}", email);

            if (Url.IsLocalUrl(returnUrl))
            {
                _logger.LogInformation("Redirecting to return URL: {ReturnUrl}", returnUrl);
                return Redirect(returnUrl);
            }

            _logger.LogInformation("Redirecting to Home/Index");
            return RedirectToAction("Index", "Home");
        }

        _logger.LogWarning("Invalid login attempt for email: {Email}", email);
        ModelState.AddModelError("", "Invalid email or password.");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }

    private bool IsValidUser(string email, string password)
    {
        // TODO: Implement your user validation (DB check, etc.)
        return email == "admin@gmail.com" && password == "admin123";
    }
}
