using System.ComponentModel.DataAnnotations;

namespace X_ZIGZAG_SERVER_WEB_API.Models
{
    public class Result
    {
        [Required]
        public required string ClientId { get; set; }


        [Required]
        public required long InstructionId { get; set; }

        [Required]
        public required short Code { get; set; }
        public required DateTimeOffset ResultDate { get; set; }
        public string? FunctionArgs { get; set; }
        public string? Output { get; set; }


    }
}
