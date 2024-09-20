using System.ComponentModel.DataAnnotations;

namespace X_ZIGZAG_SERVER_WEB_API.ViewModels.Response
{
    public class InstructionResponseVM
    {
        [Required]
        public required long InstructionId { get; set; }

        [Required]
        public required short Code { get; set; }

        public string? Script { get; set; }
        public required bool Notify { get; set; }
        public string? FunctionArgs { get; set; }
    }
}
