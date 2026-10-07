using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KisiselFinansTakip.Models.Siniflar
{
    public class ButceHedefleri
    {
        [Key]
        public int HedefID { get; set; }

        [Required]
        public int KullaniciID { get; set; }

        [ForeignKey("KullaniciID")]
        public Kullanici Kullanici { get; set; }

        [Required]
        public int KategoriID { get; set; }

        [ForeignKey("KategoriID")]
        public Kategori Kategori { get; set; }

        [Required]
        public int Ay { get; set; }  // 1-12 arası

        [Required]
        public int Yil { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal HedefTutar { get; set; }
    
    }
}
