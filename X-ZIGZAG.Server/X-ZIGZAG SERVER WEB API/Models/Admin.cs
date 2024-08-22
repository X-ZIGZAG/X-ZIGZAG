using System.ComponentModel.DataAnnotations;

namespace X_ZIGZAG_SERVER_WEB_API.Models
{

    public class Admin
    {
        [Key]
        public required string Id { get; set; }
        [Required]
        [StringLength(100,MinimumLength =1)]
        public required string Username { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public required string Password { get; set; }


    }
}
