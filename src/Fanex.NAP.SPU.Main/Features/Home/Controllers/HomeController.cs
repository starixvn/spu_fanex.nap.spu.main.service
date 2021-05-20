namespace Fanex.NAP.SPU.Main.Controllers
{
    using Fanex.NAP.SPU.Main.Middleware;
    using Microsoft.AspNetCore.Mvc;
    
    public class HomeController : Controller
    {        
        private readonly IAppSettings _appSettings;
        
        public HomeController(IAppSettings appSettings)
        {                        
            _appSettings = appSettings;            
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult PortainerUrl()
        {
            if (_appSettings.Environment == 1)
            {
                return Redirect("http://portainer.nexdev.net/");
            }

            return Redirect("https://sbportainer.starixplay.com/");

        }
    }
}
