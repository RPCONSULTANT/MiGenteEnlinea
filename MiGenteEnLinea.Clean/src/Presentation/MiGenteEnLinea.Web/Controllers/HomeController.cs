using Microsoft.AspNetCore.Mvc;

namespace MiGenteEnLinea.Web.Controllers;

/// <summary>
/// Controller for public landing pages
/// </summary>
public class HomeController : Controller
{
    /// <summary>
    /// Landing page - Main public home page
    /// </summary>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Planes - Subscription plans page
    /// </summary>
    public IActionResult Planes()
    {
        return View();
    }

    /// <summary>
    /// FAQ - Frequently asked questions
    /// </summary>
    public IActionResult FAQ()
    {
        return View();
    }

    /// <summary>
    /// Terminos - Terms and conditions
    /// </summary>
    public IActionResult Terminos()
    {
        return View();
    }

    /// <summary>
    /// Error page handler
    /// </summary>
    public IActionResult Error()
    {
        return View();
    }
}
