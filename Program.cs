using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CultureWeb.Data;
using CultureWeb.Models;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Globalization;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity.UI.Services;
using CultureWeb.Models;
using CultureWeb.Services;
using CultureWeb.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Configure application services and settings
builder.Services.AddRazorPages();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

// Configure Identity with required email confirmation
builder.Services.Configure<IdentityOptions>(opts => opts.SignIn.RequireConfirmedEmail = true);

// Configure email services
var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);
builder.Services.AddScoped<IEmailService, EmailService>();

// Configure localization
builder.Services.AddLocalization(options => { options.ResourcesPath = "Resources"; });
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en"),
        new CultureInfo("km"),
    };

    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

// Configure session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(30);
    options.Cookie.IsEssential = true;
});

// Configure routing
builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true; // Optional: To make the URLs lowercase
});

// Configure database
var connectionString = builder.Configuration.GetConnectionString("Culturedb") ?? throw new InvalidOperationException("Connection string 'Culturedb' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySQL(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configure Identity
builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

// Configure request localization based on previously set options
var requestLocalizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(requestLocalizationOptions);

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Seed data
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;

    // Seed admin user and other data
    UserRoleInitailizer.InitailizeAsync(serviceProvider).GetAwaiter().GetResult();
    MainCategorySeeder.Seed(serviceProvider);

    // Additional data seeding or management as needed
    DataHelper.ManageDataAsync(serviceProvider).GetAwaiter().GetResult();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseSession();
app.MapRazorPages();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area=Customer}/{Controller=Home}/{action=Index}/{id?}"
);

app.Run();
