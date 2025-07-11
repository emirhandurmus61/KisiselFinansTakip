using System.ComponentModel.DataAnnotations;

namespace KisiselFinansTakip.Models.Siniflar
{
	public class Kategori
	{
		[Key]
        public int KategoriID { get; set; }
		[Required, StringLength(100)]
		public string KategoriAdi { get; set; }
		[Required, StringLength(20)]
		public string Tip { get; set; } //Gelir-Gider	
        public virtual ICollection<Islem> Islemler { get; set; }
    }
}
