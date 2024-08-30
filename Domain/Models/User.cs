using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class User : IdentityUser<int>
    {
        public string NID { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public Workspace Workspace { get; set; }
        
    }
}
