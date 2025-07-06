# 📚 BlogStore – ASP.NET Core Katmanlı Mimari Blog Platformu


## 🚀 Proje Özeti

**BlogStore**, kullanıcıların blog yazılarını oluşturabildiği, yorum yapabildiği ve bir yönetim paneli aracılığıyla içeriklerin kontrol edilebildiği dinamik bir web uygulamasıdır.

## 🛠️ Kullanılan Teknolojiler

- **ASP.NET Core MVC **
- **Entity Framework Core**
- **ASP.NET Identity** (giriş, kayıt, kullanıcı yönetimi)
- **FluentValidation** (form doğrulama)
- **SQL Server**
- **AJAX / jQuery**
- **HuggingFace Toxic-BERT API** (→ toksik yorum tespiti için altyapı hazırlandı)

## 🧩 Katmanlı Mimari Yapı

Proje, yazılım geliştirmenin sürdürülebilirliği ve test edilebilirliği açısından 4 katmanlı mimariyle yapılandırılmıştır:

- **EntityLayer** – Varlık sınıfları (Entity modelleri)
- **DataAccessLayer (DAL)** – Veri erişim katmanı (Repository Pattern)
- **BusinessLayer** – İş mantığı
- **PresentationLayer (WebUI)** – Kullanıcı arayüzü ve controller'lar

## 💡 Öne Çıkan Özellikler

- ✅ SEO dostu **Slug** tabanlı URL yapısı
- ✅ **AJAX** ile oturum kontrolü ve yorum gönderme
- ✅ **HuggingFace Toxic-BERT** API için hazır altyapı (yakında aktif edilecek)
- ✅ Admin paneli üzerinden: içerik, yorum, kategori yönetimi
- ✅ Kullanıcı profili, şifre sıfırlama, giriş/kayıt işlemleri
- ✅ Yazar & kategori sayfaları
- ✅ Responsive blog kartları (3 sütunlu yapı)
- ✅ Kullanıcıya özel **dashboard** ekranı (istatistikler, son makaleler)




