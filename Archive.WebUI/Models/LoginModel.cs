using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Archive.WebUI.Models
{
    public class LoginModel
    {
        [DisplayName("Имя пользователя")] 
        public string UserName { get; set; }

        [DisplayName("Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayName("Запомнить?")]
        public bool RememberMe { get; set; }
    }
}