namespace X_ZIGZAG_SERVER_WEB_API.ViewModels.Response
{
    public class CookieVM
    {
        public required string Browser {  get; set; }
        public required string Origin { get; set; }
        public required string Name { get; set; }
        public required string Value { get; set; }
        public required long Expire { get; set; }
    }
}
