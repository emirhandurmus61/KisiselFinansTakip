using Microsoft.AspNetCore.Mvc;

namespace KisiselFinansTakip.Controllers
{
    public class ProfilController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
