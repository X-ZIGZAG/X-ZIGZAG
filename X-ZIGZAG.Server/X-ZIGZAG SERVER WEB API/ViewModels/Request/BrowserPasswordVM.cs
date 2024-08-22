using System.ComponentModel.DataAnnotations;

namespace X_ZIGZAG_SERVER_WEB_API.ViewModels.Request
{
    public class BrowserPasswordVM
    {
        [Required]
        public required string Key { get; set; }
        [Required]
        public required string Data { get; set; }
        [Required]
        public required string Browser { get; set; }
    }
}
