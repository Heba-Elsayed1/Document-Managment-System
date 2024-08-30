using Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto
{
    public class FolderDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsPublic { get; set; } = false;
        
    }
}
