using System.ComponentModel.DataAnnotations;

namespace Hammock.AssetView.Platinum.Tools.Models
{
    public class DownloadLatestVersionModel
    {
        [Required]
        [StringLength(300)]
        public string SoftwareName { get; set; } = "";

        [Required]
        [StringLength(50)]
        public string Version { get; set; } = "";
    }
}

