using KisiselFinansTakip.Models.Siniflar;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KisiselFinansTakip.Models.ViewModels
{
    public class KategoriViewModel
    {
        public int KategoriID { get; set; }
        public string KategoriAdi { get; set; }
        public string Tip { get; set; }
        public List<Kategori> Kategoriler { get; set; }
        public List<string> TipListesi { get; set; }

    }
}
