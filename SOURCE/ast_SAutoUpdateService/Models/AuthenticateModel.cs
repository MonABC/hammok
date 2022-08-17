using Microsoft.AspNetCore.Mvc;

namespace Hammock.AssetView.Platinum.Tools.Models
{
    public class AuthenticateModel
    {
        [FromHeader]
        public string? UserName { get; set; }

        [FromHeader]
        public string? Password { get; set; }
    }
}
