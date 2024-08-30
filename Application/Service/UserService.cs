using Application.Dto;
using Application.Interface;
using AutoMapper;
using Domain.Interface;
using Domain.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class UserService : BaseService, IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configration;

        public UserService(IUnitOfWork unitOfWork , IMapper mapper , UserManager<User> userManager , IConfiguration configration, IWebHostEnvironment environment):base(environment)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _configration=configration;

        }

        public async Task<UserDto> GetUserData(int userId)
        {

            var user = await _unitOfWork.UserRepository.GetById(userId);
            return _mapper.Map<UserDto>(user);

        }

        public async Task<IEnumerable<UserDto>> GetUsers()
        {
            var users = await _unitOfWork.UserRepository.GetAll();
            return _mapper.Map<List<UserDto>>(users);

        }

        public async Task<(bool isLogin, string message)> lockUser(int userId , int durationInMinutes)
        {
            var userIdString = userId.ToString();
            var user = await _userManager.FindByIdAsync(userIdString);
            if (user == null)
            {
                return (false,"User not found.");
            }

            user.LockoutEnabled = true;
            user.LockoutEnd = DateTime.UtcNow.AddMinutes(durationInMinutes);

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return (true, "User account locked successfully.");
            }
            else
                return (false,"");

        }

        public async Task<(bool isLogin, string message , JwtSecurityToken myToken)> loginUser(LoginDto userDto)
        {
            User user = await _userManager.FindByEmailAsync(userDto.Email);

            if (user == null)
            {
                return (false,"Invalid Email",null);
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                return (false ,"User account is locked out.",null);
            }

            bool isPasswordTrue = await _userManager.CheckPasswordAsync(user, userDto.Password);

            if (!isPasswordTrue)
            {
                await _userManager.AccessFailedAsync(user);
                return (false, "Invalid password",null);
            }


            await _userManager.ResetAccessFailedCountAsync(user);

            var claims = new List<Claim>
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName)
             };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configration["JWT:SecretKey"]));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var myToken = new JwtSecurityToken(
                issuer: _configration["JWT:ValidIssuer"],
                audience: _configration["JWT:ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: signingCredentials);

            return (true,"", myToken);
           }


            public async Task<(bool isRegister, IdentityResult result)> registerUser(RegistrationDto userDto)
            {
                var user = new User
                {
                    FullName = userDto.FullName,
                    UserName = userDto.UserName,
                    Email = userDto.Email,
                    NID = userDto.NID,
                    PhoneNumber = userDto.PhoneNumber,
                    Gender = userDto.Gender,
                    Workspace = new Workspace
                    {
                        Name = userDto.WorkspaceName,
                    }
                };

                IdentityResult result = await _userManager.CreateAsync(user, userDto.Password);
                if (!result.Succeeded)
                {
                    return (false , result);
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    string WorkspacePath = GetWorkspacePath(userDto.WorkspaceName);
                    Directory.CreateDirectory(WorkspacePath);
                    return (true,null) ;

                }
        }

        public async Task<bool> updateUser(UserDto userDto)
        {
            var user = await _unitOfWork.UserRepository.GetById(userDto.Id);
            if (user == null)
                return false;

            _mapper.Map(userDto, user);
            _unitOfWork.UserRepository.Update(user);
            return _unitOfWork.Save() > 0;

        }
    }
}
