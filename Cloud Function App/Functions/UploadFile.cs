using System.IO;
using System.Net;
using Azure.Storage.Files.Shares;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Cloud_Function_App.Functions
{
    public static class UploadFile
    {
        [Function("UploadFile")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string shareName = "contracts-logs";

            string fileName = req.Query["fileName"];

            if (string.IsNullOrEmpty(fileName) && string.IsNullOrEmpty(shareName))
            {
                return new BadRequestObjectResult("Share name and file name must be provided.");
            }
            else if (string.IsNullOrEmpty(fileName))
            {
                return new BadRequestObjectResult("File name must be provided.");
            }
            else if (string.IsNullOrEmpty(shareName))
            {
                return new BadRequestObjectResult("Share name must be provided.");
            }

            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

            var shareServiceClient = new ShareServiceClient(connectionString);

            var shareClient = shareServiceClient.GetShareClient(shareName);

            await shareClient.CreateIfNotExistsAsync();

            var directoryClient = shareClient.GetRootDirectoryClient();

            var fileClient = directoryClient.GetFileClient(fileName);

            var file = req.Form.Files["file"];

            if (file == null || file.Length == 0)
            {
                return new BadRequestObjectResult("No file provided or file is empty.");
            }

            using var stream = file.OpenReadStream();

            await fileClient.CreateAsync(stream.Length);

            await fileClient.UploadAsync(stream);

            return new OkObjectResult("File uploaded to Azure Files");
        }
    }
}
