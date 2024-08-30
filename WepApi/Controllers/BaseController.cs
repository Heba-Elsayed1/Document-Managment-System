using Application.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WepApi.Controllers
{
    
    public class BaseController : ControllerBase
    {
        protected int GetUserId()
        {
            return User.GeTUserIdFromClaims();
        }
        protected bool idAdmin()
        {
            return User.IsAdmin();
        }
    }
}
