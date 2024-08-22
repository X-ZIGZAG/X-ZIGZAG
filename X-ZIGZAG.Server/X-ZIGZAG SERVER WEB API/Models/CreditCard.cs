using System.ComponentModel.DataAnnotations;

namespace X_ZIGZAG_SERVER_WEB_API.Models
{
    public class CreditCard
    {
        [Required]
        public required string ClientId { get; set; }
        [Required]
        public required long CreditCardId { get; set; }
        [Required]
        public required string BrowserName { get; set; }
        public required string Origin { get; set; }
        public required string CardHolder { get; set; }
        public required string ExpireDate { get; set; }
        public required string DecrypredCreditCard { get; set; }
    }
}
