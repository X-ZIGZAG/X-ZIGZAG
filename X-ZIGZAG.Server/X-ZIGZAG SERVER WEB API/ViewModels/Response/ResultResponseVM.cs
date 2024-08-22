namespace X_ZIGZAG_SERVER_WEB_API.ViewModels.Response
{
    public class ResultResponseVM
    {
        public required long InstructionId { get; set; }
        public required short Code { get; set; }
        public required DateTimeOffset ResultDate { get; set; }
        public string? FunctionArgs { get; set; }
        public string? Output { get; set; }

    }
}
