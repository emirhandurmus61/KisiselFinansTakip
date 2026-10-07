using KisiselFinansTakip.Data;
using KisiselFinansTakip.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace KisiselFinansTakip.Controllers
{
    public class ButceHedeflerimController : Controller
    {
        private readonly VeritabaniContext _db;
        private readonly KisiselFinansTakip.Services.FinansAsistaniService _asistanService;

        public ButceHedeflerimController(VeritabaniContext db, KisiselFinansTakip.Services.FinansAsistaniService asistanService)
        {
            _db = db;
            _asistanService = asistanService;
        }

        public IActionResult ButceHedeflerim(int? ayFiltre = null, int? yilFiltre = null)
        {
            int? kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
            if (kullaniciId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int secilenAy = ayFiltre ?? DateTime.Now.Month;
            int secilenYil = yilFiltre ?? DateTime.Now.Year;

            var hedefler = _db.ButceHedefleri
                .Include(b => b.Kategori)
                .Where(b => b.KullaniciID == kullaniciId.Value && b.Ay == secilenAy && b.Yil == secilenYil)
                .Select(b => new ButceHedefleriViewModel
                {
                    HedefID = b.HedefID,
                    KategoriID = b.KategoriID,
                    KategoriAdi = b.Kategori.KategoriAdi,
                    KategoriTip = b.Kategori.Tip,
                    HedefTutar = b.HedefTutar,
                    Ay = b.Ay,
                    Yil = b.Yil,
                    ToplamHarcama = _db.Islemler
                        .Where(i => i.KullaniciID == kullaniciId.Value
                                    && i.KategoriID == b.KategoriID
                                    && i.Tarih.Month == b.Ay
                                    && i.Tarih.Year == b.Yil)
                        .Sum(i => (decimal?)i.Tutar) ?? 0m
                })
                .ToList();

            ViewBag.SecilenAy = secilenAy;
            ViewBag.SecilenYil = secilenYil;

            decimal toplamHedeflenen = hedefler.Where(h => h.KategoriTip == "Gider").Sum(h => h.HedefTutar);
            decimal toplamHarcanan = hedefler.Where(h => h.KategoriTip == "Gider").Sum(h => h.ToplamHarcama);
            decimal kalanButce = toplamHedeflenen - toplamHarcanan;

            ViewBag.ToplamHedeflenen = toplamHedeflenen;
            ViewBag.ToplamHarcanan = toplamHarcanan;
            ViewBag.KalanButce = kalanButce;

            // Akıllı Asistan Bildirimleri ve İçgörüleri
            var icgoruler = _asistanService.GetButceHedefIcgoruListesi(kullaniciId.Value, secilenAy, secilenYil, hedefler);
            ViewBag.Icgoruler = icgoruler;

            return View(hedefler);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Sil(int id)
        {
            int? kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
            if (kullaniciId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var hedef = _db.ButceHedefleri
                .FirstOrDefault(b => b.HedefID == id && b.KullaniciID == kullaniciId.Value);

            if (hedef != null)
            {
                _db.ButceHedefleri.Remove(hedef);
                _db.SaveChanges();
                TempData["Mesaj"] = "Bütçe hedefi başarıyla silindi.";
            }
            else
            {
                TempData["HataMesaji"] = "Hedef bulunamadı veya yetkiniz yok.";
            }

            return RedirectToAction("ButceHedeflerim");
        }
    }
}
