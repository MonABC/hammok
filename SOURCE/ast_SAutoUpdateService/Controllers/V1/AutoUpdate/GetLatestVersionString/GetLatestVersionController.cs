using Azure.Storage.Blobs;
using Hammock.AssetView.Platinum.Tools.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Hammock.AssetView.Platinum.Tools.Controllers
{
    [ApiController]
    [Route("api/autoupdate/v1/getLatestVersion")]
    public class GetLatestVersionController : ControllerBase
    {
        [HttpPost]
        public async Task<string> GetLatestVersion([FromHeader] AuthenticateModel authenticate, [FromBody] GetLatestVersionModel getLatestVersion)
        {
            if (!Authentication.Authenticate(authenticate))
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return string.Empty;
            }

            if (getLatestVersion is null)
            {
                throw new ArgumentNullException(nameof(getLatestVersion));
            }

            Validator.ValidateObject(getLatestVersion, new ValidationContext(getLatestVersion), true);

            string blobName = string.Format("{0}/{1}", getLatestVersion.SoftwareName, "version.txt");
            BlobClient blobClient = new BlobClient(AutoUpdateServiceSettings.CONNECTION_STRING, AutoUpdateServiceSettings.BLOB_CONTAINER_NAME, blobName);

            var response = await blobClient.DownloadAsync();
            var streamReader = new StreamReader(response.Value.Content);
            var version = await streamReader.ReadLineAsync();

            if (version is null)
            {
                throw new InvalidOperationException();
            }
            else
            {
                Version.Parse(version);
            }

            return version;
        }
    }
}