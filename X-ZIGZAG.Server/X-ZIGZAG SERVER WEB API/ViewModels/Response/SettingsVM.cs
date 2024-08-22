using System.ComponentModel.DataAnnotations;

namespace X_ZIGZAG_SERVER_WEB_API.ViewModels.Response
{
    public class SettingsVM
    {
        public class CheckSetting
        {
            public required int Screenshot { get; set; }
            [Required]
            public required uint CheckDuration { get; set; }
            [Required]
            public required DateTimeOffset LatestPing { get; set; }
        }
    }
}
