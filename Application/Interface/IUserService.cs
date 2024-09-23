using Application.Common;
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
        Task<GenericResult<IEnumerable<UserDto>>> GetUsers();
        Task<GenericResult<UserDto>> GetUserData(int userId);
        Task<Result> registerUser(RegistrationDto userDto);
        Task<GenericResult<LoginResultDto>> loginUser(LoginDto userDto);
        Task<Result> lockUser(int userId , int durationInMinutes);
        Task<Result> updateUser(UserDto userDto, int userId);
    }
}
