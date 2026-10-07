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

        [HttpGet]
        public IActionResult Profile()
        {
            // Giriş yapan kullanıcıyı session'dan al
            int? kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
            if (kullaniciId == null)
                return RedirectToAction("Login", "Account");

            var kullanici = _db.Kullanicilar.FirstOrDefault(x => x.KullaniciID == kullaniciId);
            if (kullanici == null)
                return NotFound();

            return View(kullanici);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ProfilGuncelle(Kullanici model)
        {
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    Console.WriteLine($"Hata - Alan: {state.Key}, Mesaj: {error.ErrorMessage}");
                }
            }

            //if (!ModelState.IsValid)
            //{
            //    TempData["Hata"] = "Lütfen geçerli bilgiler giriniz.";
            //    return View("Profile", model);
            //}

            // Veritabanındaki mevcut kullanıcıyı al
            var kullanici = _db.Kullanicilar.FirstOrDefault(x => x.KullaniciID == model.KullaniciID);
            if (kullanici == null)
            {
                TempData["Hata"] = "Kullanıcı bulunamadı.";
                return View("Profile", model);
            }

            // Şifre boşsa güncelleme yapma
            if (string.IsNullOrWhiteSpace(model.Sifre))
            {
                TempData["Hata"] = "Şifre boş olamaz.";
                return View("Profile", model);
            }

            // Girilen şifre mevcut şifreyle eşleşiyor mu?
            if (model.Sifre != kullanici.Sifre)
            {
                TempData["Hata"] = "Şifreniz hatalı, bilgileri güncelleyemezsiniz.";
                return View("Profile", model);
            }

            // Şifre doğruysa bilgileri güncelle
            kullanici.Ad = model.Ad;
            kullanici.Soyad = model.Soyad;
            kullanici.Email = model.Email;
            // Şifre değiştirme yok, aynı kalacak (istersen yeni şifre için alan eklenebilir)

            _db.SaveChanges();

            TempData["Mesaj"] = "Profil başarıyla güncellendi.";
            return RedirectToAction("Profile");
        }
    }
}
