using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto
{
    public class DocumentUploadDto
    {
        public string? Name;
        public int FolderId { get; set; }
        [Required]
        public IFormFile File { get; set; }
    }
}
