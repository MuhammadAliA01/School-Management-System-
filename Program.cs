using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using SchoolManagmentSystem.Models;
using SchoolManagmentSystem.ServicesAi.EmploymentAI;
using SchoolManagmentSystem.ServicesAi.IncidentAI;
using Stripe;
using SchoolManagmentSystem.Services;

var builder = WebApplication.CreateBuilder(args);



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

var app = builder.Build();

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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=HomePage}/{id?}");

app.Run();
