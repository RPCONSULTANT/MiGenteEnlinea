using Microsoft.AspNetCore.Mvc;

namespace MiGenteEnLinea.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
