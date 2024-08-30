using Application.Dto;
using Application.Interface;
using Application.Service;
using AutoMapper;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WepApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configration;
        private readonly IWorkspaceService _workspace;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;

        public UserController(UserManager<User> userManager, IConfiguration configration, IWorkspaceService workspace, IUserService userService, IMapper mapper, IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _configration = configration;
            _workspace = workspace;
            _userService = userService;
            _mapper = mapper;
            _environment = environment;

        }

        [HttpPost("register")]
        public async Task<IActionResult> Registeration(RegistrationDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var workspaceExists = await _workspace.WorkspaceExists(userDto.WorkspaceName);
            if (workspaceExists == true)
            {
                return BadRequest("A workspace with this name already exists");
            }

            var (isRegister , result) = await _userService.registerUser(userDto);

            if (isRegister == false)
            {
                var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(errorMessage);
            }

            return Ok("Account is Created Successfully");
        }





        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto userDto)
        {
            if (!ModelState.IsValid)
                return Unauthorized();
            

            var (isLogin, message, mytoken) = await _userService.loginUser(userDto);
            if (isLogin == false)
                return BadRequest(message);

            else
            {
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(mytoken),
                    expiration = DateTime.Now.AddHours(1)
                });
            }
        }



        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetUsers();

            if (users != null)
                return Ok(users);
            else
                return BadRequest();

        }


        [Authorize(Roles = "Admin")]
        [HttpPost("lockuser/{userId}")]
        public async Task<IActionResult> LockUser(int userId, int durationInMinutes )
        {
            var (islocked, message) = await _userService.lockUser(userId, durationInMinutes);

            if (islocked)
            {
                return Ok("User account locked successfully.");
            }

            return BadRequest(message);
        }

        [HttpGet("userData")]
        public async Task<IActionResult> GetUserData(int userId)
        {
            int userIdclaims = GetUserId();

            if (userIdclaims == userId || (userIdclaims != userId && idAdmin()))
            {
                var user = await _userService.GetUserData(userId);
                return Ok(user);
            }
            else
                return BadRequest("Not Authorized");

        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser(UserDto userDto)
        {
            int userId = GetUserId();
            if(userDto.Id == userId)
            {
                var user = await _userService.updateUser(userDto);
                return Ok("user updated successfully");
            }
            else
                return BadRequest("Not Authorized");

        }



    }     
}
