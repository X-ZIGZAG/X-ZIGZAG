using System.ComponentModel.DataAnnotations;

namespace X_ZIGZAG_SERVER_WEB_API.ViewModels.Request
{
    public class SettingsRequestVM
    {
        public int? Screenshot { get; set; }
        public uint? CheckDuration { get; set; }
        public string? CustomName { get; set; }
    }
}