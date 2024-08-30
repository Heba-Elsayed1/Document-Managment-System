using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto
{
    public class RegistrationDto
    {
        public string FullName { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string PasswordConfirmed { get; set; }
        public string NID { get; set; }
        public string PhoneNumber {  get; set; }
        public string Gender { get; set; }

        [Required]
        public string WorkspaceName {  get; set; }
    }
}
