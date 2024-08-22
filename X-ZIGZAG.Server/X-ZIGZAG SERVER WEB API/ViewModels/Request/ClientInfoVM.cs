using System.ComponentModel.DataAnnotations;

namespace X_ZIGZAG_SERVER_WEB_API.ViewModels.Request
{
    public class ClientInfoVM
    {
        [Key]
        [Required]
        public required string Id { get; set; }
        [Required]
        public required string Name { get; set; }
        [Required]
        public required string IpAddress { get; set; }
        [Required]
        public required string Version { get; set; }
        [Required]
        public required string SystemSpecs { get; set; }
    }
}
