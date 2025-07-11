using Microsoft.AspNetCore.Mvc;

namespace KisiselFinansTakip.ViewComponents
{
    public class PreloaderViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
