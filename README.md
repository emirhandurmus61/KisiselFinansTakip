<p align="center">
  <img src="docs/images/dashboard-preview.png" alt="FinFlow Dashboard Preview" width="100%" style="border-radius: 12px; box-shadow: 0 8px 30px rgba(0,0,0,0.12);" />
</p>

<h1 align="center">💳 FinFlow - Akıllı Kişisel Finans & Bütçe Yönetimi</h1>

<p align="center">
  <b>Smart Personal Finance, Budget Planning & Financial Health Platform</b><br>
  Modern, responsive, user-friendly, and intelligent financial tracker built with ASP.NET Core & PostgreSQL.
</p>

<p align="center">
  <img src="https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white" alt=".NET 8.0" />
  <img src="https://img.shields.io/badge/Database-PostgreSQL-336791?logo=postgresql&logoColor=white" alt="PostgreSQL" />
  <img src="https://img.shields.io/badge/Architecture-MVC_%26_EF_Core-0078D7" alt="MVC Architecture" />
  <img src="https://img.shields.io/badge/UI-Bootstrap_%26_Focus2-7952B3" alt="Bootstrap" />
  <img src="https://img.shields.io/badge/License-MIT-green" alt="MIT License" />
</p>

---

## 🌐 Dil / Language
- [🇹🇷 Türkçe Dokümantasyon](#-türkçe-dokümantasyon)
- [🇬🇧 English Documentation](#-english-documentation)

---

# 🇹🇷 Türkçe Dokümantasyon

## 📌 Proje Hakkında
**FinFlow**, bireysel finans yönetimini sıkıcı Excel tablolarından ve karmaşık hesaplardan kurtarıp, kullanıcıyı yönlendiren, uyaran ve motive eden **akıllı bir finans asistanı** sunan web tabanlı bir bütçe yönetim platformudur.

Kullanıcıların gelir ve harcamalarını kaydetmelerini, kategorize etmelerini, son 1 yıllık finansal eğilimlerini grafiklerle analiz etmelerini ve bütçe aşımı durumlarında yapay zeka tarzı dinamik tavsiyeler almalarını sağlar.

---

## ✨ Öne Çıkan Özellikler

### 1. 🧠 Akıllı Finans Danışmanı & İçgörü Motoru
- **Canlı Finansal Sağlık Analizi:** Tasarruf oranlarınızı anlık olarak analiz eder.
- **Kritik Eşik & Harcama Alarmları:** Bir harcama kalemi (örn. Gıda, Ulaşım) aylık bütçenizin %35'inden fazlasını kapladığında otomatik uyarı verir.
- **50/30/20 Kuralı Rehberliği:** Gelirinizi %50 zorunlu ihtiyaçlar, %30 kişisel istekler ve %20 birikim olarak dengede tutmanız için rehberlik sağlar.
- **Önerilen Aksiyonlar:** Her analiz kartında kullanıcıya özel, somut aksiyon adımları listeler.

### 2. 📊 Son 1 Yılın Gelir - Gider & Trend Analizi
- **12 Aylık Karşılaştırmalı Grafikler (Chart.js):** Gelir sütunları, gider sütunları ve aylık net tasarruf eğrisi.
- **Kategori Dağılım Grafiği (Donut Chart):** Yıllık ve aylık harcamaların oransal payı.
- **KPI Göstergeleri:** Net kasa bakiyesi, aylık ortalama tasarruf yüzdesi, 1 yıllık kümülatif kazanç/gider.

### 3. 🎯 Bütçe Hedefleri & Dinamik Bildirimler
- Her kategoriye özel aylık harcama limitleri tanımlama.
- **Canlı İlerleme Çubuğu:** Limitinizin ne kadarını harcadığınızı anlık takip edin (%80+ uyarı, %100+ limit aşımı).
- **Akıllı Bütçe Bildirimleri:**
  - *Limit Aşımı Alarmı:* Belirlenen hedef aşıldığında anında uyarı ve tasarruf tavsiyesi.
  - *Başarı & Motivasyon Bildirimleri:* Bütçe disiplinine sadık kalındığında tebrik ve yatırım yönlendirmeleri.

### 4. 📅 Etkileşimli Finansal Takvim & Günlük Analiz
- **Ayrık Görünüm Filtreleri:** Gelir, gider veya bütçe hedeflerini tek tıkla filtreleme.
- **Bütünsel Günlük Rapor Modalı:** Takvimdeki herhangi bir güne tıklandığında, o günün tüm gelir, gider, net bakiye ve kategori dökümünü tek seferde gösteren detay penceresi.
- **Hızlı İşlem Ekleme / Silme:** Sayfa değiştirmeden doğrudan takvim üzerinden işlem kaydetme.

### 5. 📱 Tam Mobil Uyumlu Drawer (Çekmece) Tasarım
- iOS ve Android standartlarında akıcı mobil çekmece menü.
- Karanlık mod sidebarında yüksek kontrastlı, göz yormayan tipografi.
- Sayfa karartma (backdrop overlay) ve dokunmatik jest optimizasyonu.

---

## 🛠️ Teknik Altyapı & Teknolojiler

| Katman | Teknoloji / Kütüphane |
|---|---|
| **Framework** | .NET 8.0 (C# / ASP.NET Core MVC) |
| **ORM** | Entity Framework Core 8.0 (Code-First & Migrations) |
| **Veritabanı** | PostgreSQL (Npgsql.EntityFrameworkCore.PostgreSQL) |
| **Oturum Yönetimi** | ASP.NET Core Session & Cookie Güvenliği |
| **Arayüz (Frontend)** | HTML5, CSS3, JavaScript (ES6+), Bootstrap 4 / Focus-2 |
| **Grafikler & Takvim** | Chart.js 2.8, FullCalendar, Moment.js, jQuery UI |
| **İkonlar & Bildirimler**| FontAwesome 6, SweetAlert2 |

---

## 🚀 Kurulum & Çalıştırma Adımları

### 1. Gereksinimler
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/) (veya Docker)

### 2. Depoyu Klonlayın
```bash
git clone https://github.com/emirhandurmus/KisiselFinansTakip.git
cd KisiselFinansTakip
```

### 3. Veritabanı Ayarları (`appsettings.json`)
`KisiselFinansTakip/appsettings.json` dosyasında PostgreSQL bağlantı cümlenizi düzenleyin:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=FinansTakipDb;Username=postgres;Password=your_password"
  }
}
```

### 4. Veritabanı Şemasını Uygulayın
```bash
dotnet ef database update --project KisiselFinansTakip
```

### 5. Projeyi Başlatın
```bash
dotnet run --project KisiselFinansTakip
```
Tarayıcınızdan `http://localhost:5074` adresine giderek uygulamayı kullanmaya başlayabilirsiniz.

