using ABCRetail.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetail.Controllers
{
    public class HomeController : Controller
    {
        private readonly FileStorageService _fileStorageService;

        public HomeController(FileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        public async Task<IActionResult> Index()
        {
            await _fileStorageService.WriteLogAsync(
                "application-log.txt",
                $"Application accessed at {DateTime.Now}");

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DownloadLog()
        {
            var file = await _fileStorageService.DownloadLogAsync(
                "application-log.txt");

            if (file == null)
            {
                return NotFound("Log file was not found.");
            }

            return File(
                file,
                "text/plain",
                "application-log.txt");
        }
    }
}