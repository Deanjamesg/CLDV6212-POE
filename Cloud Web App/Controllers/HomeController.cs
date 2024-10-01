using Cloud_Web_App.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Cloud_Web_App.Services;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;


namespace Cloud_Web_App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly BlobService _blobService;

        private readonly TableService _tableService;

        private readonly QueueService _queueService;

        private readonly FileService _fileService;

        private readonly HttpClient _httpClient;

        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(BlobService blobService, TableService tableService, QueueService queueService, FileService fileService, HttpClient httpClient, IHttpClientFactory httpClientFactory)
        {
            _blobService = blobService;
            _tableService = tableService;
            _queueService = queueService;
            _fileService = fileService;

            _httpClient = httpClient;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Home()
        {
            return View();
        }

        public IActionResult Customer()
        {
            return View();
        }

        public IActionResult Product()
        {
            return View();
        }

        public IActionResult Order()
        {
            return View();
        }

        public IActionResult Contract()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file != null)
            {
                var functionUrl = "https://st10378305functions.azurewebsites.net/api/UploadBlob?code=g7V6y2RAxCatZMEUIe8vKy0rIns3ZrQ-hdqTnKsmkWzoAzFuwElRxQ%3D%3D";

                var stream = file.OpenReadStream();

                var content = new MultipartFormDataContent();

                content.Add(new StreamContent(stream), "file", file.FileName);

                var client = _httpClientFactory.CreateClient();

                var response = await client.PostAsync($"{functionUrl}&blobName={file.FileName}", content);

            }

            return RedirectToAction("Order");
        }

        //[HttpPost]
        //public async Task<IActionResult> UploadImage(IFormFile file)
        //{
        //    if (file != null)
        //    {
        //        using var stream = file.OpenReadStream();
        //        await _blobService.UploadBlobAsync("product-images", file.FileName, stream);
        //    }
        //    return RedirectToAction("Home");
        //}

        [HttpPost]
        public async Task<IActionResult> AddCustomerProfile(CustomerProfile profile)
        {
            if (ModelState.IsValid)
            {
                await _tableService.AddEntityAsync(profile);
            }
            return RedirectToAction("Home");
        }

        [HttpPost]
        public async Task<IActionResult> ProcessOrder(string orderId)
        {
            await _queueService.SendMessageAsync("order-processing", $"Processing order {orderId}");
            return RedirectToAction("Home");
        }

        [HttpPost]
        public async Task<IActionResult> UploadContract(IFormFile file)
        {
            if (file != null)
            {
                var functionUrl = "https://st10378305functions.azurewebsites.net/api/UploadFile?code=ekMqNwAbyUD3EArnJeMPMRXHgZCwxT78CkJDzPbdMaBIAzFu6CrMzw%3D%3D";

                var stream = file.OpenReadStream();

                var content = new MultipartFormDataContent();

                content.Add(new StreamContent(stream), "file", file.FileName);

                var client = _httpClientFactory.CreateClient();

                var response = await client.PostAsync($"{functionUrl}&fileName={file.FileName}", content);
            }

            return RedirectToAction("Home");
        }

        //[HttpPost]
        //public async Task<IActionResult> UploadContract(IFormFile file)
        //{
        //    if (file != null)
        //    {
        //        using var stream = file.OpenReadStream();
        //        await _fileService.UploadFileAsync("contracts-logs", file.FileName, stream);
        //    }
        //    return RedirectToAction("Home");
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
