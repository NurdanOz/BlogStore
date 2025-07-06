using BlogStore.BusinessLayer.Abstract;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics; // Debug.WriteLine için bu using'i ekleyin!

namespace BlogStore.BusinessLayer.Concrete
{
    public class ToxicityManager : IToxicityDetectionService
    {
        private readonly HttpClient _httpClient;
        private readonly string _huggingFaceApiToken;
        private readonly string _huggingFaceModelUrl;

        public ToxicityManager(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _huggingFaceApiToken = configuration["HuggingFace:ApiKey"];
            _huggingFaceModelUrl = configuration["HuggingFace:ModelEndpoint"];

            if (string.IsNullOrEmpty(_huggingFaceApiToken) || string.IsNullOrEmpty(_huggingFaceModelUrl))
            {
                Debug.WriteLine("Hugging Face Toxicity API Key or Model URL is missing in appsettings.json!");
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _huggingFaceApiToken);
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
        }

        public async Task<ToxicityDetectionResult> DetectToxicityAsync(string commentText)
        {
            Debug.WriteLine($"Toxicity URL: {_huggingFaceModelUrl}");
            Debug.WriteLine($"Toxicity API Key (first 5 chars): {_huggingFaceApiToken?.Substring(0, Math.Min(5, _huggingFaceApiToken.Length)) ?? "N/A"}...");

            var requestBody = new { inputs = commentText };
            var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(_huggingFaceModelUrl, jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Toxicity API Response: {responseString}");

                    // JSON yanıtınız çift liste içinde ModelPrediction listesi olduğu için doğru deserialize ediyoruz
                    var modelResponse = JsonConvert.DeserializeObject<List<List<ModelPrediction>>>(responseString);

                    // modelResponse[0] içindeki en yüksek skorlu tahmini bul
                    var topPrediction = modelResponse[0].OrderByDescending(p => p.Score).FirstOrDefault();

                    // Toksisite mantığı:
                    // "Very Negative" veya "Negative" etiketlerinden biri en yüksek skora sahipse ve skor belirli bir eşiğin üzerindeyse toksik kabul et.
                    bool isToxic = false;
                    string detectedLabel = "Not Detected";
                    double score = 0;

                    if (topPrediction != null)
                    {
                        score = topPrediction.Score;
                        detectedLabel = topPrediction.Label;

                        // "Very Negative" veya "Negative" etiketlerini toksik olarak kabul edelim
                        if (topPrediction.Label.Equals("Very Negative", StringComparison.OrdinalIgnoreCase) ||
                            topPrediction.Label.Equals("Negative", StringComparison.OrdinalIgnoreCase))
                        {
                            // Toksik kabul etmek için bir eşik belirle
                            // Örneğin, 0.5 (yüzde 50) üzeri 'Negative' veya 'Very Negative' ise toksik kabul et.
                            isToxic = topPrediction.Score > 0.35;
                        }
                    }

                    return new ToxicityDetectionResult
                    {
                        IsToxic = isToxic,
                        Score = score,
                        DetectedLabel = detectedLabel
                    };
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Hugging Face Toxicity API Error: {response.StatusCode} - {response.ReasonPhrase} - {errorContent}");
                    return new ToxicityDetectionResult { IsToxic = false, Score = 0, DetectedLabel = "API Error" };
                }
            }
            catch (HttpRequestException httpEx)
            {
                Debug.WriteLine($"HttpRequestException in ToxicityManager: {httpEx.Message}");
                if (httpEx.InnerException != null)
                {
                    Debug.WriteLine($"Inner Exception: {httpEx.InnerException.Message}");
                }
                return new ToxicityDetectionResult { IsToxic = false, Score = 0, DetectedLabel = "Network Error" };
            }
            catch (JsonSerializationException jsonEx)
            {
                Debug.WriteLine($"JsonSerializationException in ToxicityManager: {jsonEx.Message}");
                // DEBUG: Eğer buraya düşüyorsa, responseString'i tekrar kontrol etmekte fayda var
                // Debug.WriteLine($"Problematic JSON (if available): {responseString}"); // responseString'i burada loglayabiliriz
                return new ToxicityDetectionResult { IsToxic = false, Score = 0, DetectedLabel = "JSON Parse Error" };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"General Exception in ToxicityManager: {ex.Message}");
                return new ToxicityDetectionResult { IsToxic = false, Score = 0, DetectedLabel = "General Error" };
            }
        }

        // Modelin JSON yanıtına uygun olarak bu sınıfları tanımlayın
        // Önceki tanımınız doğruydu
        private class ModelPrediction
        {
            public string Label { get; set; }
            public double Score { get; set; }
        }
    }
}