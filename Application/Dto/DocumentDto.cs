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
    public class DocumentDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int FolderId { get; set; }
        public string Type { get; set; }
        public DateTime CreationDate { get; set; }


    }
}
