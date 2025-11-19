using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectBusiness
{
    public class Users
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserId { get; set; } // Primary Key
        [Required(ErrorMessage = "First name cannot be blank")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name cannot be blank")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email cannot be blank")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Date of birth cannot be blank")]
        public DateTime DateOfBirth { get; set; }
        [Required(ErrorMessage = "The roles cannot be different between student, parent, and admin")]
        public Role Role { get; set; } // Parent or Student or Admin
        [Required(ErrorMessage = "Password cannot be blank")]
        public string Password { get; set; }
        public string Avatar { get; set; }
        public string PickImage1 { get; set; }
        public string PickImage2 { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // IFormFile is used for only upload img from frontend
        [NotMapped]
        public IFormFile AvatarFromFrontend { get; set; }
        [NotMapped]
        public IFormFile PickImage1FromFrontend { get; set; }
        [NotMapped]
        public IFormFile PickImage2FromFrontend { get; set; }
    }
}
