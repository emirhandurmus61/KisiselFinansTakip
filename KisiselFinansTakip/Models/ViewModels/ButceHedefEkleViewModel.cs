using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KisiselFinansTakip.Models.ViewModels
{
    public class ButceHedefEkleViewModel
    {
        public int HedefID { get; set; }

        public int KullaniciID { get; set; }

        [Required]
        public int KategoriID { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal HedefTutar { get; set; }

        [Required]
        public int Ay { get; set; }

        [Required]
        public int Yil { get; set; }
        public string? KategoriTip { get; set; }  // Gelir veya Gider
    }
}
