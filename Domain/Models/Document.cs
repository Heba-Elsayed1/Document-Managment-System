using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Document
    {
        [Key]
        public int Id { get; set; }

        //[Required]
        //[MaxLength(255)]
        public string Name { get; set; }

        //[Required]
        //[MaxLength(255)]
        public string Path { get; set; }

        [MaxLength(50)]
        public string Type { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsPublic { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public Folder Folder { get; set; }

        [ForeignKey("Folder")]
        public int FolderId { get; set; }
    }
}
