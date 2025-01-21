namespace BackendManagement.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateToken(string userId, string username, IEnumerable<string> roles);
    bool ValidateToken(string token);
    ClaimsPrincipal? GetPrincipalFromToken(string token);
} 