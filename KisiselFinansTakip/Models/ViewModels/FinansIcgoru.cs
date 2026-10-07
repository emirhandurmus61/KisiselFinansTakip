using System;

namespace KisiselFinansTakip.Models.ViewModels
{
    public enum FinansTavsiyeTipi
    {
        Basari,      // Aferin / Harika Tasarruf / Hedef Başarılı
        Uyari,       // Dikkat / %80+ limit / Yüksek Harcama Artışı
        Tehlike,     // Limit Aşıldı / Acil Tasarruf / Bütçe Delindi
        Bilgi,       // Fırsat / Gelir Artırma İpucu / Yatırım Fikri
        Oneri        // Kategori bazlı pratik aksiyon
    }

    public class FinansIcgoru
    {
        public string Baslik { get; set; } = string.Empty;
        public string Mesaj { get; set; } = string.Empty;
        public string AksiyonOnerisi { get; set; } = string.Empty;
        public FinansTavsiyeTipi Tip { get; set; }
        public string Ikon { get; set; } = "fa-lightbulb";
        public string KategoriAdi { get; set; } = string.Empty;
        public decimal? Tutar { get; set; }
        public int Oncelik { get; set; } // 1: En kritik, 5: Genel ipucu
    }
}
