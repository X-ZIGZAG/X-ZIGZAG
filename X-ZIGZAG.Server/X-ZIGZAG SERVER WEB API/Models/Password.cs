using System.ComponentModel.DataAnnotations;

namespace X_ZIGZAG_SERVER_WEB_API.Models
{
    public class Password
    {
        [Required]
        public required string ClientId { get; set; }
        [Required]
        public required long PasswordId { get; set; }
        [Required]
        public required string BrowserName { get; set; }
        public string ?Url { get; set; }
        public string ?Login { get; set; }
        public required string DecrypredPassword { get; set; }

    }
}
