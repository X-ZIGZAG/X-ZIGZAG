using X_ZIGZAG_SERVER_WEB_API.Models;

namespace X_ZIGZAG_SERVER_WEB_API.ViewModels.Response
{
    public class ClientPingResponse
    {
        public int? Code { get; set; }
        public int? Screenshot { get; set; }
        public required uint NextPing { get; set; }
        public List<InstructionResponseVM>? Instructions { get; set; }
    }
}
