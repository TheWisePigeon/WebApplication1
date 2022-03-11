using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace WebApplication1.Models
{
    public class PostModel
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        [DataType(DataType.Upload)]
        public IFormFile Media { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Poster { get; set; }

    }
}
