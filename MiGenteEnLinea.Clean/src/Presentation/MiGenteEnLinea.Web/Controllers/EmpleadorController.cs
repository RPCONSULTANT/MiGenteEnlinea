using Microsoft.AspNetCore.Mvc;

namespace MiGenteEnLinea.Web.Controllers;

/// <summary>
/// Controller for Empleador (Employer) dashboard pages
/// </summary>
public class EmpleadorController : Controller
{
    /// <summary>
    /// Empleador dashboard index
    /// </summary>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Empleados list - Employee management
    /// </summary>
    public IActionResult Empleados()
    {
        return View();
    }

    /// <summary>
    /// Ficha Empleado - Employee details/profile
    /// </summary>
    public IActionResult FichaEmpleado(int? id)
    {
        ViewData["EmpleadoId"] = id;
        return View();
    }

    /// <summary>
    /// Nomina - Payroll management
    /// </summary>
    public IActionResult Nomina()
    {
        return View();
    }

    /// <summary>
    /// Contrataciones - Temporary contracts
    /// </summary>
    public IActionResult Contrataciones()
    {
        return View();
    }

    /// <summary>
    /// Perfil - Employer profile
    /// </summary>
    public IActionResult Perfil()
    {
        return View();
    }

    /// <summary>
    /// MiSuscripcion - Current subscription
    /// </summary>
    public IActionResult MiSuscripcion()
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
    /// Calificaciones - Rate contractors
    /// </summary>
    public IActionResult Calificaciones()
    {
        return View();
    }

    /// <summary>
    /// Abogado Virtual - AI Legal Assistant
    /// </summary>
    public IActionResult AbogadoVirtual()
    {
        return View();
    }

    /// <summary>
    /// FAQ - Frequently Asked Questions
    /// </summary>
    public IActionResult FAQ()
    {
        return View();
    }

    /// <summary>
    /// Buscador - Search for contractors/professionals
    /// </summary>
    public IActionResult Buscador()
    {
        return View();
    }
}