---
---

# 🇬🇧 English Documentation

## 📌 About The Project
**FinFlow** is a modern personal finance and cash flow management web application designed to transform boring expense tracking into an **engaging, intelligent financial health companion**.

Built with ASP.NET Core 8 and PostgreSQL, FinFlow provides dynamic financial insights, proactive budget alarms, interactive calendar-based daily ledger analysis, and full-year comparative charts.

---

## ✨ Key Features

### 1. 🧠 Smart Financial Advisor & Insights Engine
- **Real-Time Savings Rate Detection:** Evaluates cash savings velocity and recommends liquid emergency funds or investment instruments.
- **Heavy Expense Outlier Alert:** Detects categories exceeding 35% of total monthly outflow.
- **50/30/20 Budget Rule Companion:** Evaluates essential needs vs. wants vs. savings balance.
- **Actionable Steps:** Every insight card includes direct, actionable financial recommendations.

### 2. 📊 1-Year Financial Analytics & Visual Trends
- **Comparative 12-Month Charts (Chart.js):** Income bars, expense bars, and net cashflow trends.
- **Expense Category Doughnut Chart:** Annual and monthly percentage breakdown.
- **Financial KPI Ribbon:** Net vault balance, 12-month savings rate, and annual totals.

### 3. 🎯 Budget Targets & Proactive Notifications
- Category-level custom monthly expenditure targets.
- **Real-Time Progress Bar:** Color-coded stages (Green < 80%, Warning > 80%, Red = Over-budget).
- **Proactive Alarms & Positive Reinforcement:**
  - *Over-Budget Warnings:* Instant alarm when category cap is breached.
  - *Discipline Praise:* Motivational reinforcement for categories maintained under budget.

### 4. 📅 Interactive Financial Calendar & Holistic Daily Ledger
- **Category Filter Pills:** Separate filter toggles for Income, Expense, and Targets.
- **Holistic Daily Modal:** Clicking any day displays an aggregated popup showing daily total income, expense, net balance, category breakdown, and itemized entries in a single view.
- **Quick Add/Delete:** Add or delete transactions directly from the calendar without reloading.

### 5. 📱 Fully Responsive Mobile Drawer UI
- Sleek off-canvas navigation drawer tailored for modern iOS & Android viewports.
- High-contrast, readable typography on dark sidebar themes.
- Touch backdrop overlay with auto-closing gestures.

---

## 🛠️ Technical Stack & Architecture

| Layer | Technology |
|---|---|
| **Backend Framework** | .NET 8.0 (C# / ASP.NET Core MVC) |
| **Data Access / ORM** | Entity Framework Core 8.0 |
| **Database** | PostgreSQL via Npgsql Driver |
| **Authentication** | Session State & Antiforgery Tokens |
| **Frontend UI** | HTML5, CSS3, JavaScript (ES6+), Bootstrap 4, Focus-2 Theme |
| **Data Visualization** | Chart.js, FullCalendar, Moment.js |
| **Icons & Modals** | FontAwesome 6, SweetAlert2 |

---

## 🚀 Getting Started

### 1. Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/) (Local or Docker container)

### 2. Clone the Repository
```bash
git clone https://github.com/emirhandurmus/KisiselFinansTakip.git
cd KisiselFinansTakip
```

### 3. Configure Database Connection (`appsettings.json`)
Edit `KisiselFinansTakip/appsettings.json` with your credentials:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=FinansTakipDb;Username=postgres;Password=your_password"
  }
}
```

### 4. Apply Database Migrations
```bash
dotnet ef database update --project KisiselFinansTakip
```

### 5. Run the Application
```bash
dotnet run --project KisiselFinansTakip
```
Navigate to `http://localhost:5074` in your browser.

---

## 📄 License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
