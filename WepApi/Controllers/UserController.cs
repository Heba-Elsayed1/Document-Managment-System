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
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;

        public UserController(UserManager<User> userManager, IConfiguration configration, IUserService userService, IMapper mapper, IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _configration = configration;
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

            var register = await _userService.registerUser(userDto);

            if (register.IsSuccess)
            {
                return Ok();
            }
            else
            {
                return BadRequest(register.ErrorMessage);
            }
        }





        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto userDto)
        {
            if (!ModelState.IsValid)
                return Unauthorized();

            var login = await _userService.loginUser(userDto);
            if (login.IsSuccess)
            {
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(login.Value.Token),
                    expiration = DateTime.Now.AddHours(3),
                    role = login.Value.Role 
                });
            }
            else
            {
               return BadRequest(login.ErrorMessage);
            }
        }



        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetUsers(int pageNumber = 1, int pageSize = 10)
        {
            var users = await _userService.GetUsers(pageNumber , pageSize);

            if (users.IsSuccess)
                return Ok(users.Value);
            else
                return BadRequest(users.ErrorMessage);

        }


        [Authorize(Roles = "Admin")]
        [HttpPost("lockuser/{userId}")]
        public async Task<IActionResult> LockUser(int userId, int durationInMinutes )
        {
            var locked = await _userService.lockUser(userId, durationInMinutes);

            if (locked.IsSuccess)
            {
                return Ok();
            }

            return BadRequest(locked.ErrorMessage);
        }

        [HttpGet("userData")]
        public async Task<IActionResult> GetUserData()
        {
            int userId = GetUserId();

            var user = await _userService.GetUserData(userId);
            if (user.IsSuccess)
                return Ok(user.Value);
            else
                return BadRequest(user.ErrorMessage);

        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser(UserDto userDto)
        {
            int userId = GetUserId();
            var user = await _userService.updateUser(userDto, userId);

            if (user.IsSuccess)
                return Ok();
            else
                return BadRequest(user.ErrorMessage); 

        }



    }     
}
