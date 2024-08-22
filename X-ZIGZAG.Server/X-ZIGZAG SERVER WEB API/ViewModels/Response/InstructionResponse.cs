using X_ZIGZAG_SERVER_WEB_API.Models;

namespace X_ZIGZAG_SERVER_WEB_API.ViewModels.Response
{
    public class InstructionResponse
    {
        public string? Message { get; set; }
        public List<InstructionResponseVM>? Instructions { get; set; }
    }
}
