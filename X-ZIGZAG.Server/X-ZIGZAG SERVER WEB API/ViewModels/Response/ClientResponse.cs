using X_ZIGZAG_SERVER_WEB_API.Models;

namespace X_ZIGZAG_SERVER_WEB_API.ViewModels.Response
{
    public class ClientResponse
    {
        public string? Message { get; set; }
        public List<ClientInfoResponseVM>? DevicesInfo { get; set; }
    }
}
