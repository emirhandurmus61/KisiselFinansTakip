using Microsoft.AspNetCore.Mvc;

namespace KisiselFinansTakip.ViewComponents
{
    public class HeadViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
