using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PayVault.API.Extensions
{
    public static class ControllerExtensions
    {
        public static Guid GetUserId(this ControllerBase controller)
        {
            // 👈 TEMP - YOUR REAL USERID
            return Guid.Parse("87d84407-11fb-4e8e-8131-fb41555631b7");
        }
    }
}