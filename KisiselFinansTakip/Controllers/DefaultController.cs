using Microsoft.AspNetCore.Mvc;

namespace KisiselFinansTakip.Controllers
{
	public class DefaultController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
