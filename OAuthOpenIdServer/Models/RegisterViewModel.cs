using System.ComponentModel.DataAnnotations;

namespace OAuthOpenIdServer.Models
{
    public class RegisterViewModel
    {
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        //[Required]
        //[DataType(DataType.Password)]
        //[Compare("Password")]
        //public string ConfirmPassword { get; set; }

        public string ReturnUrl { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }
    }
}