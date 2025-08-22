using System.Text;
using System.Text.Json;

namespace MauiApp.Services;

public class ProxyApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public ProxyApiClient()
    {
        _httpClient = new HttpClient();
        
        // For demo purposes, use localhost
        _baseUrl = "http://localhost:5000";
    }

    public async Task<string> TranscribeAsync(string wavFilePath)
    {
        try
        {
            using var form = new MultipartFormDataContent();
            var fileBytes = await File.ReadAllBytesAsync(wavFilePath);
            var fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("audio/wav");
            form.Add(fileContent, "audioFile", Path.GetFileName(wavFilePath));

            var response = await _httpClient.PostAsync($"{_baseUrl}/stt", form);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<TranscriptionResponse>(jsonResponse, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });

            return result?.Text ?? string.Empty;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to transcribe audio: {ex.Message}", ex);
        }
    }

    public async Task<string> ChatAsync(string text)
    {
        try
        {
            var request = new ChatRequest { Text = text };
            var jsonContent = JsonSerializer.Serialize(request);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/chat", content);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ChatResponse>(jsonResponse, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });

            return result?.Text ?? string.Empty;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to get chat response: {ex.Message}", ex);
        }
    }

    public async Task<string> TextToSpeechAsync(string text)
    {
        try
        {
            var request = new TtsRequest { Text = text };
            var jsonContent = JsonSerializer.Serialize(request);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/tts", content);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<TtsResponse>(jsonResponse, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });

            if (string.IsNullOrEmpty(result?.AudioPath))
                throw new InvalidOperationException("No audio path returned from TTS service");

            // Download the audio file to local cache if it's a URL
            if (result.AudioPath.StartsWith("http"))
            {
                var audioResponse = await _httpClient.GetAsync(result.AudioPath);
                audioResponse.EnsureSuccessStatusCode();
                
                var audioBytes = await audioResponse.Content.ReadAsByteArrayAsync();
                var localPath = Path.Combine(Path.GetTempPath(), $"tts_{Guid.NewGuid()}.mp3");
                await File.WriteAllBytesAsync(localPath, audioBytes);
                
                return localPath;
            }

            return result.AudioPath;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to convert text to speech: {ex.Message}", ex);
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}

public class TranscriptionResponse
{
    public string Text { get; set; } = string.Empty;
}

public class ChatRequest
{
    public string Text { get; set; } = string.Empty;
}

public class ChatResponse
{
    public string Text { get; set; } = string.Empty;
}

public class TtsRequest
{
    public string Text { get; set; } = string.Empty;
}

public class TtsResponse
{
    public string AudioPath { get; set; } = string.Empty;
}