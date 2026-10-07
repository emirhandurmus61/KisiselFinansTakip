using KisiselFinansTakip.Data;
using KisiselFinansTakip.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KisiselFinansTakip.Services
{
    public class FinansAsistaniService
    {
        private readonly VeritabaniContext _db;

        public FinansAsistaniService(VeritabaniContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Bütçe hedefleri sayfasına özel dinamik bildirim ve içgörü üretir.
        /// </summary>
        public List<FinansIcgoru> GetButceHedefIcgoruListesi(int kullaniciId, int ay, int yil, List<ButceHedefleriViewModel> hedefler)
        {
            var icgoruler = new List<FinansIcgoru>();

            if (hedefler == null || !hedefler.Any())
            {
                icgoruler.Add(new FinansIcgoru
                {
                    Baslik = "Bu Dönem İçin Hedef Belirlenmedi",
                    Mesaj = "Bu ay henüz herhangi bir harcama kategorisine limit atanmadı. Bütçenizi kontrolsüz harcamaktan korumak için en az 2-3 temel kategoriye limit belirlemeniz önerilir.",
                    AksiyonOnerisi = "Gıda, Kira ve Sosyal Yaşam kategorilerine aylık bütçe limiti tanımlayın.",
                    Tip = FinansTavsiyeTipi.Bilgi,
                    Ikon = "fa-bullseye",
                    Oncelik = 1
                });
                return icgoruler;
            }

            decimal toplamHedeflenen = hedefler.Where(h => h.KategoriTip == "Gider").Sum(h => h.HedefTutar);
            decimal toplamHarcanan = hedefler.Where(h => h.KategoriTip == "Gider").Sum(h => h.ToplamHarcama);
            int asilanHedefSayisi = 0;
            int kritikHedefSayisi = 0;
            int basariliHedefSayisi = 0;

            foreach (var h in hedefler.Where(h => h.KategoriTip == "Gider"))
            {
                if (h.HedefTutar <= 0) continue;

                decimal oran = (h.ToplamHarcama / h.HedefTutar) * 100m;
                decimal fark = h.HedefTutar - h.ToplamHarcama;

                if (h.ToplamHarcama > h.HedefTutar)
                {
                    asilanHedefSayisi++;
                    decimal asimTutari = h.ToplamHarcama - h.HedefTutar;
                    icgoruler.Add(new FinansIcgoru
                    {
                        Baslik = $"Bütçe Aşımı: {h.KategoriAdi} Kategorisi",
                        Mesaj = $"{h.KategoriAdi} için belirlediğiniz {h.HedefTutar:N2} ₺ limit, {asimTutari:N2} ₺ aşıldı (Toplam Harcama: {h.ToplamHarcama:N2} ₺).",
                        AksiyonOnerisi = $"Bu ay bu kategorideki harcamaları durdurun ve esnek kategorilerden {asimTutari:N2} ₺ tasarruf sağlayın.",
                        Tip = FinansTavsiyeTipi.Tehlike,
                        Ikon = "fa-exclamation-circle",
                        KategoriAdi = h.KategoriAdi,
                        Tutar = asimTutari,
                        Oncelik = 1
                    });
                }
                else if (oran >= 80m)
                {
                    kritikHedefSayisi++;
                    icgoruler.Add(new FinansIcgoru
                    {
                        Baslik = $"Kritik Seviye: {h.KategoriAdi} Limiti Dolmak Üzere",
                        Mesaj = $"{h.KategoriAdi} bütçenizin %{oran:N0}'i kullanıldı. Kalan harcama payınız {fark:N2} ₺.",
                        AksiyonOnerisi = "Ay sonuna kadar bu kategoride planlanmamış ek harcama yapmaktan kaçının.",
                        Tip = FinansTavsiyeTipi.Uyari,
                        Ikon = "fa-bell",
                        KategoriAdi = h.KategoriAdi,
                        Tutar = fark,
                        Oncelik = 2
                    });
                }
                else if (oran <= 60m && h.ToplamHarcama > 0)
                {
                    basariliHedefSayisi++;
                }
            }

            // Genel Bütçe Değerlendirme Mesajları
            if (asilanHedefSayisi == 0 && kritikHedefSayisi == 0 && hedefler.Any())
            {
                icgoruler.Insert(0, new FinansIcgoru
                {
                    Baslik = "Tebrikler: Bütçe Hedefleriniz Dengeli İlerliyor",
                    Mesaj = $"Tüm kategorilerde harcamalarınız belirlenen sınırların altında seyrediyor. Toplam bütçe kullanımınız %{(toplamHedeflenen > 0 ? (toplamHarcanan / toplamHedeflenen * 100m) : 0):N0} seviyesinde.",
                    AksiyonOnerisi = "Mevcut tasarruf disiplininizi koruyarak ay sonu birikiminizi yatırım veya acil durum fonuna aktarabilirsiniz.",
                    Tip = FinansTavsiyeTipi.Basari,
                    Ikon = "fa-shield-alt",
                    Oncelik = 1
                });
            }
            else if (basariliHedefSayisi > 0 && asilanHedefSayisi > 0)
            {
                icgoruler.Add(new FinansIcgoru
                {
                    Baslik = "Tasarruf Başarısı: Kontrollü Kategoriler",
                    Mesaj = $"{basariliHedefSayisi} farklı kategoride harcama hızınız bütçe sınırının oldukça altında seyrediyor.",
                    AksiyonOnerisi = "Tasarruf sağladığınız kategorileri, limit aşımı olan kalemleri dengelemek için kullanabilirsiniz.",
                    Tip = FinansTavsiyeTipi.Basari,
                    Ikon = "fa-check-circle",
                    Oncelik = 3
                });
            }

            return icgoruler.OrderBy(x => x.Oncelik).ToList();
        }

        /// <summary>
        /// Genel finansal durum ve dashboard için akıllı analiz içgörüleri üretir.
        /// </summary>
        public List<FinansIcgoru> GetGenelFinansIcgoruListesi(int kullaniciId)
        {
            var icgoruler = new List<FinansIcgoru>();
            DateTime bugun = DateTime.Now;
            DateTime son30Gun = bugun.AddDays(-30);
            DateTime onceki30Gun = bugun.AddDays(-60);

            var islemlerSon60Gun = _db.Islemler
                .Include(i => i.Kategori)
                .Where(i => i.KullaniciID == kullaniciId && i.Tarih >= onceki30Gun)
                .ToList();

            var buAyIslemleri = islemlerSon60Gun.Where(i => i.Tarih >= son30Gun).ToList();
            var gecenAyIslemleri = islemlerSon60Gun.Where(i => i.Tarih < son30Gun).ToList();

            decimal buAyGelir = buAyIslemleri.Where(i => i.Kategori != null && i.Kategori.Tip == "Gelir").Sum(i => i.Tutar);
            decimal buAyGider = buAyIslemleri.Where(i => i.Kategori != null && i.Kategori.Tip == "Gider").Sum(i => i.Tutar);
            decimal gecenAyGider = gecenAyIslemleri.Where(i => i.Kategori != null && i.Kategori.Tip == "Gider").Sum(i => i.Tutar);

            // 1. Tasarruf Oranı Analizi
            if (buAyGelir > 0)
            {
                decimal netTasarruf = buAyGelir - buAyGider;
                decimal tasarrufOrani = (netTasarruf / buAyGelir) * 100m;

                if (tasarrufOrani >= 30m)
                {
                    icgoruler.Add(new FinansIcgoru
                    {
                        Baslik = "Yüksek Tasarruf Performansı",
                        Mesaj = $"Son 30 günde toplam gelirinizin %{tasarrufOrani:N0}'ini başarıyla biriktirdiniz (Net Kalan: {netTasarruf:N2} ₺).",
                        AksiyonOnerisi = "Bu nakit fazlasını değerlendirmek adına vadeli mevduat, kıymetli maden veya fon araçlarını inceleyebilirsiniz.",
                        Tip = FinansTavsiyeTipi.Basari,
                        Ikon = "fa-chart-line",
                        Oncelik = 1
                    });
                }
                else if (tasarrufOrani < 10m && tasarrufOrani >= 0m)
                {
                    icgoruler.Add(new FinansIcgoru
                    {
                        Baslik = "Düşük Tasarruf Marjı Uyarısı",
                        Mesaj = $"Son 30 günde kazancınızın sadece %{tasarrufOrani:N0}'lik kısmı kasada kaldı. Beklenmedik acil harcamalarda nakit sıkışıklığı yaşanabilir.",
                        AksiyonOnerisi = "Sabit aboneliklerinizi ve dışarıda yeme-içme giderlerinizi inceleyerek en az %15 tasarruf payı ayırın.",
                        Tip = FinansTavsiyeTipi.Uyari,
                        Ikon = "fa-shield-alt",
                        Oncelik = 1
                    });
                }
                else if (netTasarruf < 0)
                {
                    decimal acik = Math.Abs(netTasarruf);
                    icgoruler.Add(new FinansIcgoru
                    {
                        Baslik = "Negatif Nakit Akışı Bildirimi",
                        Mesaj = $"Son 30 günde harcamalarınız kazancınızı {acik:N2} ₺ tutarında aştı. Bütçe dengeniz eksi yönde ilerliyor.",
                        AksiyonOnerisi = "Zorunlu olmayan tüm harcamaları durdurun veya ek gelir alternatiflerini devreye sokun.",
                        Tip = FinansTavsiyeTipi.Tehlike,
                        Ikon = "fa-exclamation-triangle",
                        Tutar = acik,
                        Oncelik = 1
                    });
                }
            }

            // 2. Kategori Bazlı En Çok Para Yutan Kalemler
            var kategoriGiderleri = buAyIslemleri
                .Where(i => i.Kategori != null && i.Kategori.Tip == "Gider")
                .GroupBy(i => i.Kategori.KategoriAdi)
                .Select(g => new { Kategori = g.Key, Toplam = g.Sum(x => x.Tutar) })
                .OrderByDescending(g => g.Toplam)
                .ToList();

            if (kategoriGiderleri.Any())
            {
                var enCokHarcanan = kategoriGiderleri.First();
                decimal yuzdePay = buAyGider > 0 ? (enCokHarcanan.Toplam / buAyGider) * 100m : 0;

                if (yuzdePay > 35m && enCokHarcanan.Kategori != "Kira")
                {
                    icgoruler.Add(new FinansIcgoru
                    {
                        Baslik = $"Yoğun Harcama: {enCokHarcanan.Kategori}",
                        Mesaj = $"Aylık toplam harcamanızın %{yuzdePay:N0}'i ({enCokHarcanan.Toplam:N2} ₺) tek başına {enCokHarcanan.Kategori} kalemine ayrıldı.",
                        AksiyonOnerisi = "Bu kategori için haftalık harcama limiti koyarak bir sonraki ayda %20 oranında tasarruf hedefleyin.",
                        Tip = FinansTavsiyeTipi.Oneri,
                        Ikon = "fa-sliders-h",
                        KategoriAdi = enCokHarcanan.Kategori,
                        Oncelik = 2
                    });
                }
            }

            // 3. Geçen Aya Göre Harcama Artış Uyarısı
            if (gecenAyGider > 0 && buAyGider > gecenAyGider)
            {
                decimal artisYuzdesi = ((buAyGider - gecenAyGider) / gecenAyGider) * 100m;
                if (artisYuzdesi >= 25m)
                {
                    icgoruler.Add(new FinansIcgoru
                    {
                        Baslik = "Aylık Harcama Artış Eğilimi",
                        Mesaj = $"Geçen aya göre toplam giderleriniz %{artisYuzdesi:N0} oranında yükselerek {buAyGider:N2} ₺ seviyesine ulaştı.",
                        AksiyonOnerisi = "Rutin dışı harcama kalemlerini tespit edin ve acil olmayan alımları sonraki aylara planlayın.",
                        Tip = FinansTavsiyeTipi.Uyari,
                        Ikon = "fa-chart-area",
                        Oncelik = 2
                    });
                }
            }

            // 4. Gelir Çeşitlendirme Tavsiyesi
            var gelirKaynaklari = buAyIslemleri
                .Where(i => i.Kategori != null && i.Kategori.Tip == "Gelir")
                .GroupBy(i => i.Kategori.KategoriAdi)
                .Count();

            if (gelirKaynaklari <= 1 && buAyGelir > 0)
            {
                icgoruler.Add(new FinansIcgoru
                {
                    Baslik = "Gelir Çeşitliliği Stratejisi",
                    Mesaj = "Finansal akışınız tek bir gelir kaynağına dayanıyor. Güçlü bir finansal güvence için alternatif gelir kanalları oluşturmak önemlidir.",
                    AksiyonOnerisi = "Ek uzmanlık alanlarınızla freelance projeler veya pasif getiri sağlayacak küçük yatırımlar planlayabilirsiniz.",
                    Tip = FinansTavsiyeTipi.Bilgi,
                    Ikon = "fa-briefcase",
                    Oncelik = 3
                });
            }

            // 5. 50/30/20 Bütçe Kuralı İpuçları
            icgoruler.Add(new FinansIcgoru
            {
                Baslik = "Finansal Denge Kuralı (50/30/20)",
                Mesaj = "Sağlıklı bir bütçe dağılımında gelirin %50'si zorunlu ihtiyaçlara, %30'u kişisel isteklere, %20'si ise tasarruf ve borç kapamaya ayrılır.",
                AksiyonOnerisi = "İşlemlerinizi filtreleyerek zorunlu harcamalarınızın %50 sınırında kalıp kalmadığını kontrol edin.",
                Tip = FinansTavsiyeTipi.Bilgi,
                Ikon = "fa-balance-scale",
                Oncelik = 4
            });

            return icgoruler.OrderBy(x => x.Oncelik).ToList();
        }
    }
}
