namespace MauiApp.Services;

public class OpenAIService
{
    private readonly ProxyApiClient _proxyApiClient;
    private readonly AudioService _audioService;

    public OpenAIService(ProxyApiClient proxyApiClient, AudioService audioService)
    {
        _proxyApiClient = proxyApiClient;
        _audioService = audioService;
    }

    public async Task<string> TranscribeAsync(string wavPath)
    {
        try
        {
            return await _proxyApiClient.TranscribeAsync(wavPath);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Transcription failed: {ex.Message}", ex);
        }
    }

    public async Task<string> ChatAsync(string text)
    {
        try
        {
            return await _proxyApiClient.ChatAsync(text);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Chat failed: {ex.Message}", ex);
        }
    }

    public async Task SpeakAsync(string text)
    {
        try
        {
            var audioPath = await _proxyApiClient.TextToSpeechAsync(text);
            await _audioService.PlayAudioAsync(audioPath);
            
            // Clean up the temporary file
            if (File.Exists(audioPath))
            {
                try
                {
                    File.Delete(audioPath);
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Text-to-speech failed: {ex.Message}", ex);
        }
    }
}