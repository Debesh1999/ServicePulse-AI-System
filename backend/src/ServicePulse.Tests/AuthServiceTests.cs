using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ServicePulse.Api.Services;
using ServicePulse.Application.DTOs;
using ServicePulse.Domain.Entities;
using ServicePulse.Infrastructure.Persistence;

namespace ServicePulse.Tests;

public class AuthServiceTests
{
    private static ServicePulseDbContext CreateContext()
    {
        var dbName = $"ServicePulseTestAuth_{Guid.NewGuid():N}";
        var options = new DbContextOptionsBuilder<ServicePulseDbContext>()
            .UseSqlServer($"Server=(localdb)\\mssqllocaldb;Database={dbName};Trusted_Connection=true;TrustServerCertificate=true")
            .Options;
        var ctx = new ServicePulseDbContext(options);
        ctx.Database.EnsureCreated();
        return ctx;
    }

    private static AuthService CreateService(ServicePulseDbContext ctx)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        var httpContextAccessor = new HttpContextAccessor();
        return new AuthService(ctx, config, httpContextAccessor);
    }

    [Fact]
    public async Task RegisterAsync_CreatesUserAndReturnsToken()
    {
        var ctx = CreateContext();
        var svc = CreateService(ctx);

        var req = new RegisterRequest("newuser@test.com", "P@ssw0rd!", "New User", "Agent");

        var result = await svc.RegisterAsync(req);

        Assert.NotNull(result.AccessToken);
        Assert.Equal("Bearer", result.TokenType);
        Assert.Equal(3600, result.ExpiresIn);
        Assert.Equal("newuser@test.com", result.User.Email);
        Assert.Equal("New User", result.User.DisplayName);
        Assert.Equal("Agent", result.User.Role);

        var dbUser = await ctx.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == "newuser@test.com");
        Assert.NotNull(dbUser);
        Assert.True(BCrypt.Net.BCrypt.Verify("P@ssw0rd!", dbUser.PasswordHash));
    }

    [Fact]
    public async Task RegisterAsync_Throws_WhenEmailAlreadyExists()
    {
        var ctx = CreateContext();
        var svc = CreateService(ctx);

        await svc.RegisterAsync(new RegisterRequest("dup@test.com", "pwd", "Dup", "Agent"));

        var ex = await Record.ExceptionAsync(() =>
            svc.RegisterAsync(new RegisterRequest("dup@test.com", "pwd2", "Dup2", "Agent")));

        Assert.NotNull(ex);
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Contains("already registered", ex.Message);
    }

    [Fact]
    public async Task RegisterAsync_Throws_WhenRoleInvalid()
    {
        var ctx = CreateContext();
        var svc = CreateService(ctx);

        var ex = await Record.ExceptionAsync(() =>
            svc.RegisterAsync(new RegisterRequest("x@test.com", "pwd", "X", "SuperAdmin")));

        Assert.NotNull(ex);
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Contains("Invalid role", ex.Message);
    }

    [Fact]
    public async Task LoginAsync_ReturnsToken_ForValidCredentials()
    {
        var ctx = CreateContext();
        var svc = CreateService(ctx);

        await svc.RegisterAsync(new RegisterRequest("login@test.com", "Secret123", "Login User", "Manager"));
        var result = await svc.LoginAsync(new LoginRequest("login@test.com", "Secret123"));

        Assert.NotNull(result.AccessToken);
        Assert.Equal("Manager", result.User.Role);
    }

    [Fact]
    public async Task LoginAsync_Throws_WhenPasswordWrong()
    {
        var ctx = CreateContext();
        var svc = CreateService(ctx);

        await svc.RegisterAsync(new RegisterRequest("pw@test.com", "CorrectPass", "Pw User", "Agent"));

        var ex = await Record.ExceptionAsync(() =>
            svc.LoginAsync(new LoginRequest("pw@test.com", "WrongPass")));

        Assert.NotNull(ex);
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Contains("Invalid email or password", ex.Message);
    }

    [Fact]
    public async Task LoginAsync_Throws_WhenAccountInactive()
    {
        var ctx = CreateContext();
        var svc = CreateService(ctx);

        await svc.RegisterAsync(new RegisterRequest("inactive@test.com", "Pwd1", "Inactive", "Agent"));
        var user = await ctx.ApplicationUsers.FirstAsync(u => u.Email == "inactive@test.com");
        user.IsActive = false;
        await ctx.SaveChangesAsync();

        var ex = await Record.ExceptionAsync(() =>
            svc.LoginAsync(new LoginRequest("inactive@test.com", "Pwd1")));

        Assert.NotNull(ex);
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Contains("deactivated", ex.Message);
    }

    [Fact]
    public void SeedAdminIfNeeded_CreatesDefaultAdmin_WhenNoUsersExist()
    {
        var ctx = CreateContext();
        AuthService.SeedAdminIfNeeded(ctx);

        Assert.Single(ctx.ApplicationUsers);
        var admin = ctx.ApplicationUsers.First();
        Assert.Equal("admin@servicepulse.local", admin.Email);
        Assert.Equal("System Admin", admin.DisplayName);
        Assert.Equal("Admin", admin.Role);
        Assert.True(admin.IsActive);
        Assert.True(BCrypt.Net.BCrypt.Verify("Admin@123", admin.PasswordHash));
    }

    [Fact]
    public void SeedAdminIfNeeded_DoesNothing_WhenUsersAlreadyExist()
    {
        var ctx = CreateContext();
        ctx.ApplicationUsers.Add(new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = "existing@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("pwd"),
            DisplayName = "Existing",
            Role = "Agent",
            IsActive = true
        });
        ctx.SaveChanges();

        AuthService.SeedAdminIfNeeded(ctx);

        Assert.Single(ctx.ApplicationUsers);
        Assert.DoesNotContain("admin@servicepulse.local", ctx.ApplicationUsers.Select(u => u.Email));
    }

    [Fact]
    public async Task LoginAsync_NormalizesEmailAddress()
    {
        var ctx = CreateContext();
        var svc = CreateService(ctx);

        await svc.RegisterAsync(new RegisterRequest("NORMAL@test.com", "Pwd", "Norm", "Agent"));
        var result = await svc.LoginAsync(new LoginRequest("normal@test.com", "Pwd"));

        Assert.NotNull(result.AccessToken);
    }
}
