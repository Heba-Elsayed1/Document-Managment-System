using Application.Dto;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetUsers();
        Task<(bool isRegister , IdentityResult result)> registerUser(RegistrationDto userDto);
        Task<(bool isLogin , string message , JwtSecurityToken myToken)> loginUser(LoginDto userDto);
        Task<(bool isLogin, string message)> lockUser(int userId , int durationInMinutes);
        Task<UserDto> GetUserData (int userId);
        Task<bool> updateUser(UserDto userDto);
    }
}
