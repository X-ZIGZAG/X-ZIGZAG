using System.ComponentModel.DataAnnotations;

namespace X_ZIGZAG_SERVER_WEB_API.ViewModels.Response
{
    public class ClientInfoResponseVM
    {
        public string? Id { get; set; }  
        public string? CustomName { get; set; }
        public required string Name { get; set; }
        public required DateTimeOffset Created { get; set; }
        public required DateTimeOffset LatestUpdate { get; set; }
        public required DateTimeOffset LatestPing{ get; set; }
        public required string IpAddress { get; set; }
        public required string Version { get; set; }
        public required string SystemSpecs { get; set; }
        public required uint CheckDuration { get; set; }

    }
}
