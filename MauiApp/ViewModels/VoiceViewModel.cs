using System.ComponentModel;
using System.Runtime.CompilerServices;
using MauiApp.Services;

namespace MauiApp.ViewModels;

public class VoiceViewModel : INotifyPropertyChanged
{
    private readonly AudioService _audioService;
    private readonly OpenAIService _openAIService;
    
    private bool _isRecording;
    private bool _isProcessing;
    private string _currentRecordingPath = string.Empty;

    public VoiceViewModel(AudioService audioService, OpenAIService openAIService)
    {
        _audioService = audioService;
        _openAIService = openAIService;
    }

    public bool IsRecording
    {
        get => _isRecording;
        private set => SetProperty(ref _isRecording, value);
    }

    public bool IsProcessing
    {
        get => _isProcessing;
        private set => SetProperty(ref _isProcessing, value);
    }

    public async Task<string> StartRecordingAsync()
    {
        try
        {
            if (IsRecording)
                return _currentRecordingPath;

            _currentRecordingPath = await _audioService.StartRecordingAsync();
            IsRecording = true;
            
            return _currentRecordingPath;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to start recording: {ex.Message}", ex);
        }
    }

    public async Task<string> StopAndTranscribeAsync()
    {
        try
        {
            if (!IsRecording)
                throw new InvalidOperationException("No active recording session.");

            IsRecording = false;
            IsProcessing = true;

            var recordingPath = await _audioService.StopRecordingAsync();
            var transcribedText = await _openAIService.TranscribeAsync(recordingPath);

            // Clean up the recording file
            try
            {
                if (File.Exists(recordingPath))
                    File.Delete(recordingPath);
            }
            catch
            {
                // Ignore cleanup errors
            }

            return transcribedText;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to stop recording and transcribe: {ex.Message}", ex);
        }
        finally
        {
            IsProcessing = false;
        }
    }

    public async Task<string> GetReplyAndSpeakAsync(string userText)
    {
        try
        {
            IsProcessing = true;

            var assistantReply = await _openAIService.ChatAsync(userText);
            
            // Start speech synthesis without waiting for it to complete
            _ = Task.Run(async () =>
            {
                try
                {
                    await _openAIService.SpeakAsync(assistantReply);
                }
                catch (Exception ex)
                {
                    // Log speech synthesis errors but don't fail the whole operation
                    System.Diagnostics.Debug.WriteLine($"Speech synthesis error: {ex.Message}");
                }
                finally
                {
                    IsProcessing = false;
                }
            });

            return assistantReply;
        }
        catch (Exception ex)
        {
            IsProcessing = false;
            throw new InvalidOperationException($"Failed to get reply: {ex.Message}", ex);
        }
    }

    public void StopAllOperations()
    {
        try
        {
            if (IsRecording)
            {
                _audioService.StopRecordingAsync().GetAwaiter().GetResult();
                IsRecording = false;
            }

            _audioService.StopPlayback();
            IsProcessing = false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error stopping operations: {ex.Message}");
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
            return false;

        backingStore = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}