using System.Collections.Concurrent;
using System.Net;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Cloud_Function_App.Functions
{
    public static class StoreTableInfo
    {
        [Function("StoreTableInfo")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string tableName = "CustomerProfiles";

            string partitionKey = "CustomerProfile";

            string rowKey = Guid.NewGuid().ToString();

            var formdata = await req.ReadFormAsync();

            string firstName = formdata["FirstName"];

            string lastName = formdata["LastName"];

            string email = formdata["Email"];

            string phoneNumber = formdata["PhoneNumber"];

            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

            var serviceClient = new TableServiceClient(connectionString);

            var tableClient = serviceClient.GetTableClient(tableName);

            await tableClient.CreateIfNotExistsAsync();

            var entity = new TableEntity(partitionKey, rowKey)
            {
                ["FirstName"] = firstName,
                ["LastName"] = lastName,
                ["Email"] = email,
                ["PhoneNumber"] = phoneNumber
            };

            await tableClient.AddEntityAsync(entity);

            return new OkObjectResult("Data added to table");
        }
    }
}
