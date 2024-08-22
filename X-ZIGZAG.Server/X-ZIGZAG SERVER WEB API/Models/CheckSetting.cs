using System.ComponentModel.DataAnnotations;

namespace X_ZIGZAG_SERVER_WEB_API.Models
{
    public class CheckSetting
    {
        [Key]
        [Required]
        public required string Id { get; set; }
        [Required]
        public required int Screenshot { get; set; }
        // Duration Before Checking
        [Required]
        public required uint CheckDuration { get; set; }
        [Required]
        public required Boolean CheckCmds { get; set; }
        [Required]
        public required DateTimeOffset LatestPing { get; set; }
    }
}
