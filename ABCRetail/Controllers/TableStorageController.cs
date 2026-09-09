using Microsoft.AspNetCore.Mvc;

namespace ABCRetail.Controllers
{
    public class TableStorageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
