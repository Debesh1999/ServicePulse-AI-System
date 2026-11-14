using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Hangfire;
using Hangfire.SqlServer;
using ServicePulse.Api.Middleware;
using ServicePulse.Api.Services;
using ServicePulse.Application.Services;
using ServicePulse.Infrastructure.AiAnalysis;
using ServicePulse.Infrastructure.Persistence;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<ServicePulseDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Hangfire
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();

// Auth
builder.Services.AddScoped<AuthService>();
builder.Services.AddHttpClient<IAiAnalyzerService, PythonAiClient>(c =>
    c.BaseAddress = new Uri(builder.Configuration["AiService:BaseUrl"] ?? "http://localhost:8000"));
var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "SuperSecretKeyValue_ForDevOnly_ChangeInProd_2026!";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "ServicePulse.Api",
            ValidAudience = "ServicePulse.Client",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ServicePulseDbContext>();
    await context.Database.MigrateAsync();
    await DataSeeder.SeedAsync(context);
    AuthService.SeedAdminIfNeeded(context);
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new Hangfire.Dashboard.LocalRequestsOnlyAuthorizationFilter() }
});

// Set service locator for Hangfire job activator
ServicePulse.Infrastructure.BackgroundJobs.ServiceLocator.Instance = app.Services;
app.MapControllers();

app.Run();
