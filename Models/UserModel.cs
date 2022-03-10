using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class UserModel
    {
        public UserModel(string email, string username, string password, string gender, string bio)
        {
            Email = email;
            Username = username;
            Password = password;
            Gender = gender;
            Bio = bio;
        }

        [Key]
        [Required]
        public string Email { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string Bio { get; set; }
    }
}
