using Microsoft.AspNetCore.Mvc;

namespace KisiselFinansTakip.Controllers
{
    public class DefaultController : Controller
    {
        public IActionResult Index()
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
            if (kullaniciId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }
    }
}
