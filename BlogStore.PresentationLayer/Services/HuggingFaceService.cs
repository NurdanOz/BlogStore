using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json; // Newtonsoft.Json kütüphanesini kullanmak için bu using olmalı
using System.Threading.Tasks;
using System.Collections.Generic; // List için
using System.Linq; // Any() ve FirstOrDefault() için
using System; // ArgumentNullException için

namespace BlogStore.PresentationLayer.Services
{
    public class HuggingFaceService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _modelEndpoint;

        public HuggingFaceService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            // API anahtarını User Secrets'tan veya ortam değişkenlerinden okunduğundan emin olun.
            // appsettings.json'da APIKey'in artık olmaması gerekiyor!
            _apiKey = configuration["HuggingFace:ApiKey"];
            _modelEndpoint = configuration["HuggingFace:ModelEndpoint"];

            // API anahtarı veya model endpoint'i eksikse hata fırlat (başlangıçta)
            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new ArgumentNullException("HuggingFace:ApiKey is not configured. Ensure it's in User Secrets or environment variables.");
            }
            if (string.IsNullOrEmpty(_modelEndpoint))
            {
                throw new ArgumentNullException("HuggingFace:ModelEndpoint is not configured in appsettings.json.");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        // Hugging Face API'sinden beklediğimiz yanıt formatını temsil eden sınıflar.
        // Bu sınıflar, kullandığınız modelin döndüğü JSON'a göre değişebilir!
        // fc63/turkzh-toxic-language-detection modeli genellikle etiketleri LABEL_0, LABEL_1 olarak döner.
        // Hangisinin 'toxic' olduğunu Debug çıktısından teyit etmeniz gerekecek.
        public class Prediction
        {
            [JsonProperty("label")] // JSON'daki anahtar adı
            public string Label { get; set; }

            [JsonProperty("score")] // JSON'daki anahtar adı
            public float Score { get; set; }
        }

        public async Task<float?> GetToxicityScoreAsync(string commentText)
        {
            // API'ye gönderilecek JSON payload'ı
            var json = $"{{\"inputs\": \"{commentText}\"}}";
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(_modelEndpoint, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"HuggingFace API Hatası: {response.StatusCode} - {errorContent}");
                    // Hata durumunda null döndür
                    return null;
                }

                var responseString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"HuggingFace API Yanıtı: {responseString}"); // Konsola/Çıktı penceresine yanıtı yazdır

                // API yanıtını ayrıştır
                // fc63/turkzh-toxic-language-detection modeli genellikle bu formatta yanıt verir:
                // [[{"label":"LABEL_0", "score":0.99}, {"label":"LABEL_1", "score":0.01}]]
                var results = JsonConvert.DeserializeObject<List<List<Prediction>>>(responseString);

                if (results != null && results.Any() && results[0].Any())
                {
                    // Şimdi burada, LABEL_0 mı yoksa LABEL_1 mi toksikliği temsil ediyor, bunu belirlemeliyiz.
                    // Genellikle, "positive" (olumlu) veya "non-toxic" (toksik olmayan) gibi etiketler LABEL_0'a,
                    // "negative" (olumsuz) veya "toxic" (toksik) gibi etiketler LABEL_1'e denk gelebilir.
                    // En doğrusu, Debug çıktısında gördüğünüz JSON'daki label değerlerini kontrol etmektir.

                    // Şimdilik LABEL_1'in toksik olduğunu varsayalım ve yüksek skorunu arayalım.
                    // VEYA, her iki label'ı da alıp, 'toxic' olana ait skoru döndürelim.

                    // Önce 'toxic' etiketli olanı bulmaya çalışalım.
                    // Eğer model çıktı etiketleri LABEL_0, LABEL_1 gibiyse,
                    // bu kontrol başarısız olabilir ve aşağıdaki yorumlu kısım devreye girer.
                    var toxicPrediction = results[0].FirstOrDefault(p =>
                        p.Label.Equals("toxic", StringComparison.OrdinalIgnoreCase) ||
                        p.Label.Equals("saldırgan", StringComparison.OrdinalIgnoreCase) ||
                        p.Label.Equals("abusive", StringComparison.OrdinalIgnoreCase) ||
                        p.Label.Equals("küfür", StringComparison.OrdinalIgnoreCase) ||
                        p.Label.Equals("LABEL_1", StringComparison.OrdinalIgnoreCase) // LABEL_1'in toksik olma ihtimali
                    );

                    if (toxicPrediction != null)
                    {
                        return toxicPrediction.Score;
                    }

                    // Eğer yukarıdaki etiketlerden hiçbiri bulunamazsa,
                    // en yüksek skora sahip olanı döndürmek veya null döndürmek bir seçenek olabilir.
                    // Genellikle sınıflandırma modellerinde, toksiklik gibi bir sınıfın skoru yüksekse,
                    // o sınıfı temsil eden değeri döndürürüz.

                    // Geçici olarak en yüksek skorlu tahmini döndürebiliriz:
                    return results[0].OrderByDescending(p => p.Score).FirstOrDefault()?.Score;
                }
            }
            catch (JsonException ex)
            {
                System.Diagnostics.Debug.WriteLine($"HuggingFace API Yanıtı JSON ayrıştırma hatası: {ex.Message}");
                return null;
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"HuggingFace API HTTP isteği hatası: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Genel hata oluştu: {ex.Message}");
                return null;
            }

            return null; // Hiçbir skor bulunamazsa veya beklenmedik bir durum olursa
        }
    }
}