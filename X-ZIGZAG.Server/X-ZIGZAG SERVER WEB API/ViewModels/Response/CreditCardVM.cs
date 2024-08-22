using System.ComponentModel.DataAnnotations;

namespace X_ZIGZAG_SERVER_WEB_API.ViewModels.Response
{
    public class CreditCardVM
    {
        public required string Browser { get; set; }
        public required string Origin { get; set; }
        public required string Name { get; set; }
        public required string Expire { get; set; }
        public required string Value { get; set; }
    }
}
