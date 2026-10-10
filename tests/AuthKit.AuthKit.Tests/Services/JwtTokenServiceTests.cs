using AuthKit.Core.Options;
using AuthKit.AuthKit.Services;

namespace AuthKit.AuthKit.Tests.Services;

public class JwtTokenServiceTests
{
    private readonly JwtOptions _jwtOptions;

    public JwtTokenServiceTests()
    {
        _jwtOptions = new JwtOptions
        {
            Secret = "ThisIsASecretKeyForTesting1234567890!",
            Issuer = "AuthKit.NET",
            Audience = "AuthKitApp",
            AccessExpiration = TimeSpan.FromMinutes(15),
            ValidateIssuer = true,
            ClockSkewMinutes = 1
        };
    }

    [Fact]
    public void GenerateAccessToken_ReturnsNonNullString()
    {
        // Arrange
        var service = new JwtTokenService(_jwtOptions);
        var userId = Guid.NewGuid();
        var email = "test@example.com";

        // Act
        var token = service.GenerateAccessToken(userId, email, [], [], []);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    [Fact]
    public void GenerateAccessToken_ContainsCorrectClaims()
    {
        // Arrange
        var service = new JwtTokenService(_jwtOptions);
        var userId = Guid.NewGuid();
        var email = "test@example.com";
        var roles = new[] { "Admin", "User" };
        var permissions = new[] { "users:read", "users:write" };

        // Act
        var token = service.GenerateAccessToken(userId, email, roles, permissions, []);

        // Assert
        var principal = service.ValidateToken(token);
        Assert.NotNull(principal);
        Assert.Equal(userId.ToString(), principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
        Assert.Equal(email, principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value);
    }

    [Fact]
    public void GenerateAccessToken_IncludesRoles()
    {
        // Arrange
        var service = new JwtTokenService(_jwtOptions);
        var userId = Guid.NewGuid();
        var email = "test@example.com";
        var roles = new[] { "Admin", "User" };

        // Act
        var token = service.GenerateAccessToken(userId, email, roles, [], []);

        // Assert
        var principal = service.ValidateToken(token);
        Assert.NotNull(principal);
        var rolesClaims = principal.FindAll(System.Security.Claims.ClaimTypes.Role).ToList();
        Assert.Contains(rolesClaims, c => c.Value == "Admin");
        Assert.Contains(rolesClaims, c => c.Value == "User");
    }

    [Fact]
    public void GenerateAccessToken_IncludesPermissions()
    {
        // Arrange
        var service = new JwtTokenService(_jwtOptions);
        var userId = Guid.NewGuid();
        var email = "test@example.com";
        var permissions = new[] { "users:read", "settings:edit" };

        // Act
        var token = service.GenerateAccessToken(userId, email, [], permissions, []);

        // Assert
        var principal = service.ValidateToken(token);
        Assert.NotNull(principal);
        var permissionClaims = principal.FindAll("permission").ToList();
        Assert.Contains(permissionClaims, c => c.Value == "users:read");
        Assert.Contains(permissionClaims, c => c.Value == "settings:edit");
    }

    [Fact]
    public void GenerateTokenPair_ReturnsBothTokens()
    {
        // Arrange
        var service = new JwtTokenService(_jwtOptions);
        var userId = Guid.NewGuid();
        var email = "test@example.com";

        // Act
        var (accessToken, refreshToken) = service.GenerateTokenPair(userId, email, [], [], []);

        // Assert
        Assert.NotEmpty(accessToken);
        Assert.NotEmpty(refreshToken);
        Assert.NotEqual(accessToken, refreshToken);
    }

    [Fact]
    public void ValidateToken_WithValidToken_ReturnsPrincipal()
    {
        // Arrange
        var service = new JwtTokenService(_jwtOptions);
        var userId = Guid.NewGuid();
        var token = service.GenerateAccessToken(userId, "test@example.com", [], [], []);

        // Act
        var principal = service.ValidateToken(token);

        // Assert
        Assert.NotNull(principal);
    }

    [Fact]
    public void ValidateToken_WithInvalidToken_ReturnsNull()
    {
        // Arrange
        var service = new JwtTokenService(_jwtOptions);

        // Act
        var principal = service.ValidateToken("invalid.token.here");

        // Assert
        Assert.Null(principal);
    }

    [Fact]
    public void ValidateToken_WithTamperedToken_ReturnsNull()
    {
        // Arrange
        var service = new JwtTokenService(_jwtOptions);
        var token = service.GenerateAccessToken(Guid.NewGuid(), "test@example.com", [], [], []);
        var tamperedToken = token.Substring(0, token.Length - 5) + "XXXXX";

        // Act
        var principal = service.ValidateToken(tamperedToken);

        // Assert
        Assert.Null(principal);
    }

    [Fact]
    public void GetUserIdFromToken_WithValidToken_ReturnsUserId()
    {
        // Arrange
        var service = new JwtTokenService(_jwtOptions);
        var userId = Guid.NewGuid();
        var token = service.GenerateAccessToken(userId, "test@example.com", [], [], []);

        // Act
        var result = service.GetUserIdFromToken(token);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result);
    }

    [Fact]
    public void GetEmailFromToken_WithValidToken_ReturnsEmail()
    {
        // Arrange
        var service = new JwtTokenService(_jwtOptions);
        var userId = Guid.NewGuid();
        var token = service.GenerateAccessToken(userId, "test@example.com", [], [], []);

        // Act
        var email = service.GetEmailFromToken(token);

        // Assert
        Assert.Equal("test@example.com", email);
    }

    [Fact]
    public void GetRolesFromToken_WithRoles_ReturnsRoles()
    {
        // Arrange
        var service = new JwtTokenService(_jwtOptions);
        var userId = Guid.NewGuid();
        var token = service.GenerateAccessToken(userId, "test@example.com", new[] { "Admin", "User" }, [], []);

        // Act
        var roles = service.GetRolesFromToken(token);

        // Assert
        Assert.NotNull(roles);
        Assert.Contains("Admin", roles);
        Assert.Contains("User", roles);
    }

    [Fact]
    public void GenerateRefreshToken_ReturnsLongRandomString()
    {
        // Arrange
        var service = new JwtTokenService(_jwtOptions);

        // Access private method via GenerateTokenPair
        var (_, refreshToken) = service.GenerateTokenPair(Guid.NewGuid(), "test@example.com", [], [], []);

        // Assert - 64 bytes = 86 base64 chars
        Assert.NotNull(refreshToken);
        Assert.True(refreshToken.Length > 60);
    }
}
