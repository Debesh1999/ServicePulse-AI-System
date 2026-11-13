using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ServicePulse.Application.DTOs;
using ServicePulse.Domain.Entities;
using ServicePulse.Infrastructure.Persistence;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ServicePulse.Api.Services;

public class AuthService
{
    private readonly ServicePulseDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(ServicePulseDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<TokenResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _context.ApplicationUsers.AnyAsync(u => u.Email == request.Email))
            throw new InvalidOperationException("Email already registered.");

        if (!IsValidRole(request.Role))
            throw new InvalidOperationException("Invalid role. Must be Admin, Manager, or Agent.");

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = request.Email.ToLowerInvariant().Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            DisplayName = request.DisplayName.Trim(),
            Role = request.Role.Trim()
        };

        _context.ApplicationUsers.Add(user);
        await _context.SaveChangesAsync();

        return new TokenResponse(GenerateJwtToken(user), "Bearer", 3600, new UserDto(user.Id, user.Email, user.DisplayName, user.Role));
    }

    public async Task<TokenResponse> LoginAsync(LoginRequest request)
    {
        var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == request.Email.ToLowerInvariant().Trim());
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new InvalidOperationException("Invalid email or password.");

        if (!user.IsActive)
            throw new InvalidOperationException("Account is deactivated.");

        return new TokenResponse(GenerateJwtToken(user), "Bearer", 3600, new UserDto(user.Id, user.Email, user.DisplayName, user.Role));
    }

    public Application.DTOs.UserDto GetCurrentUser(Guid userId)
    {
        var user = _context.ApplicationUsers.Find(userId)
            ?? throw new InvalidOperationException("User not found.");
        return new Application.DTOs.UserDto(user.Id, user.Email, user.DisplayName, user.Role);
    }

    public static void SeedAdminIfNeeded(ServicePulseDbContext context)
    {
        if (context.ApplicationUsers.Any()) return;

        context.ApplicationUsers.Add(new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = "admin@servicepulse.local",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            DisplayName = "System Admin",
            Role = "Admin",
            IsActive = true
        });
        context.SaveChanges();
    }

    private static string GenerateJwtToken(ApplicationUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetJwtKey()));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.DisplayName),
            new Claim("role", user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: GetIssuer(),
            audience: GetAudience(),
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds,
            claims: claims
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GetJwtKey() =>
        Environment.GetEnvironmentVariable("JWT_SECRET") ?? "SuperSecretKeyValue_ForDevOnly_ChangeInProd_2026!";

    private static string GetIssuer() => "ServicePulse.Api";
    private static string GetAudience() => "ServicePulse.Client";

    private static bool IsValidRole(string role) =>
        role is "Admin" or "Manager" or "Agent";
}

