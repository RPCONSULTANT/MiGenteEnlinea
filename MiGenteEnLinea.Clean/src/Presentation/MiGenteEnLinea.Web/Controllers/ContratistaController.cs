using Microsoft.AspNetCore.Mvc;

namespace MiGenteEnLinea.Web.Controllers;

/// <summary>
/// Controller for Contratista (Contractor) dashboard pages
/// </summary>
public class ContratistaController : Controller
{
    /// <summary>
    /// Contratista dashboard index - Profile page
    /// </summary>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// MisCalificaciones - View received ratings
    /// </summary>
    public IActionResult MisCalificaciones()
    {
        return View();
    }

    /// <summary>
    /// Perfil - Edit profile
    /// </summary>
    public IActionResult Perfil()
    {
        return View();
    }

    /// <summary>
    /// Suscripciones - Subscription management
    /// </summary>
    public IActionResult Suscripciones()
    {
        return View();
    }

    /// <summary>
    /// AdquirirPlan - Purchase subscription plan
    /// </summary>
    public IActionResult AdquirirPlan()
    {
        return View();
    }

    /// <summary>
    /// Checkout - Payment page
    /// </summary>
    public IActionResult Checkout(int? planId)
    {
        ViewData["PlanId"] = planId;
        return View();
    }

    /// <summary>
    /// Directorio - Directory of employers/jobs
    /// </summary>
    public IActionResult Directorio()
    {
        return View();
    }
}
