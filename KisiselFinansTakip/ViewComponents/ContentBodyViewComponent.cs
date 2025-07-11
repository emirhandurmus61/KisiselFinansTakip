using Microsoft.AspNetCore.Mvc;

namespace KisiselFinansTakip.ViewComponents
{
    public class ContentBodyViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
