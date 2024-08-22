using System.ComponentModel.DataAnnotations;

namespace X_ZIGZAG_SERVER_WEB_API.ViewModels.Request
{
    public class AdminPasswordVM
    {
        [Required(ErrorMessage = "The New Password is Required")]
        public required string Password { get; set; }
    }
}
