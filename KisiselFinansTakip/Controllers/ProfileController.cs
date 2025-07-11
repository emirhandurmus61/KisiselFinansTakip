using KisiselFinansTakip.Data;
using KisiselFinansTakip.Models.Siniflar;
using Microsoft.AspNetCore.Mvc;

namespace KisiselFinansTakip.Controllers
{
    public class ProfileController : Controller
    {
        private readonly VeritabaniContext _db;
        public ProfileController(VeritabaniContext db)
        {
            _db = db;
        }
        public IActionResult Profile()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ProfilGuncelle(Kullanici model)
        {
            if (!ModelState.IsValid)
            {
                return View("Ayarlar", model); // veya hangi view'sa
            }

            var kullanici = _db.Kullanicilar.FirstOrDefault(x => x.KullaniciID == model.KullaniciID);
            if (kullanici == null)
                return NotFound();

            kullanici.Ad = model.Ad;
            kullanici.Soyad = model.Soyad;
            kullanici.Email = model.Email;
            kullanici.Sifre = model.Sifre;

            _db.SaveChanges();

            TempData["Mesaj"] = "Profil başarıyla güncellendi.";
            return RedirectToAction("Ayarlar");
        }

    }
}
