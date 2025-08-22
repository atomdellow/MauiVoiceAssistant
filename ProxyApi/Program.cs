using OpenAI;
using OpenAI.Audio;
using OpenAI.Chat;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS for MAUI app
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Configure OpenAI
var openAIApiKey = builder.Configuration["OPENAI_API_KEY"] ?? 
                   Environment.GetEnvironmentVariable("OPENAI_API_KEY");

if (string.IsNullOrEmpty(openAIApiKey))
{
    throw new InvalidOperationException("OPENAI_API_KEY is not configured. Please set it in appsettings.Development.json or as an environment variable.");
}

var openAI = new OpenAIClient(openAIApiKey);
builder.Services.AddSingleton(openAI);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpsRedirection();

// Speech-to-Text endpoint
app.MapPost("/stt", async (IFormFile audioFile, OpenAIClient client) =>
{
    try
    {
        using var stream = audioFile.OpenReadStream();
        
        var options = new AudioTranscriptionOptions
        {
            ResponseFormat = AudioTranscriptionFormat.Text
        };

        var response = await client.GetAudioClient("whisper-1").TranscribeAudioAsync(stream, audioFile.FileName, options);
        return Results.Ok(new { text = response.Value.Text });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("SpeechToText")
.WithOpenApi()
.DisableAntiforgery();

// Chat endpoint
app.MapPost("/chat", async (ChatRequest request, OpenAIClient client) =>
{
    try
    {
        var chatClient = client.GetChatClient("gpt-4o-mini");
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage("You are a helpful voice assistant. Keep your responses concise and conversational."),
            new UserChatMessage(request.Text)
        };

        var response = await chatClient.CompleteChatAsync(messages);
        
        return Results.Ok(new { text = response.Value.Content[0].Text });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("Chat")
.WithOpenApi();

// Text-to-Speech endpoint
app.MapPost("/tts", async (TtsRequest request, OpenAIClient client) =>
{
    try
    {
        var audioClient = client.GetAudioClient("tts-1");
        
        var response = await audioClient.GenerateSpeechAsync(request.Text, GeneratedSpeechVoice.Alloy);
        
        // Save to temporary file
        var tempFileName = $"tts_{Guid.NewGuid()}.mp3";
        var tempPath = Path.Combine(Path.GetTempPath(), tempFileName);
        
        using (var fileStream = File.Create(tempPath))
        {
            await response.Value.ToStream().CopyToAsync(fileStream);
        }
        
        return Results.Ok(new { audioPath = tempPath });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("TextToSpeech")
.WithOpenApi();

app.Run();

public record ChatRequest(string Text);
public record TtsRequest(string Text);
