using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TicketMaster.Authorization;
using TicketMaster.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();

// SQLite connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=invitations.db"));


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";      // Redirect here if not logged in
        options.LogoutPath = "/Account/Logout";    // Logout URL
        options.AccessDeniedPath = "/Account/AccessDenied";  // Forbidden page
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

// Register authorization handler
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

// Add authorization with policies
builder.Services.AddAuthorization(options =>
{
    // Define permission-based policies
    options.AddPolicy("tickets.manage", policy => policy.Requirements.Add(new PermissionRequirement("tickets.manage")));
    options.AddPolicy("tickets.view", policy => policy.Requirements.Add(new PermissionRequirement("tickets.view")));
    options.AddPolicy("events.manage", policy => policy.Requirements.Add(new PermissionRequirement("events.manage")));
    options.AddPolicy("events.view", policy => policy.Requirements.Add(new PermissionRequirement("events.view")));
    options.AddPolicy("users.manage", policy => policy.Requirements.Add(new PermissionRequirement("users.manage")));
    options.AddPolicy("designs.manage", policy => policy.Requirements.Add(new PermissionRequirement("designs.manage")));
    options.AddPolicy("issuers.manage", policy => policy.Requirements.Add(new PermissionRequirement("issuers.manage")));
    options.AddPolicy("reports.view", policy => policy.Requirements.Add(new PermissionRequirement("reports.view")));
    options.AddPolicy("settings.manage", policy => policy.Requirements.Add(new PermissionRequirement("settings.manage")));
    options.AddPolicy("invitations.verify", policy => policy.Requirements.Add(new PermissionRequirement("invitations.verify")));
});

var app = builder.Build();

// Initialize database with migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        // Apply any pending migrations
        logger.LogInformation("Applying database migrations...");
        context.Database.Migrate();
        logger.LogInformation("Database migrations applied successfully");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
else
{
    app.UseDeveloperExceptionPage();
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();  // CRITICAL: Must come before UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();