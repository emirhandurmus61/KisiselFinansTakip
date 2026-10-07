using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using KisiselFinansTakip.Models.Siniflar;
using System.Linq;
using KisiselFinansTakip.Data;
using KisiselFinansTakip.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;

namespace KisiselFinansTakip.Controllers
{
    public class ButceHedefleriController : Controller
    {
        private readonly VeritabaniContext _db;

        public ButceHedefleriController(VeritabaniContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult ButceHedefleri()
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
            if (kullaniciId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new ButceHedefEkleViewModel
            {
                Ay = DateTime.Now.Month,
                Yil = DateTime.Now.Year
            };

            ViewBag.KategoriTip = "Gider";
            ViewBag.Kategoriler = new SelectList(
                _db.Kategoriler.Where(k => k.Tip == "Gider").OrderBy(k => k.KategoriAdi).ToList(),
                "KategoriID", "KategoriAdi"
            );

            // Mevcut hedefleri de sayfada listelemek icin tasiyalim
            ViewBag.MevcutHedefler = _db.ButceHedefleri
                .Include(b => b.Kategori)
                .Where(b => b.KullaniciID == kullaniciId.Value)
                .OrderByDescending(b => b.Yil)
                .ThenByDescending(b => b.Ay)
                .Take(10)
                .ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Ekle(ButceHedefEkleViewModel model)
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
            if (kullaniciId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Kategoriler = new SelectList(
                    _db.Kategoriler.Where(k => k.Tip == model.KategoriTip).OrderBy(k => k.KategoriAdi).ToList(),
                    "KategoriID", "KategoriAdi"
                );

                ViewBag.MevcutHedefler = _db.ButceHedefleri
                    .Include(b => b.Kategori)
                    .Where(b => b.KullaniciID == kullaniciId.Value)
                    .OrderByDescending(b => b.Yil)
                    .ThenByDescending(b => b.Ay)
                    .Take(10)
                    .ToList();

                return View("ButceHedefleri", model);
            }

            // Ayni kullanici, yil, ay ve kategori icin hedef var mi kontrol et
            var mevcutHedef = _db.ButceHedefleri
                .FirstOrDefault(b => b.KullaniciID == kullaniciId.Value 
                                     && b.KategoriID == model.KategoriID 
                                     && b.Ay == model.Ay 
                                     && b.Yil == model.Yil);

            if (mevcutHedef != null)
            {
                mevcutHedef.HedefTutar = model.HedefTutar;
                TempData["BasariMesaji"] = "Mevcut bütçe hedefi başarıyla güncellendi.";
            }
            else
            {
                var hedef = new ButceHedefleri
                {
                    KullaniciID = kullaniciId.Value,
                    KategoriID = model.KategoriID,
                    Ay = model.Ay,
                    Yil = model.Yil,
                    HedefTutar = model.HedefTutar
                };
                _db.ButceHedefleri.Add(hedef);
                TempData["BasariMesaji"] = "Yeni bütçe hedefi başarıyla kaydedildi.";
            }

            _db.SaveChanges();
            return RedirectToAction("ButceHedefleri");
        }

        [HttpGet]
        public JsonResult KategorileriGetir(string tip)
        {
            var kategoriler = _db.Kategoriler
                .Where(k => k.Tip == tip)
                .OrderBy(k => k.KategoriAdi)
                .Select(k => new
                {
                    kategoriID = k.KategoriID,
                    kategoriAdi = k.KategoriAdi
                })
                .ToList();

            return Json(kategoriler);
        }
    }
}
