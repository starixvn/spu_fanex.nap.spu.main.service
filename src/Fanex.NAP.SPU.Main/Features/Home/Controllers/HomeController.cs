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

        public IActionResult DCSUrl()
        {
            if (_appSettings.Environment == 1)
            {
                return Redirect("http://spu-dcs-job.nexdev.net/swagger/index.html");
            }

            return Redirect("https://spudcs.oneops.net/swagger/index.html");

        }

        public IActionResult CTSUrl()
        {
            if (_appSettings.Environment == 1)
            {
                return Redirect("http://spu-cts-job.nexdev.net/swagger/index.html");
            }

            return Redirect("https://spucts.oneops.net/swagger/index.html");

        }

        public IActionResult FSUrl()
        {
            if (_appSettings.Environment == 1)
            {
                return Redirect("http://spu-fs-job.nexdev.net/swagger/index.html");
            }

            return Redirect("https://spufs.oneops.net/swagger/index.html");

        }

        public IActionResult VRUrl()
        {
            if (_appSettings.Environment == 1)
            {
                return Redirect("http://spu-vr-job.nexdev.net/swagger/index.html");
            }

            return Redirect("https://spuvr.oneops.net/swagger/index.html");

        }

        public IActionResult TGRUrl()
        {
            if (_appSettings.Environment == 1)
            {
                return Redirect("http://spu-tgr-job.nexdev.net/swagger/index.html");
            }

            return Redirect("https://sputgr.oneops.net/swagger/index.html");

        }
    }
}
