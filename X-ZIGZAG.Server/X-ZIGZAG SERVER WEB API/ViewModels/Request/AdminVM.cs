using System.ComponentModel.DataAnnotations;

namespace X_ZIGZAG_SERVER_WEB_API.ViewModels.Request
{
    public class AdminVM
    {
        [Required(ErrorMessage = "Username is Required")]
        public required string Username { get; set; }
        [Required(ErrorMessage = "Password is Required")]
        public required string Password { get; set; }

    }
}
