using KisiselFinansTakip.Data;
using KisiselFinansTakip.Models.Siniflar;
using KisiselFinansTakip.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace KisiselFinansTakip.Controllers
{
    public class KategoriController : Controller
    {
        private readonly VeritabaniContext _db;

        public KategoriController(VeritabaniContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Kategori()
        {
            if (HttpContext.Session.GetInt32("KullaniciID") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new KategoriViewModel
            {
                TipListesi = new List<string> { "Gelir", "Gider" },
                Kategoriler = _db.Kategoriler.OrderBy(x => x.Tip).ThenBy(x => x.KategoriAdi).ToList()
            };
            return View(model);
        }

        public IActionResult KategoriGetir(int id)
        {
            if (HttpContext.Session.GetInt32("KullaniciID") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var kategori = _db.Kategoriler.FirstOrDefault(x => x.KategoriID == id);
            if (kategori == null) return NotFound();

            var model = new KategoriViewModel
            {
                KategoriID = kategori.KategoriID,
                KategoriAdi = kategori.KategoriAdi,
                Tip = kategori.Tip,
                TipListesi = new List<string> { "Gelir", "Gider" },
                Kategoriler = _db.Kategoriler.OrderBy(x => x.Tip).ThenBy(x => x.KategoriAdi).ToList()
            };

            return View("Kategori", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult KategoriIslem(KategoriViewModel model, string action)
        {
            if (HttpContext.Session.GetInt32("KullaniciID") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (action == "Ekle")
            {
                if (string.IsNullOrWhiteSpace(model.KategoriAdi) || string.IsNullOrWhiteSpace(model.Tip))
                {
                    TempData["Uyari"] = "Lütfen kategori adı ve türünü eksiksiz giriniz.";
                    return RedirectToAction("Kategori");
                }

                bool ayniVarmi = _db.Kategoriler.Any(k => k.KategoriAdi.ToLower() == model.KategoriAdi.Trim().ToLower() && k.Tip == model.Tip);
                if (ayniVarmi)
                {
                    TempData["Uyari"] = "Bu isimde ve türde bir kategori zaten mevcut.";
                    return RedirectToAction("Kategori");
                }

                var kategori = new Kategori
                {
                    KategoriAdi = model.KategoriAdi.Trim(),
                    Tip = model.Tip.Trim()
                };
                _db.Kategoriler.Add(kategori);
                _db.SaveChanges();

                TempData["Mesaj"] = $"'{kategori.KategoriAdi}' kategorisi başarıyla eklendi.";
            }
            else if (action == "Guncelle")
            {
                var kategori = _db.Kategoriler.FirstOrDefault(x => x.KategoriID == model.KategoriID);
                if (kategori == null) return NotFound();

                if (string.IsNullOrWhiteSpace(model.KategoriAdi) || string.IsNullOrWhiteSpace(model.Tip))
                {
                    TempData["Uyari"] = "Kategori adı ve türü boş bırakılamaz.";
                    return RedirectToAction("Kategori");
                }

                kategori.KategoriAdi = model.KategoriAdi.Trim();
                kategori.Tip = model.Tip.Trim();
                _db.SaveChanges();

                TempData["Mesaj"] = "Kategori başarıyla güncellendi.";
            }
            else if (action == "Sil")
            {
                var kategori = _db.Kategoriler.FirstOrDefault(x => x.KategoriID == model.KategoriID);
                if (kategori == null) return NotFound();

                // Bagli islem kontrolu
                bool islemVar = _db.Islemler.Any(i => i.KategoriID == model.KategoriID);
                if (islemVar)
                {
                    TempData["Uyari"] = "Bu kategoriye ait finansal işlemler bulunduğu için silinemez. Önce ilişkili işlemleri silmelisiniz.";
                    return RedirectToAction("Kategori");
                }

                _db.Kategoriler.Remove(kategori);
                _db.SaveChanges();

                TempData["Mesaj"] = "Kategori başarıyla silindi.";
            }

            return RedirectToAction("Kategori");
        }
    }
}
