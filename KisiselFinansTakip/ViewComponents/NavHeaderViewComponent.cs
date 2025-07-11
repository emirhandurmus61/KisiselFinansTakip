using Microsoft.AspNetCore.Mvc;

namespace KisiselFinansTakip.ViewComponents
{
    public class NavHeaderViewComponent:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
