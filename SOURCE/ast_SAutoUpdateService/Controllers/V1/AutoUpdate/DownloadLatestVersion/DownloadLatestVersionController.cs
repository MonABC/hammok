using Azure.Storage.Blobs;
using Hammock.AssetView.Platinum.Tools.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Hammock.AssetView.Platinum.Tools.Controllers
{
    [ApiController]
    [Route("api/autoupdate/v1/downloadLatestVersion")]
    public class DownloadLatestVersionController : ControllerBase
    {
        [HttpPost]
        public Stream DownloadLatestVersion([FromHeader] AuthenticateModel authenticate, [FromBody] DownloadLatestVersionModel downloadLatestVersion)
        {
            if (!Authentication.Authenticate(authenticate))
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return null;
            }

            if (downloadLatestVersion is null)
            {
                throw new ArgumentNullException(nameof(downloadLatestVersion));
            }

            Validator.ValidateObject(downloadLatestVersion, new ValidationContext(downloadLatestVersion), true);

            string blobName = string.Format("{0}/{1}/{2}", downloadLatestVersion.SoftwareName, downloadLatestVersion.Version, "installer.zip");
            BlobClient blobClient = new BlobClient(AutoUpdateServiceSettings.CONNECTION_STRING, AutoUpdateServiceSettings.BLOB_CONTAINER_NAME, blobName);
            return blobClient.DownloadStreaming().Value.Content;
        }
    }
}