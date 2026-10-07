using KisiselFinansTakip.Models.Siniflar;
using System.ComponentModel.DataAnnotations;

namespace KisiselFinansTakip.Models.ViewModels
{
    public class IslemViewModel
    {
        public int IslemID { get; set; }

        [Required]
        public string Aciklama { get; set; }

        [Required]
        public decimal Tutar { get; set; }

        [Required]
        public DateTime Tarih { get; set; } = DateTime.Now;

        [Required]
        public int KategoriID { get; set; }

        public string Tip { get; set; }

        public List<Kategori> Kategoriler { get; set; } = new List<Kategori>();

        public List<Islem> Islemler { get; set; } = new List<Islem>();
    }

}

