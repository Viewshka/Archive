using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;

namespace Archive.WebUI.Models
{
    public class RegisterModel
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public ObjectId DepartmentId { get; set; }

        [Required]
        public string UserName { get; set; }
        
        [Required]
        [StringLength(100, ErrorMessage = "Длина пароля должна быть от 6 до 100 символов",
            MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
    }
}