namespace AuthKit.Endpoints;

using AuthKit.Core.Services;
using AuthKit.Core.Services.DTOs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

public static class AuthEndpoints
{
    public static IEndpointConventionBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth").WithTags("Authentication");

        group.MapPost("/register", async (IAuthService service, RegisterRequest request) =>
        {
            return await service.RegisterAsync(request);
        });

        group.MapPost("/login", async (IAuthService service, LoginRequest request, string? ipAddress) =>
        {
            return await service.LoginAsync(request, ipAddress);
        });

        group.MapPost("/logout", async (IAuthService service, Guid userId) =>
        {
            return await service.LogoutAsync(userId);
        });

        group.MapPost("/logout-all", async (IAuthService service, Guid userId) =>
        {
            return await service.LogoutAllAsync(userId);
        });

        group.MapPost("/refresh", async (IAuthService service, RefreshTokenRequest request) =>
        {
            return await service.RefreshTokenAsync(request);
        });

        group.MapPost("/verify-email", async (IAuthService service, EmailVerificationRequest request) =>
        {
            return await service.VerifyEmailAsync(request);
        });

        group.MapPost("/resend-verification-email", async (IAuthService service, string email) =>
        {
            return await service.ResendVerificationEmailAsync(email);
        });

        group.MapPost("/change-password", async (IAuthService service, Guid userId, ChangePasswordRequest request) =>
        {
            return await service.ChangePasswordAsync(userId, request);
        });

        group.MapPost("/forgot-password", async (IAuthService service, ForgotPasswordRequest request) =>
        {
            return await service.ForgotPasswordAsync(request);
        });

        group.MapPost("/reset-password", async (IAuthService service, ResetPasswordRequest request) =>
        {
            return await service.ResetPasswordAsync(request);
        });

        return group;
    }
}
