using KisiselFinansTakip.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace KisiselFinansTakip.Controllers
{
    public class IslemlerimController : Controller
    {
        private readonly VeritabaniContext _db;

        public IslemlerimController(VeritabaniContext db)
        {
            _db = db;
        }

        public IActionResult Islemler(string tipFiltre = null, int? kategoriFiltre = null)
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
            if (kullaniciId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var query = _db.Islemler
                .Include(i => i.Kategori)
                .Where(i => i.KullaniciID == kullaniciId.Value);

            if (!string.IsNullOrEmpty(tipFiltre))
            {
                query = query.Where(i => i.Kategori.Tip == tipFiltre);
            }

            if (kategoriFiltre.HasValue && kategoriFiltre.Value > 0)
            {
                query = query.Where(i => i.KategoriID == kategoriFiltre.Value);
            }

            var islemler = query.OrderByDescending(i => i.Tarih).ToList();

            // Ozet hesaplamalari (Filtresiz tum islemler uzerinden)
            var tumIslemler = _db.Islemler
                .Include(i => i.Kategori)
                .Where(i => i.KullaniciID == kullaniciId.Value)
                .ToList();

            ViewBag.ToplamGelir = tumIslemler.Where(i => i.Kategori.Tip == "Gelir").Sum(i => i.Tutar);
            ViewBag.ToplamGider = tumIslemler.Where(i => i.Kategori.Tip == "Gider").Sum(i => i.Tutar);
            ViewBag.NetBakiye = (decimal)ViewBag.ToplamGelir - (decimal)ViewBag.ToplamGider;
            ViewBag.Kategoriler = _db.Kategoriler.OrderBy(k => k.KategoriAdi).ToList();
            ViewBag.SecilenTip = tipFiltre;
            ViewBag.SecilenKategori = kategoriFiltre;

            return View(islemler);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Sil(int id)
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
            if (kullaniciId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var islem = _db.Islemler.FirstOrDefault(i => i.IslemID == id && i.KullaniciID == kullaniciId.Value);
            if (islem == null)
            {
                TempData["HataMesaji"] = "İşlem bulunamadı veya silme yetkiniz yok.";
                return RedirectToAction("Islemler");
            }

            _db.Islemler.Remove(islem);
            _db.SaveChanges();

            TempData["Mesaj"] = "İşlem kaydı başarıyla silindi.";
            return RedirectToAction("Islemler");
        }
    }
}
