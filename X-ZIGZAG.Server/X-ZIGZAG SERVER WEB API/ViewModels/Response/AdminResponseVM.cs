using X_ZIGZAG_SERVER_WEB_API.Models;

namespace X_ZIGZAG_SERVER_WEB_API.ViewModels.Response
{
    public class AdminResponseVM
    {
        public string? Token { get; set; }
        public string? Message { get; set; }
        public List<string>? Admins { get; set; }
    }
}
