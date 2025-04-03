using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
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

    public async Task UploadAudioAsync(Stream audioStream, string fileName)
    {
        try
        {
            using var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(audioStream);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("audio/webm");
            content.Add(fileContent, "file", fileName);

            var response = await _httpClient.PostAsync(_apiUrl, content);
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Translation Result: " + result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Upload failed: {ex.Message}");
            Console.WriteLine("📌 STACK TRACE: " + ex.StackTrace);
        }
    }
}
