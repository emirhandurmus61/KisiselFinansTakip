using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KisiselFinansTakip.Models.Siniflar
{
	public class Islem
	{
        [Key]
        public int IslemID { get; set; }
        [Required, StringLength(100)]
        public string Aciklama { get; set; }
		[Required]
		[Column(TypeName = "decimal(18,2)")]
		public decimal Tutar { get; set; }
		public DateTime Tarih { get; set; } = DateTime.Now;
		public int KategoriID { get; set; }
		[ForeignKey("KategoriID")]
		public virtual Kategori Kategori { get; set; }

		public int KullaniciID { get; set; }
		[ForeignKey("KullaniciID")]
		public virtual Kullanici Kullanici { get; set; }
   
    }
}
