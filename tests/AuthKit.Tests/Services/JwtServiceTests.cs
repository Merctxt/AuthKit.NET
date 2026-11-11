namespace AuthKit.Tests.Services;

using AuthKit.Core.Configuration;
using AuthKit.Core.Services.Implementations;
using Microsoft.IdentityModel.Tokens;

public class JwtServiceTests
{
    private readonly JwtOptions _validOptions = new()
    {
        SecretKey = "T3h!K8m#pL2qR$vN5wX7yA0bC4dF9gH1jK3mP6rS8tU0xZ2cE5fI7lO1qT4vW8yB",
        Issuer = "AuthKit",
        Audience = "AuthKitClient",
        Expiration = TimeSpan.FromMinutes(15),
    };

    [Fact]
    public async Task GenerateTokenAsync_ReturnsNonEmptyString()
    {
        var service = new JwtService(_validOptions);
        var userId = Guid.NewGuid();
        var token = await service.GenerateTokenAsync(userId, new List<string> { "Admin" });

        token.Should().NotBeNullOrEmpty();
        token.Split('.').Length.Should().Be(3, "JWT must have 3 parts.");
    }

    [Fact]
    public async Task GenerateTokenAsync_IncludesUserId()
    {
        var service = new JwtService(_validOptions);
        var userId = Guid.NewGuid();
        var token = await service.GenerateTokenAsync(userId, null);

        var principal = await service.ParseTokenAsync(token);
        principal.Should().NotBeNull();

        var claimValue = principal!.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        claimValue.Should().Be(userId.ToString());
    }

    [Fact]
    public async Task GenerateTokenAsync_IncludesRoles()
    {
        var service = new JwtService(_validOptions);
        var userId = Guid.NewGuid();
        var roles = new List<string> { "Admin", "User" };
        var token = await service.GenerateTokenAsync(userId, roles);

        var principal = await service.ParseTokenAsync(token);
        principal.Should().NotBeNull();

        var roleClaims = principal!.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToList();
        roleClaims.Should().Contain("Admin").And.Contain("User");
    }

    [Fact]
    public async Task GenerateTokenAsync_EmptyRoles_OnlyIncludesUserId()
    {
        var service = new JwtService(_validOptions);
        var userId = Guid.NewGuid();
        var token = await service.GenerateTokenAsync(userId, new List<string>());

        var principal = await service.ParseTokenAsync(token);
        principal.Should().NotBeNull();

        var roleClaims = principal!.FindAll(System.Security.Claims.ClaimTypes.Role).ToList();
        roleClaims.Should().BeEmpty();

        var userIdClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        userIdClaim.Should().Be(userId.ToString());
    }

    [Fact]
    public async Task ValidateTokenAsync_ValidToken_ReturnsTrue()
    {
        var service = new JwtService(_validOptions);
        var userId = Guid.NewGuid();
        var token = await service.GenerateTokenAsync(userId, null);

        var isValid = await service.ValidateTokenAsync(token);

        isValid.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateTokenAsync_InvalidToken_ReturnsFalse()
    {
        var service = new JwtService(_validOptions);
        var isValid = await service.ValidateTokenAsync("invalid.token.here");

        isValid.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateTokenAsync_TamperedToken_ReturnsFalse()
    {
        var service = new JwtService(_validOptions);
        var userId = Guid.NewGuid();
        var token = await service.GenerateTokenAsync(userId, null);

        var tampered = token + "_modified";

        var isValid = await service.ValidateTokenAsync(tampered);

        isValid.Should().BeFalse();
    }

    [Fact]
    public async Task ParseTokenAsync_ValidToken_ReturnsPrincipal()
    {
        var service = new JwtService(_validOptions);
        var userId = Guid.NewGuid();
        var roles = new List<string> { "Admin" };
        var token = await service.GenerateTokenAsync(userId, roles);

        var principal = await service.ParseTokenAsync(token);

        principal.Should().NotBeNull();
        principal!.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value.Should().Be(userId.ToString());
    }

    [Fact]
    public async Task ParseTokenAsync_InvalidToken_ReturnsNull()
    {
        var service = new JwtService(_validOptions);
        var principal = await service.ParseTokenAsync("not.a.token");

        principal.Should().BeNull();
    }

    [Fact]
    public async Task GenerateRefreshTokenAsync_ReturnsNonEmptyString()
    {
        var service = new JwtService(_validOptions);
        var refreshToken = await service.GenerateRefreshTokenAsync();

        refreshToken.Should().NotBeNullOrEmpty();
        refreshToken.Length.Should().BeGreaterThan(32);
    }

    [Fact]
    public async Task GenerateRefreshTokenAsync_DifferentValuesEachTime()
    {
        var service = new JwtService(_validOptions);
        var r1 = await service.GenerateRefreshTokenAsync();
        var r2 = await service.GenerateRefreshTokenAsync();

        r1.Should().NotBe(r2);
    }

    [Fact]
    public async Task RotateTokenAsync_WithValidTokens_ReturnsNewTokenResponse()
    {
        var service = new JwtService(_validOptions);
        var userId = Guid.NewGuid();
        var roles = new List<string> { "User" };

        var accessToken = await service.GenerateTokenAsync(userId, roles);
        var refreshToken = await service.GenerateRefreshTokenAsync();

        var result = await service.RotateTokenAsync(accessToken, refreshToken);

        result.AccessToken.Should().NotBeNullOrEmpty();
        result.AccessToken.Should().NotBe(accessToken);
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.AccessToken.Should().NotBe(result.RefreshToken);
    }

    [Fact]
    public async Task RotateTokenAsync_WithInvalidAccessToken_Throws()
    {
        var service = new JwtService(_validOptions);
        var refreshToken = await service.GenerateRefreshTokenAsync();

        var exception = await Assert.ThrowsAsync<SecurityTokenException>(() =>
            service.RotateTokenAsync("invalid.token", refreshToken));

        exception.Message.Should().Contain("Invalid access token");
    }

    [Fact]
    public async Task RegenerateTokenAsync_ReturnsSuccessResult()
    {
        var service = new JwtService(_validOptions);
        var userId = Guid.NewGuid();

        var result = await service.RegenerateTokenAsync(userId);

        result.Success.Should().BeTrue();
    }
}
