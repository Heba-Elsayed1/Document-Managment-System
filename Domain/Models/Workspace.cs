using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Workspace
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public  User User { get; set; }
        public ICollection<Folder> Folders { get; set; }
    }
}
