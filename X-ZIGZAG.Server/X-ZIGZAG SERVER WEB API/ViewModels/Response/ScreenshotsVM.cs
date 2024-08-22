namespace X_ZIGZAG_SERVER_WEB_API.ViewModels.Response
{
    public class Screenshot
    {
        public required string ScreenshotId { get; set; }
    }
    public class Screen
    {
        public required string Id { get; set; }
        public List<Screenshot>? Screenshots { get; set; }
    }
    public class ScreensVM
    {
        public List<Screen>? Screens { get; set; }
        public int? Duration { get; set; }
    }
}
