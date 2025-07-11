using KisiselFinansTakip.Data;
using KisiselFinansTakip.Models.Siniflar;
using KisiselFinansTakip.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
            var model = new KategoriViewModel
            {
                TipListesi = new List<string> { "Gelir", "Gider" },
                Kategoriler = _db.Kategoriler.ToList()
            };
            return View(model);
        }

        public IActionResult KategoriGetir(int id)
        {
            var kategori = _db.Kategoriler.FirstOrDefault(x => x.KategoriID == id);

            if (kategori == null)
                return NotFound();

            var model = new KategoriViewModel
            {
                KategoriID = kategori.KategoriID,
                KategoriAdi = kategori.KategoriAdi,
                Tip = kategori.Tip,
                TipListesi = new List<string> { "Gelir", "Gider" },
                Kategoriler = _db.Kategoriler.ToList()
            };

            return View("Kategori", model); // Aynı view'a geri gönderiyoruz
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult KategoriIslem(KategoriViewModel model, string action)
        {
            if (action == "Ekle")
            {
                if (string.IsNullOrWhiteSpace(model.KategoriAdi) || string.IsNullOrWhiteSpace(model.Tip))
                {
                    TempData["Uyari"] = "Lütfen tüm alanları doldurunuz.";
                    model.Kategoriler = _db.Kategoriler.ToList();
                    model.TipListesi = new List<string> { "Gelir", "Gider" };
                    return View("Kategori", model);
                }


                var kategori = new Kategori
                {
                    KategoriAdi = model.KategoriAdi.Trim(),
                    Tip = model.Tip.Trim()
                };
                _db.Kategoriler.Add(kategori);
                _db.SaveChanges();

                TempData["Mesaj"] = "Yeni Kategori eklendi.";
            }
            else if (action == "Guncelle")
            {
                var kategori = _db.Kategoriler.FirstOrDefault(x => x.KategoriID == model.KategoriID);
                if (kategori == null) return NotFound();

                kategori.KategoriAdi = model.KategoriAdi.Trim();
                kategori.Tip = model.Tip.Trim();
                _db.SaveChanges();

                TempData["Mesaj"] = "Kategori guncellendi.";
            }
            else if (action == "Sil")
            {
                var kategori = _db.Kategoriler.FirstOrDefault(x => x.KategoriID == model.KategoriID);
                if (kategori == null) return NotFound();

                _db.Kategoriler.Remove(kategori);
                _db.SaveChanges();

                TempData["Mesaj"] = "Kategori silindi.";
            }

            return RedirectToAction("Kategori");
        }

    }
}
