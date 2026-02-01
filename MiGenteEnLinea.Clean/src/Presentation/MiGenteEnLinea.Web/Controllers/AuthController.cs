using Microsoft.AspNetCore.Mvc;

namespace MiGenteEnLinea.Web.Controllers;

/// <summary>
/// Controller for authentication pages (Login, Register, Activate, etc.)
/// All actual authentication logic is handled client-side via JavaScript calling the API.
/// This controller only serves the views.
/// </summary>
public class AuthController : Controller
{
    /// <summary>
    /// Login page
    /// </summary>
    public IActionResult Login(string? returnUrl = null, bool logout = false)
    {
        ViewData["ReturnUrl"] = returnUrl;
        
        if (logout)
        {
            // Clear any server-side session if exists
            HttpContext.Session.Clear();
        }
        
        return View();
    }

    /// <summary>
    /// Login POST handler - Redirects to client-side handling
    /// The actual login is handled via JavaScript API calls in the view
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(string email, string password, string? returnUrl = null)
    {
        // This should not be called since we use JavaScript for login
        // Redirect back to login page to use client-side API
        return RedirectToAction("Login", new { returnUrl });
    }

    /// <summary>
    /// Register page
    /// </summary>
    public IActionResult Registrar()
    {
        return View();
    }

    /// <summary>
    /// Register POST handler - Redirects to client-side handling
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Registrar(string tipo, string nombre, string apellido, string email, string telefono1, string telefono2, string usuario)
    {
        // This should not be called since we use JavaScript for registration
        return RedirectToAction("Registrar");
    }

    /// <summary>
    /// Activate account page
    /// </summary>
    public IActionResult Activar(string? userId, string? email)
    {
        ViewData["Email"] = email ?? "";
        ViewData["UserId"] = userId ?? "";
        return View();
    }

    /// <summary>
    /// Activate account POST handler - Redirects to client-side handling
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Activar(string email, string password, string confirmPassword)
    {
        // This should not be called since we use JavaScript for activation
        return RedirectToAction("Activar");
    }

    /// <summary>
    /// Forgot password page
    /// </summary>
    public IActionResult RecuperarPassword()
    {
        return View();
    }

    /// <summary>
    /// Forgot password POST handler - Redirects to client-side handling
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RecuperarPassword(string email)
    {
        // This should not be called since we use JavaScript
        TempData["Message"] = "Si el correo existe, recibirá instrucciones para recuperar su contraseña.";
        return RedirectToAction("Login");
    }

    /// <summary>
    /// Reset password page
    /// </summary>
    public IActionResult ResetPassword(string? token, string? email)
    {
        ViewData["Token"] = token ?? "";
        ViewData["Email"] = email ?? "";
        return View();
    }

    /// <summary>
    /// Logout handler
    /// Clears server-side session and redirects to login with logout flag
    /// Client-side localStorage is cleared by JavaScript in the login page
    /// </summary>
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", new { logout = true });
    }
}
