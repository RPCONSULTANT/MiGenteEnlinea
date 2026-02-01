using Microsoft.AspNetCore.Mvc;

namespace MiGenteEnLinea.Web.Controllers;

/// <summary>
/// Controller for authentication pages (Login, Register, Activate, etc.)
/// </summary>
public class AuthController : Controller
{
    /// <summary>
    /// Login page
    /// </summary>
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    /// <summary>
    /// Login POST handler
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(string email, string password, string? returnUrl = null)
    {
        // TODO: Implement login logic via API
        // For now, redirect to appropriate dashboard based on user type
        return RedirectToAction("Index", "Empleador");
    }

    /// <summary>
    /// Register page
    /// </summary>
    public IActionResult Registrar()
    {
        return View();
    }

    /// <summary>
    /// Register POST handler
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Registrar(string tipo, string nombre, string apellido, string email, string telefono1, string telefono2, string usuario)
    {
        // TODO: Implement registration logic via API
        TempData["Message"] = "Cuenta creada exitosamente. Por favor revise su correo para activar su cuenta.";
        return RedirectToAction("Login");
    }

    /// <summary>
    /// Activate account page
    /// </summary>
    public IActionResult Activar(string? userId, string? email)
    {
        ViewData["Email"] = email;
        ViewData["UserId"] = userId;
        return View();
    }

    /// <summary>
    /// Activate account POST handler
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Activar(string email, string password, string confirmPassword)
    {
        // TODO: Implement activation logic via API
        TempData["Message"] = "Cuenta activada exitosamente. Ya puede iniciar sesión.";
        return RedirectToAction("Login");
    }

    /// <summary>
    /// Forgot password page
    /// </summary>
    public IActionResult RecuperarPassword()
    {
        return View();
    }

    /// <summary>
    /// Forgot password POST handler
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RecuperarPassword(string email)
    {
        // TODO: Implement password recovery via API
        TempData["Message"] = "Se ha enviado un correo con instrucciones para recuperar su contraseña.";
        return RedirectToAction("Login");
    }

    /// <summary>
    /// Reset password page
    /// </summary>
    public IActionResult ResetPassword(string? token, string? email)
    {
        ViewData["Token"] = token;
        ViewData["Email"] = email;
        return View();
    }

    /// <summary>
    /// Logout handler
    /// </summary>
    public IActionResult Logout()
    {
        // TODO: Clear authentication cookies/tokens
        return RedirectToAction("Login");
    }
}
