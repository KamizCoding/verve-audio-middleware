using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

public class AudioUploader
{
    private readonly HttpClient _httpClient;
    private readonly string _apiUrl;

    public AudioUploader(string apiUrl)
    {
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromMinutes(15)
        };
        _apiUrl = apiUrl;
    }

    public async Task<(string Original, string Translated)> UploadAudioAsync(Stream audioStream, string fileName)
    {
        try
        {
            using var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(audioStream);
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("audio/webm");
            content.Add(fileContent, "file", fileName);
            content.Add(new StringContent("Tamil"), "audioLanguage");

            var response = await _httpClient.PostAsync(_apiUrl, content);
            response.EnsureSuccessStatusCode();

            var resultJson = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Translation Result: " + resultJson);

            using var jsonDoc = JsonDocument.Parse(resultJson);
            var root = jsonDoc.RootElement;

            var original = root.GetProperty("original_text").GetString() ?? "";
            var translated = root.GetProperty("translated_text").GetString() ?? "";

            return (original, translated);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Upload failed: {ex.Message}");
            Console.WriteLine("📌 STACK TRACE: " + ex.StackTrace);
            return ("", "");
        }
    }
}
