using System.ComponentModel.DataAnnotations;

namespace X_ZIGZAG_SERVER_WEB_API.ViewModels.Request
{
    public class AdminUsernameVM
    {
        [Required(ErrorMessage ="The New Username is Required")]
        public required string Username { get; set; }
    }
}
