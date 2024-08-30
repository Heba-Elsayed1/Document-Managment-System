using Domain.Interface;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private UserManager<User> _userManager;

        public UserRepository(DataContext context , UserManager<User> userManager) : base(context)
        {
            _userManager = userManager;
        }

        public override async Task<IEnumerable<User>> GetAll()
        {
            var Users = await _userManager.GetUsersInRoleAsync("User");
            return Users;
        }

       
    }
}
