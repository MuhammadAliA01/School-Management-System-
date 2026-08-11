using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using SchoolManagmentSystem.Models;
using SchoolManagmentSystem.Services;
using SchoolManagmentSystem.ServicesAi.EmploymentAI;
using SchoolManagmentSystem.ServicesAi.IncidentAI;
using Stripe;
using System.Security.Cryptography;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

/*var firebaseCredentials =
    Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS");

if (string.IsNullOrWhiteSpace(firebaseCredentials))
{
    throw new Exception("FIREBASE_CREDENTIALS environment variable is missing.");
}*/



/*FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromJson(firebaseCredentials)
});*/

FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile("hla-notifs-firebase-adminsdk-fbsvc-82fbd7094d.json")
});

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<FirebaseNotificationService>();


// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IApplicationService, ApplicationService>();

builder.Services.AddSession();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddScoped<AIScoringService>();
builder.Services.AddScoped<IncidentAnalysisService>();
builder.Services.AddScoped<EmploymentService>();
builder.Services.AddScoped<CVService>();
builder.Services.AddScoped<AttendanceReportService>();
builder.Services.AddHostedService<WeeklyReportWorker>();

StripeConfiguration.ApiKey =
    builder.Configuration["Stripe:SecretKey"];



builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("Flutter", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});



builder.WebHost.UseUrls(
    "http://0.0.0.0:7205",
    "https://0.0.0.0:7206"
);

var app = builder.Build();
var supportedCultures = new[] { new System.Globalization.CultureInfo("en-US") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-US"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

app.UseCors("Flutter");



//set pdf font resolver
PdfSharp.Fonts.GlobalFontSettings.FontResolver = new MyFontResolver();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=HomePage}/{id?}");

app.Run();
