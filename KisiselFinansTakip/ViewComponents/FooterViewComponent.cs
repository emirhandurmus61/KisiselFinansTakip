using Microsoft.AspNetCore.Mvc;

namespace KisiselFinansTakip.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
