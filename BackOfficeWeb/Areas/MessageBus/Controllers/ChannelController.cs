using Microsoft.AspNetCore.Mvc;

namespace BackOfficeWeb.Areas.MessageBus.Controllers
{
    [Area("MessageBus")]
    public class ChannelController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Show()
        {
            return View();
        }

        public IActionResult Edit()
        {
            return View();
        }

        public IActionResult Delete()
        {
            return View();
        }
    }
}
