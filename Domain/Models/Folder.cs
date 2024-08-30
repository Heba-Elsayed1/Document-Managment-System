using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Folder
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        public bool IsPublic { get; set; } = false; 
        public bool IsDeleted { get; set; } = false;

        [ForeignKey("Workspace")]
        public int WorkspaceId { get; set; }
        public Workspace Workspace { get; set; }
        public ICollection<Document> Documents { get; set; }
        
    }
}
