namespace ServicePulse.Application.DTOs;

public record LoginRequest(
    string Email,
    string Password
);

public record RegisterRequest(
    string Email,
    string Password,
    string DisplayName,
    string Role = "Agent"
);

public record TokenResponse(
    string AccessToken,
    string TokenType,
    int ExpiresIn,
    UserDto User
);

public record UserDto(
    Guid Id,
    string Email,
    string DisplayName,
    string Role
);
