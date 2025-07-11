using System.ComponentModel.DataAnnotations;

namespace KisiselFinansTakip.Models.Siniflar
{
	public class Kullanici
	{
		[Key]
		public int KullaniciID { get; set; }
		[Required, StringLength(50)]
		public string Ad { get; set; }
		[Required, StringLength(50)]
		public string Soyad { get; set; }
		[Required, StringLength(100)]
		public string Email { get; set; }
		[Required, StringLength(100)]
		public string Sifre { get; set; }
		[Required, StringLength(20)]
		public string Rol { get; set; } = "Kullanici";
		public DateTime KayitTarihi { get; set; } = DateTime.Now;
		public virtual ICollection<Islem> Islemler { get; set; }
	}
}
