using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Extensions
{
    public static class FindUserIdFromClaimsExtension
    {
        public static int GeTUserIdFromClaims(this ClaimsPrincipal user)
        {
            return int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));
        }


        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            return user.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == RoleEnum.Admin.ToString());
        }
    }
}
