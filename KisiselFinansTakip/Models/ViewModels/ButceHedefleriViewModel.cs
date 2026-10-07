using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KisiselFinansTakip.Models.ViewModels
{
    public class ButceHedefleriViewModel
    {
        public int HedefID { get; set; }

        public int KategoriID { get; set; }

        public string KategoriTip { get; set; }  // Gelir/Gider için
        public string KategoriAdi { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal HedefTutar { get; set; }

        [Required]
        public int Ay { get; set; }

        [Required]
        public int Yil { get; set; }
        public decimal ToplamHarcama { get; set; }
    }
}
