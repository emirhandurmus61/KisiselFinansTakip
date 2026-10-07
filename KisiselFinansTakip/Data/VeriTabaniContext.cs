using KisiselFinansTakip.Models.Siniflar;
using Microsoft.EntityFrameworkCore;

namespace KisiselFinansTakip.Data
{
	public class VeritabaniContext : DbContext
	{
		public VeritabaniContext(DbContextOptions<VeritabaniContext> options) : base(options)
		{
		}

		public DbSet<Kullanici> Kullanicilar { get; set; }
		public DbSet<Kategori> Kategoriler { get; set; }
		public DbSet<Islem> Islemler { get; set; }
        public DbSet<ButceHedefleri> ButceHedefleri { get; set; }

    }
}
