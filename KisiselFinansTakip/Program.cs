using KisiselFinansTakip.Data;
using Microsoft.EntityFrameworkCore;

// PostgreSQL timestamp uyumlulugu (DateTime.Now ve Local timestamp hatasini onler)
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Veritabani baglantisi (PostgreSQL)
builder.Services.AddDbContext<VeritabaniContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("VeritabaniBaglantisi")));

// Session icin gerekli servisler
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// MVC servisleri
builder.Services.AddControllersWithViews();

// Finans Danışmanı / Asistan Servisi
builder.Services.AddScoped<KisiselFinansTakip.Services.FinansAsistaniService>();

var app = builder.Build();

// Hata sayfasi ve guvenlik ayarlari
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Middleware pipeline
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ** Session middleware **
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Default}/{action=Index}/{id?}");

app.Run();
