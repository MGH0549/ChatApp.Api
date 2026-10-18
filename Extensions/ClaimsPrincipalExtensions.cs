using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ChatApp.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string? GetUsername(this ClaimsPrincipal user)
        {
            return user.FindFirst("unique_name")?.Value;
        }

        public static int GetUserId(this ClaimsPrincipal user)
        {
            return int.Parse(user.FindFirst("nameid")?.Value ?? "0");
        }
    }
}