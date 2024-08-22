using System.ComponentModel.DataAnnotations;

namespace X_ZIGZAG_SERVER_WEB_API.ViewModels.Response
{
    public class PasswordVM
    {
        public required string Browser { get; set; }
        public string? Url { get; set; }
        public string? Login { get; set; }
        public required string Value { get; set; }
    }
}
