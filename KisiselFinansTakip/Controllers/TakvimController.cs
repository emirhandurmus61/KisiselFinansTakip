using KisiselFinansTakip.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace KisiselFinansTakip.Controllers
{
    public class TakvimController : Controller
    {
        private readonly VeritabaniContext _db;

        public TakvimController(VeritabaniContext db)
        {
            _db = db;
        }

        public IActionResult Takvim()
        {
            if (HttpContext.Session.GetInt32("KullaniciID") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Kategoriler = _db.Kategoriler.OrderBy(k => k.KategoriAdi).ToList();
            return View();
        }

        // Tüm gelir/gider ve bütçe hedeflerini döndürür
        [HttpGet]
        public JsonResult GetEvents()
        {
            if (HttpContext.Session.GetInt32("KullaniciID") == null)
                return Json(new List<object>()); // Giriş yapılmamışsa boş liste döner

            int kullaniciId = HttpContext.Session.GetInt32("KullaniciID").Value;

            var islemlerList = _db.Islemler
                .Include(i => i.Kategori)
                .Where(i => i.KullaniciID == kullaniciId)
                .Select(i => new
                {
                    id = "islem-" + i.IslemID,
                    islemId = i.IslemID,
                    title = (i.Kategori.Tip == "Gelir" ? "+ " : "- ") + i.Tutar.ToString("N0") + " ₺ (" + (i.Aciklama ?? i.Kategori.KategoriAdi) + ")",
                    start = i.Tarih.ToString("yyyy-MM-dd"),
                    allDay = true,
                    color = i.Kategori.Tip == "Gelir" ? "#28a745" : "#dc3545",
                    textColor = "#ffffff",
                    aciklama = i.Aciklama,
                    kategoriAdi = i.Kategori.KategoriAdi,
                    tutar = i.Tutar.ToString("N2"),
                    tutarRaw = i.Tutar,
                    tip = i.Kategori.Tip,
                    tarihFormatli = i.Tarih.ToString("dd.MM.yyyy")
                })
                .ToList();

            var butceHedefleriList = _db.ButceHedefleri
                .Include(b => b.Kategori)
                .Where(b => b.KullaniciID == kullaniciId)
                .Select(b => new
                {
                    id = "hedef-" + b.HedefID,
                    islemId = 0,
                    title = "🎯 Hedef: " + b.Kategori.KategoriAdi + " (" + b.HedefTutar.ToString("N0") + " ₺)",
                    start = new DateTime(b.Yil, b.Ay, 1).ToString("yyyy-MM-dd"),
                    allDay = true,
                    color = "#593bdb",
                    textColor = "#ffffff",
                    aciklama = b.Kategori.KategoriAdi + " kategorisi için aylık bütçe hedefi",
                    kategoriAdi = b.Kategori.KategoriAdi,
                    tutar = b.HedefTutar.ToString("N2"),
                    tutarRaw = b.HedefTutar,
                    tip = "Bütçe Hedefi",
                    tarihFormatli = new DateTime(b.Yil, b.Ay, 1).ToString("MMMM yyyy")
                })
                .ToList();

            var events = islemlerList.Concat<object>(butceHedefleriList).ToList();

            return Json(events);
        }

        // Takvim üzerinden doğrudan hızlı gelir/gider ekleme
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult HizliIslemEkle(string tip, int kategoriId, decimal tutar, string aciklama, string tarih)
        {
            if (HttpContext.Session.GetInt32("KullaniciID") == null)
                return Json(new { success = false, message = "Oturum süreniz dolmuş." });

            int kullaniciId = HttpContext.Session.GetInt32("KullaniciID").Value;

            if (tutar <= 0 || kategoriId <= 0)
                return Json(new { success = false, message = "Lütfen geçerli bir tutar ve kategori seçiniz." });

            DateTime islemTarihi;
            if (!DateTime.TryParseExact(tarih, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out islemTarihi))
            {
                islemTarihi = DateTime.Now;
            }

            var yeniIslem = new Models.Siniflar.Islem
            {
                KullaniciID = kullaniciId,
                KategoriID = kategoriId,
                Tutar = tutar,
                Aciklama = string.IsNullOrWhiteSpace(aciklama) ? (tip == "Gelir" ? "Gelir Kaydı" : "Gider Kaydı") : aciklama.Trim(),
                Tarih = islemTarihi
            };

            _db.Islemler.Add(yeniIslem);
            _db.SaveChanges();

            return Json(new { success = true, message = "Finansal işlem takvime başarıyla eklendi!" });
        }

        // Takvim üzerinden işlem silme
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult IslemSil(int id)
        {
            if (HttpContext.Session.GetInt32("KullaniciID") == null)
                return Json(new { success = false, message = "Oturum süreniz dolmuş." });

            int kullaniciId = HttpContext.Session.GetInt32("KullaniciID").Value;
            var islem = _db.Islemler.FirstOrDefault(x => x.IslemID == id && x.KullaniciID == kullaniciId);

            if (islem == null)
                return Json(new { success = false, message = "İşlem bulunamadı veya yetkiniz yok." });

            _db.Islemler.Remove(islem);
            _db.SaveChanges();

            return Json(new { success = true, message = "İşlem başarıyla silindi." });
        }

        // Belirli bir tarihe ait işlemleri, toplam gelir-gider ve net bakiye analizini döndürür
        [HttpGet]
        public JsonResult GetEventsByDate(string date)
        {
            if (HttpContext.Session.GetInt32("KullaniciID") == null)
                return Json(new { success = false, message = "Oturum süreniz dolmuş." });

            int kullaniciId = HttpContext.Session.GetInt32("KullaniciID").Value;

            DateTime selectedDate;
            if (!DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out selectedDate))
            {
                return Json(new { success = false, message = "Geçersiz tarih formatı." });
            }

            var trCulture = new CultureInfo("tr-TR");

            var islemler = _db.Islemler
                .Include(i => i.Kategori)
                .Where(i => i.KullaniciID == kullaniciId && i.Tarih.Date == selectedDate.Date)
                .OrderByDescending(i => i.Tutar)
                .ToList();

            decimal gunlukGelir = islemler.Where(i => i.Kategori.Tip == "Gelir").Sum(i => i.Tutar);
            decimal gunlukGider = islemler.Where(i => i.Kategori.Tip == "Gider").Sum(i => i.Tutar);
            decimal gunlukNet = gunlukGelir - gunlukGider;

            var kategoriDagilimi = islemler
                .GroupBy(i => new { i.Kategori.KategoriAdi, i.Kategori.Tip })
                .Select(g => new
                {
                    kategori = g.Key.KategoriAdi,
                    tip = g.Key.Tip,
                    toplam = g.Sum(x => x.Tutar),
                    toplamFormatli = g.Sum(x => x.Tutar).ToString("N2") + " ₺"
                })
                .ToList();

            var islemListesi = islemler.Select(e => new
            {
                islemId = e.IslemID,
                kategori = e.Kategori.KategoriAdi,
                aciklama = string.IsNullOrWhiteSpace(e.Aciklama) ? e.Kategori.KategoriAdi : e.Aciklama,
                tutar = e.Tutar.ToString("N2"),
                tutarRaw = e.Tutar,
                tip = e.Kategori.Tip
            }).ToList();

            return Json(new
            {
                success = true,
                tarihFormatli = selectedDate.ToString("dd MMMM yyyy, dddd", trCulture),
                tarihIso = selectedDate.ToString("yyyy-MM-dd"),
                toplamIslem = islemler.Count,
                gunlukGelir = gunlukGelir.ToString("N2"),
                gunlukGider = gunlukGider.ToString("N2"),
                gunlukNet = gunlukNet.ToString("N2"),
                gunlukNetPozitif = gunlukNet >= 0,
                kategoriDagilimi = kategoriDagilimi,
                islemler = islemListesi
            });
        }
    }
}
