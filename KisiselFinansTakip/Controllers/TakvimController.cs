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
                    id = i.IslemID,
                    title = (i.Kategori.Tip == "Gelir" ? "Gelir: " : "Gider: ") + i.Aciklama + " - " + i.Tutar.ToString("N2", CultureInfo.InvariantCulture),
                    start = i.Tarih.ToString("yyyy-MM-dd"),
                    allDay = true,
                    color = i.Kategori.Tip == "Gelir" ? "#28a745" : "#dc3545", // yeşil veya kırmızı
                    aciklama = i.Aciklama,
                    tutar = i.Tutar.ToString("N2"),
                    tip = i.Kategori.Tip
                })
                .ToList();

            var butceHedefleriList = _db.ButceHedefleri
                .Include(b => b.Kategori)
                .Where(b => b.KullaniciID == kullaniciId)
                .Select(b => new
                {
                    id = "hedef-" + b.HedefID,
                    title = "Bütçe Hedefi: " + b.Kategori.KategoriAdi + " - " + b.HedefTutar.ToString("N2", CultureInfo.InvariantCulture),
                    start = new DateTime(b.Yil, b.Ay, 1).ToString("yyyy-MM-dd"),
                    allDay = true,
                    color = "#007bff" // mavi renk
                })
                .ToList();

            var events = islemlerList.Concat<object>(butceHedefleriList).ToList();

            return Json(events);
        }

        // Belirli bir tarihe ait işlemleri (gelir/gider) döndürür
        [HttpGet]
        public JsonResult GetEventsByDate(string date)
        {
            if (HttpContext.Session.GetInt32("KullaniciID") == null)
                return Json(new List<object>());

            int kullaniciId = HttpContext.Session.GetInt32("KullaniciID").Value;

            DateTime selectedDate;
            if (!DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out selectedDate))
            {
                return Json(new List<object>());
            }

            var events = _db.Islemler
                .Include(i => i.Kategori)
                .Where(i => i.KullaniciID == kullaniciId && i.Tarih.Date == selectedDate.Date)
                .Select(e => new
                {
                    Baslik = e.Kategori.Tip == "Gelir" ? "Gelir" : "Gider",
                    Aciklama = e.Aciklama,
                    Tutar = e.Tutar.ToString("N2"),
                    Tip = e.Kategori.Tip
                })
                .ToList();

            return Json(events);
        }
    }
}
