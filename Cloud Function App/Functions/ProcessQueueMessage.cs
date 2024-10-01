using System.Net;
using Azure.Storage.Queues;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Cloud_Function_App.Functions
{
    public static class ProcessQueueMessage
    {
        [Function("ProcessQueueMessage")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string queueName = "order-processing";

            string message = req.Query["message"];

            string processOrderMessage = $"Processing order {message}";

            if (string.IsNullOrEmpty(queueName) || string.IsNullOrEmpty(message))
            {
                return new BadRequestObjectResult("Queue name and message must be provided.");
            }

            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

            var queueServiceClient = new QueueServiceClient(connectionString);

            var queueClient = queueServiceClient.GetQueueClient(queueName);

            await queueClient.CreateIfNotExistsAsync();

            await queueClient.SendMessageAsync(processOrderMessage);

            return new OkObjectResult("Message added to queue");
        }
    }
}
