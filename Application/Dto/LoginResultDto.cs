using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto
{
    public class LoginResultDto
    {
        public JwtSecurityToken Token { get; set; }
        public string Role { get; set; }
    }
}
