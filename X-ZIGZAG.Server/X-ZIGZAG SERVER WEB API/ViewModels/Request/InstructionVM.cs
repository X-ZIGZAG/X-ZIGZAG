using System.ComponentModel.DataAnnotations;

namespace X_ZIGZAG_SERVER_WEB_API.ViewModels.Request
{
    public class InstructionVM
    {
        [Required]
        public required short Code { get; set; }
        public string? FunctionArgs { get; set; }
    }
}
