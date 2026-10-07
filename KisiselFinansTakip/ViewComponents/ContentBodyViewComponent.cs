using KisiselFinansTakip.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

namespace KisiselFinansTakip.ViewComponents
{
    public class ContentBodyViewComponent : ViewComponent
    {
        private readonly VeritabaniContext _db;
        private readonly KisiselFinansTakip.Services.FinansAsistaniService _asistanService;

        public ContentBodyViewComponent(VeritabaniContext db, KisiselFinansTakip.Services.FinansAsistaniService asistanService)
        {
            _db = db;
            _asistanService = asistanService;
        }

        public IViewComponentResult Invoke()
        {
            int? kullaniciIdNullable = HttpContext.Session.GetInt32("KullaniciID");
            if (kullaniciIdNullable == null)
            {
                return View();
            }
            int kullaniciId = kullaniciIdNullable.Value;
            var trCulture = new CultureInfo("tr-TR");

            DateTime simdi = DateTime.Now;
            DateTime buAyinBasi = new DateTime(simdi.Year, simdi.Month, 1);
            DateTime buAyinSonu = buAyinBasi.AddMonths(1);

            // Kullanicinin tum islemleri
            var tumIslemler = _db.Islemler
                .Include(i => i.Kategori)
                .Where(i => i.KullaniciID == kullaniciId)
                .OrderByDescending(i => i.Tarih)
                .ToList();

            // 1. Genel Toplamlar (Tum Zamanlar)
            decimal genelToplamGelir = tumIslemler.Where(i => i.Kategori.Tip == "Gelir").Sum(i => i.Tutar);
            decimal genelToplamGider = tumIslemler.Where(i => i.Kategori.Tip == "Gider").Sum(i => i.Tutar);
            decimal netBakiye = genelToplamGelir - genelToplamGider;

            // 2. Bu Ayki Islemler
            var buAykiIslemler = tumIslemler.Where(i => i.Tarih >= buAyinBasi && i.Tarih < buAyinSonu).ToList();
            decimal buAyGelir = buAykiIslemler.Where(i => i.Kategori.Tip == "Gelir").Sum(i => i.Tutar);
            decimal buAyGider = buAykiIslemler.Where(i => i.Kategori.Tip == "Gider").Sum(i => i.Tutar);
            decimal buAyNet = buAyGelir - buAyGider;

            // 3. Son 12 Ay (1 Yil) Karsilastirmali Trend
            var aylar12Listesi = new List<string>();
            var gelirler12Listesi = new List<decimal>();
            var giderler12Listesi = new List<decimal>();
            var tasarruflar12Listesi = new List<decimal>();

            var aylikTabloVerisi = new List<dynamic>();

            for (int m = 11; m >= 0; m--)
            {
                var ayBaslangic = buAyinBasi.AddMonths(-m);
                var ayBitis = ayBaslangic.AddMonths(1);
                var oAydakiIslemler = tumIslemler.Where(i => i.Tarih >= ayBaslangic && i.Tarih < ayBitis).ToList();

                string ayAdi = ayBaslangic.ToString("MMM yyyy", trCulture);
                decimal ayGelir = oAydakiIslemler.Where(i => i.Kategori.Tip == "Gelir").Sum(i => i.Tutar);
                decimal ayGider = oAydakiIslemler.Where(i => i.Kategori.Tip == "Gider").Sum(i => i.Tutar);
                decimal ayFark = ayGelir - ayGider;
                int tasarrufOrani = ayGelir > 0 ? (int)((ayFark / ayGelir) * 100) : 0;

                aylar12Listesi.Add(ayAdi);
                gelirler12Listesi.Add(ayGelir);
                giderler12Listesi.Add(ayGider);
                tasarruflar12Listesi.Add(ayFark);

                aylikTabloVerisi.Add(new
                {
                    AyAdi = ayBaslangic.ToString("MMMM yyyy", trCulture),
                    Gelir = ayGelir,
                    Gider = ayGider,
                    NetFark = ayFark,
                    TasarrufOrani = tasarrufOrani,
                    IslemSayisi = oAydakiIslemler.Count
                });
            }

            // 4. Son 1 Yillik Kategori Bazli Gider Dagilimi
            DateTime birYilOnce = buAyinBasi.AddMonths(-11);
            var son1YilIslemleri = tumIslemler.Where(i => i.Tarih >= birYilOnce).ToList();

            var yillikGiderKategorileri = son1YilIslemleri
                .Where(i => i.Kategori.Tip == "Gider")
                .GroupBy(i => i.Kategori.KategoriAdi)
                .Select(g => new
                {
                    KategoriAdi = g.Key,
                    ToplamTutar = g.Sum(x => x.Tutar)
                })
                .OrderByDescending(x => x.ToplamTutar)
                .ToList();

            var yillikGelirKategorileri = son1YilIslemleri
                .Where(i => i.Kategori.Tip == "Gelir")
                .GroupBy(i => i.Kategori.KategoriAdi)
                .Select(g => new
                {
                    KategoriAdi = g.Key,
                    ToplamTutar = g.Sum(x => x.Tutar)
                })
                .OrderByDescending(x => x.ToplamTutar)
                .ToList();

            // 5. Ortalama Hesaplamalari (Son 1 Yil)
            decimal yillikToplamGelir = gelirler12Listesi.Sum();
            decimal yillikToplamGider = giderler12Listesi.Sum();
            decimal yillikNetBirikim = yillikToplamGelir - yillikToplamGider;
            decimal aylikOrtalamaGelir = yillikToplamGelir / 12;
            decimal aylikOrtalamaGider = yillikToplamGider / 12;
            int ortalamaTasarrufYuzdesi = yillikToplamGelir > 0 ? (int)((yillikNetBirikim / yillikToplamGelir) * 100) : 0;
            string enCokHarcamaKategori = yillikGiderKategorileri.FirstOrDefault()?.KategoriAdi ?? "Kira";
            decimal enCokHarcamaTutar = yillikGiderKategorileri.FirstOrDefault()?.ToplamTutar ?? 0m;

            // 6. Bu Ayki Kategori Giderleri (Kucuk Tablo icin)
            var buAyKategoriGiderleri = buAykiIslemler
                .Where(i => i.Kategori.Tip == "Gider")
                .GroupBy(i => i.Kategori.KategoriAdi)
                .Select(g => new
                {
                    KategoriAdi = g.Key,
                    ToplamTutar = g.Sum(x => x.Tutar),
                    Yuzde = buAyGider > 0 ? (int)((g.Sum(x => x.Tutar) / buAyGider) * 100) : 0
                })
                .OrderByDescending(x => x.ToplamTutar)
                .ToList();

            // 7. Bu Ayin Butce Hedefleri
            var hedefler = _db.ButceHedefleri
                .Include(h => h.Kategori)
                .Where(h => h.KullaniciID == kullaniciId && h.Ay == simdi.Month && h.Yil == simdi.Year)
                .ToList();

            var hedefDetaylar = new List<dynamic>();
            foreach (var hedef in hedefler)
            {
                var harcama = buAykiIslemler.Where(i => i.KategoriID == hedef.KategoriID).Sum(i => i.Tutar);
                int yuzde = hedef.HedefTutar > 0 ? (int)((harcama / hedef.HedefTutar) * 100) : 0;

                hedefDetaylar.Add(new
                {
                    hedef.HedefID,
                    KategoriAdi = hedef.Kategori?.KategoriAdi ?? "Kategori",
                    HedefTutar = hedef.HedefTutar,
                    Harcama = harcama,
                    Kalan = hedef.HedefTutar - harcama,
                    Yuzde = yuzde
                });
            }

            // ViewBag Tasiyicilari
            ViewBag.GenelToplamGelir = genelToplamGelir;
            ViewBag.GenelToplamGider = genelToplamGider;
            ViewBag.NetBakiye = netBakiye;
            ViewBag.BuAyGelir = buAyGelir;
            ViewBag.BuAyGider = buAyGider;
            ViewBag.BuAyNet = buAyNet;
            ViewBag.ToplamIslemSayisi = tumIslemler.Count;

            ViewBag.YillikToplamGelir = yillikToplamGelir;
            ViewBag.YillikToplamGider = yillikToplamGider;
            ViewBag.YillikNetBirikim = yillikNetBirikim;
            ViewBag.AylikOrtalamaGelir = aylikOrtalamaGelir;
            ViewBag.AylikOrtalamaGider = aylikOrtalamaGider;
            ViewBag.OrtalamaTasarrufYuzdesi = ortalamaTasarrufYuzdesi;
            ViewBag.EnCokHarcamaKategori = enCokHarcamaKategori;
            ViewBag.EnCokHarcamaTutar = enCokHarcamaTutar;

            // Grafikler Icin JSON dizileri
            ViewBag.Aylar12Json = Newtonsoft.Json.JsonConvert.SerializeObject(aylar12Listesi);
            ViewBag.Gelirler12Json = Newtonsoft.Json.JsonConvert.SerializeObject(gelirler12Listesi);
            ViewBag.Giderler12Json = Newtonsoft.Json.JsonConvert.SerializeObject(giderler12Listesi);
            ViewBag.Tasarruflar12Json = Newtonsoft.Json.JsonConvert.SerializeObject(tasarruflar12Listesi);

            ViewBag.YillikGiderKategoriAdlariJson = Newtonsoft.Json.JsonConvert.SerializeObject(yillikGiderKategorileri.Select(x => x.KategoriAdi).ToList());
            ViewBag.YillikGiderKategoriTutarlariJson = Newtonsoft.Json.JsonConvert.SerializeObject(yillikGiderKategorileri.Select(x => x.ToplamTutar).ToList());

            ViewBag.YillikGelirKategoriAdlariJson = Newtonsoft.Json.JsonConvert.SerializeObject(yillikGelirKategorileri.Select(x => x.KategoriAdi).ToList());
            ViewBag.YillikGelirKategoriTutarlariJson = Newtonsoft.Json.JsonConvert.SerializeObject(yillikGelirKategorileri.Select(x => x.ToplamTutar).ToList());

            ViewBag.AylikTabloVerisi = aylikTabloVerisi;
            ViewBag.BuAyGiderKategorileri = buAyKategoriGiderleri;
            ViewBag.ButceHedefleriDetay = hedefDetaylar;
            ViewBag.SonIslemler = tumIslemler.Take(6).ToList();

            // FinFlow Akıllı Finans Danışmanı İçgörüleri
            ViewBag.GenelIcgoruler = _asistanService.GetGenelFinansIcgoruListesi(kullaniciId);

            return View();
        }
    }
}
