using Application.Common;
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

            public async Task<GenericResult<UserDto>> GetUserData(int userId)
            {
                if (userId < 0)
                    return GenericResult<UserDto>.Failure("Invalid Id");
            
                var user = await _unitOfWork.UserRepository.GetById(userId);
                if (user == null)
                    return GenericResult<UserDto>.Failure("User not found");

                var userDto = _mapper.Map<UserDto>(user);
                return GenericResult<UserDto>.Success(userDto);

            }

            public async Task<GenericResult<IEnumerable<UserDto>>> GetUsers(int pageNumber , int pageSize )
            {
                var users = await _unitOfWork.UserRepository.GetAll();
                var paginatedUsers = users.Skip((pageNumber-1)* pageSize).Take(pageSize).ToList(); 

                if (users == null)
                    return GenericResult<IEnumerable<UserDto>>.Failure("Users not found");

                var usersDto = _mapper.Map<List<UserDto>>(paginatedUsers);
                return GenericResult<IEnumerable<UserDto>>.Success(usersDto); 

            }

            public async Task<Result> lockUser(int userId , int durationInMinutes)
            {
                if (userId < 0)
                    return Result.Failure("Invalid Id");

                var userIdString = userId.ToString();
                var user = await _userManager.FindByIdAsync(userIdString);
                if (user == null)
                {
                    return Result.Failure("User not found");
                }
                user.LockoutEnabled = true;
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(durationInMinutes);

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return Result.Success();
                }
                else
                    return Result.Failure("Failed to lock user");

            }

            public async Task<GenericResult<LoginResultDto>> loginUser(LoginDto userDto)
            {
                User user = await _userManager.FindByEmailAsync(userDto.Email);

                if (user == null)
                {
                    return GenericResult<LoginResultDto>.Failure("Invalid Email");
                }

                if (await _userManager.IsLockedOutAsync(user))
                {
                    return GenericResult<LoginResultDto>.Failure("User account is locked out.");
                }

                bool isPasswordTrue = await _userManager.CheckPasswordAsync(user, userDto.Password);

                if (!isPasswordTrue)
                {
                    await _userManager.AccessFailedAsync(user);
                    return GenericResult<LoginResultDto>.Failure("Invalid password");
                }


                await _userManager.ResetAccessFailedCountAsync(user);

                 var (myToken, roles) = await createToken(user);

               return GenericResult<LoginResultDto>.Success(new LoginResultDto
                {
                    Token = myToken,
                    Role = roles.FirstOrDefault()
                });

                }

           public async Task<(JwtSecurityToken token , IList<string> roles)> createToken(User user)
            {
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
                expires: DateTime.Now.AddHours(3),
                signingCredentials: signingCredentials);
            
             return (myToken,roles);

           }

            public async Task<Result> registerUser(RegistrationDto userDto)
            {
            var workspaceExists = await _unitOfWork.WorkspaceRepository.GetWorkspaceByName(userDto.WorkspaceName);
            if (workspaceExists != null)
            {
                return Result.Failure("A workspace with this name already exists");
            }
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
                var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result.Failure(errorMessage);
            }
            else
            {
                await _userManager.AddToRoleAsync(user, "User");
                string WorkspacePath = GetWorkspacePath(userDto.WorkspaceName);
                Directory.CreateDirectory(WorkspacePath);
                return Result.Success();
            }
        }

            public async Task<Result> updateUser(UserDto userDto , int userId)
            {
                if(userDto.Id != userId)
                    return Result.Failure("Not Authorized");
            

                if (userDto.Id<0)
                    return Result.Failure("Invalid Id");

                var user = await _unitOfWork.UserRepository.GetById(userDto.Id);
                if (user == null)
                    return Result.Failure("user not found");

                _mapper.Map(userDto, user);
                _unitOfWork.UserRepository.Update(user);
                var result = _unitOfWork.Save() > 0;
                if (result)
                    return Result.Success();
                else
                    return Result.Failure("Falied to update user");

            }
        }
}
