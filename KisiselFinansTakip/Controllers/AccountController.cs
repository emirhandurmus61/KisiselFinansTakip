using KisiselFinansTakip.Data;
using KisiselFinansTakip.Models.Siniflar;
using KisiselFinansTakip.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace KisiselFinansTakip.Controllers
{
    public class AccountController : Controller
    {
        private readonly VeritabaniContext _db;
        public AccountController(VeritabaniContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var kullanici = _db.Kullanicilar
                    .FirstOrDefault(x => x.Email == model.Email && x.Sifre == model.Sifre);

                if (kullanici != null)
                {
                    // Session bilgisi ekle
                    HttpContext.Session.SetInt32("KullaniciID", kullanici.KullaniciID);
                    HttpContext.Session.SetString("KullaniciAdi", kullanici.Ad + " " + kullanici.Soyad);

                    return RedirectToAction("Index", "Default");
                }
                else
                {
                    ModelState.AddModelError("", "Email veya şifre yanlış.");
                }
            }
            return View(model);
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool emailVarmi = _db.Kullanicilar.Any(x => x.Email == model.Email);
                if (emailVarmi)
                {
                    ModelState.AddModelError("Email", "Bu email zaten kayıtlı");
                    return View(model);
                }

                var kullanici = new Kullanici
                {
                    Ad = model.Ad,
                    Soyad = model.Soyad,
                    Email = model.Email,
                    Sifre = model.Sifre,
                    Rol = "Kullanici",
                    KayitTarihi = DateTime.Now,
                };

                _db.Kullanicilar.Add(kullanici);
                _db.SaveChanges();
            }

            return View(model);
        }
    }
}
