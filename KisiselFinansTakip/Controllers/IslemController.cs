using KisiselFinansTakip.Data;
using KisiselFinansTakip.Models.Siniflar;
using KisiselFinansTakip.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace KisiselFinansTakip.Controllers
{
    public class IslemController : Controller
    {
        private readonly VeritabaniContext _db;

        public IslemController(VeritabaniContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Islem()
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
            if (kullaniciId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new IslemViewModel
            {
                Tip = "Gider", // Varsayilan olarak gider secili gelsin
                Kategoriler = _db.Kategoriler.Where(k => k.Tip == "Gider").OrderBy(k => k.KategoriAdi).ToList(),
                Tarih = DateTime.Today
            };

            // Kullanicinin son 5 islemini de gosterim icin tasiyalim
            ViewBag.SonIslemler = _db.Islemler
                .Include(i => i.Kategori)
                .Where(i => i.KullaniciID == kullaniciId.Value)
                .OrderByDescending(i => i.Tarih)
                .Take(5)
                .ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Islem(IslemViewModel model)
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
            if (kullaniciId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                model.Kategoriler = _db.Kategoriler
                    .Where(k => k.Tip == model.Tip)
                    .OrderBy(k => k.KategoriAdi)
                    .ToList();

                ViewBag.SonIslemler = _db.Islemler
                    .Include(i => i.Kategori)
                    .Where(i => i.KullaniciID == kullaniciId.Value)
                    .OrderByDescending(i => i.Tarih)
                    .Take(5)
                    .ToList();

                return View(model);
            }

            var islem = new Islem
            {
                Aciklama = model.Aciklama.Trim(),
                Tutar = model.Tutar,
                Tarih = model.Tarih,
                KategoriID = model.KategoriID,
                KullaniciID = kullaniciId.Value
            };

            _db.Islemler.Add(islem);
            _db.SaveChanges();

            TempData["IslemMesaj"] = $"{model.Tip} işlemi ({model.Tutar:N2} ₺) başarıyla kaydedildi.";
            return RedirectToAction("Islem");
        }

        [HttpGet]
        public JsonResult KategorileriGetir(string tip)
        {
            var kategoriler = _db.Kategoriler
                .Where(k => k.Tip == tip)
                .OrderBy(k => k.KategoriAdi)
                .Select(k => new { k.KategoriID, k.KategoriAdi })
                .ToList();

            return Json(kategoriler);
        }
    }
}
