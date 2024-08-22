using System.ComponentModel.DataAnnotations;

namespace X_ZIGZAG_SERVER_WEB_API.Models
{
    public class SystemInfo
    {
        [Key]
        [Required]
        public required string Id { get; set; }
        public string? CustomName { get; set; }
        [Required]
        public required string Name { get; set; }
        [Required]
        public required DateTimeOffset Created { get; set; }
        [Required]
        public required DateTimeOffset LatestUpdate { get; set; }
        [Required]
        public required string IpAddress { get; set; }
        [Required]
        public required string Version { get; set; }
        [Required]
        public required string SystemSpecs { get; set; }
    }
}
