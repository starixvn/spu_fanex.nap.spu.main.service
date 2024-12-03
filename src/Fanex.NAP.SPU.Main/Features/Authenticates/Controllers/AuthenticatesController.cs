namespace Fanex.NAP.SPU.Main.Features.Authenticates.Controllers
{   
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json.Linq;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class AuthenticatesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string returnUrl = null)
        {
            // Validate admin credentials (replace with actual logic)
            if (username == "admin" && password == "admin")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, username),
                    new Claim(ClaimTypes.Name, password),
                    new Claim(ClaimTypes.Role, "Admin")
                };

                var identity = new ClaimsIdentity(claims, "AdminScheme");
                var principal = new ClaimsPrincipal(identity);
                var authProperties = new AuthenticationProperties()
                {
                    IsPersistent = false
                };
                await HttpContext.SignInAsync("AdminScheme", principal, authProperties);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Error = "Invalid username or password";

            return View();
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "AdminScheme")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AdminScheme");

            return Redirect("/quartz/Home");
        }
    }
}
