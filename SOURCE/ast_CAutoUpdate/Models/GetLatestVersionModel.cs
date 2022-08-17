using System.ComponentModel.DataAnnotations;

namespace Hammock.AssetView.Platinum.Tools.Models
{
    public class GetLatestVersionModel
    {
        [Required]
        [StringLength(300)]
        public string SoftwareName { get; set; } = "";
    }
}
