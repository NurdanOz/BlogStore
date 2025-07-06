using BlogStore.BusinessLayer.Abstract;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics; // Debug.WriteLine için bu using'i ekleyin!

namespace BlogStore.BusinessLayer.Concrete
{
    public class TranslationManager : ITranslationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _huggingFaceModelUrl;
        private readonly string _huggingFaceApiToken;

        public TranslationManager(HttpClient httpClient, IConfiguration configuration)
        {
            _huggingFaceModelUrl = configuration["HuggingFaceTranslate:ModelEndpoint"];
            _huggingFaceApiToken = configuration["HuggingFaceTranslate:ApiKey"];
            _httpClient = httpClient;

            // Null kontrolü yapalım, çünkü appsettings.json'dan okunan değerler null olabilir.
            if (string.IsNullOrEmpty(_huggingFaceApiToken) || string.IsNullOrEmpty(_huggingFaceModelUrl))
            {
                Debug.WriteLine("Hugging Face Translate API Key or Model URL is missing in appsettings.json!");
                // Hata fırlatmak veya başka bir şekilde yönetmek gerekebilir.
                // Şimdilik sadece Debug.WriteLine yapıyoruz.
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _huggingFaceApiToken);
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
        }

        public async Task<string> TranslateToEnglishAsync(string turkishText)
        {
            // Debugging için bu değerleri de kontrol edelim
            Debug.WriteLine($"Translation URL: {_huggingFaceModelUrl}");
            Debug.WriteLine($"Translation API Key (first 5 chars): {_huggingFaceApiToken?.Substring(0, Math.Min(5, _huggingFaceApiToken.Length)) ?? "N/A"}...");


            if (string.IsNullOrEmpty(turkishText))
            {
                return string.Empty;
            }

            var requestBody = new { inputs = turkishText };
            var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            try // Try-catch ekleyelim ki network hatalarını yakalayalım
            {
                var response = await _httpClient.PostAsync(_huggingFaceModelUrl, jsonContent);

                if (response.IsSuccessStatusCode) // <-- BURAYA KIRMIZI NOKTA KOY (1. NOKTA - Başarı)
                {
                    var responseString = await response.Content.ReadAsStringAsync(); // <-- BURAYA KIRMIZI NOKTA KOY (2. NOKTA - Başarı String)
                    // Yanıt genellikle: [{"translation_text": "translated text"}]
                    var result = JsonConvert.DeserializeObject<List<TranslationResponse>>(responseString);
                    Debug.WriteLine($"Translation API Response: {responseString}"); // Yanıtı logla
                    return result?.FirstOrDefault()?.translation_text;
                }
                else // <-- BURAYA KIRMIZI NOKTA KOY (3. NOKTA - Hata)
                {
                    var errorContent = await response.Content.ReadAsStringAsync(); // <-- BURAYA KIRMIZI NOKTA KOY (4. NOKTA - Hata String)
                    Debug.WriteLine($"Translation API Error: {response.StatusCode} - {response.ReasonPhrase} - {errorContent}");
                    return string.Empty; // Hata durumunda boş döndür
                }
            }
            catch (HttpRequestException httpEx)
            {
                // Network veya bağlantı hatalarını yakala
                Debug.WriteLine($"HttpRequestException in TranslationManager: {httpEx.Message}");
                if (httpEx.InnerException != null)
                {
                    Debug.WriteLine($"Inner Exception: {httpEx.InnerException.Message}");
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                // Diğer tüm hataları yakala
                Debug.WriteLine($"General Exception in TranslationManager: {ex.Message}");
                return string.Empty;
            }
        }

        private class TranslationResponse
        {
            public string translation_text { get; set; }
        }
    }
}