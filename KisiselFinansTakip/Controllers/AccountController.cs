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
            // Eger kullanici zaten oturum acmissa ana panele yonlendir
            if (HttpContext.Session.GetInt32("KullaniciID") != null)
            {
                return RedirectToAction("Index", "Default");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                string temizEmail = (model.Email ?? "").Trim().ToLower();
                var kullanici = _db.Kullanicilar
                    .FirstOrDefault(x => x.Email.ToLower() == temizEmail && x.Sifre == model.Sifre);

                if (kullanici != null)
                {
                    // Session bilgilerini ayarla
                    HttpContext.Session.SetInt32("KullaniciID", kullanici.KullaniciID);
                    HttpContext.Session.SetString("KullaniciAdi", $"{kullanici.Ad} {kullanici.Soyad}");
                    HttpContext.Session.SetString("Email", kullanici.Email);
                    HttpContext.Session.SetString("Rol", kullanici.Rol ?? "Kullanici");

                    return RedirectToAction("Index", "Default");
                }
                else
                {
                    ModelState.AddModelError("", "E-posta veya şifre hatalı. Lütfen bilgilerinizi kontrol ediniz.");
                }
            }

            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["BasariMesaji"] = "Başarıyla çıkış yaptınız.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (HttpContext.Session.GetInt32("KullaniciID") != null)
            {
                return RedirectToAction("Index", "Default");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                string temizEmail = (model.Email ?? "").Trim().ToLower();

                bool emailVarmi = _db.Kullanicilar.Any(x => x.Email.ToLower() == temizEmail);
                if (emailVarmi)
                {
                    ModelState.AddModelError("Email", "Bu e-posta adresi ile zaten kayıtlı bir hesap bulunmaktadır.");
                    return View(model);
                }

                var kullanici = new Kullanici
                {
                    Ad = model.Ad.Trim(),
                    Soyad = model.Soyad.Trim(),
                    Email = temizEmail,
                    Sifre = model.Sifre,
                    Rol = "Kullanici",
                    KayitTarihi = DateTime.UtcNow
                };

                _db.Kullanicilar.Add(kullanici);
                _db.SaveChanges();

                TempData["BasariMesaji"] = "Hesabınız başarıyla oluşturuldu! Şimdi giriş yapabilirsiniz.";
                return RedirectToAction("Login");
            }

            return View(model);
        }
    }
}
