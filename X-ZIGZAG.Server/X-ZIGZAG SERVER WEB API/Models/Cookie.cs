using System.ComponentModel.DataAnnotations;

namespace X_ZIGZAG_SERVER_WEB_API.Models
{
    public class Cookie
    {
        [Required]
        public required string ClientId { get; set; }
        [Required]
        public required long CookieId { get; set; }
        [Required]
        public required string BrowserName { get; set; }
        public required string Origin { get; set; }
        public required string Name { get; set; }
        public required long ExpireDate { get; set; }
        public required string Value { get; set; }
    }
}
