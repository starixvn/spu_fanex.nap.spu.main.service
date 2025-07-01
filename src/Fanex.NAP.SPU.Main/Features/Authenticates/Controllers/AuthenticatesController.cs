namespace Fanex.NAP.SPU.Main.Features.Authenticates.Controllers
{
    using Fanex.NAP.SPU.Main.Middleware;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class AuthenticatesController : Controller
    {
        private readonly IAppSettings _appSettings;        

        public AuthenticatesController(IAppSettings appSettings)
        {
            _appSettings = appSettings;
        }

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
            if (CheckValidAccount(username, password))
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

        private bool CheckValidAccount(string username, string password)
        {
            var isValid = false;
         
            switch (_appSettings.Environment)
            {
                case 3: // PROD
                case 5: // KLDR
                    if (username == "admin" && password == "admin!Q@W#E")
                        isValid = true;
                        break;
                default:
                    if (username == "admin" && password == "admin")
                        isValid = true;
                    break;
            }

            return isValid;
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
