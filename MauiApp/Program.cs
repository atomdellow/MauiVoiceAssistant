using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MauiApp.Services;
using MauiApp.ViewModels;

namespace MauiApp;

class Program
{
    static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        
        var logger = host.Services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("MauiVoiceAssistant Demo Console App Started");
        
        // Demonstrate the services are properly configured
        var audioService = host.Services.GetRequiredService<AudioService>();
        var proxyApiClient = host.Services.GetRequiredService<ProxyApiClient>();
        var openAIService = host.Services.GetRequiredService<OpenAIService>();
        var voiceViewModel = host.Services.GetRequiredService<VoiceViewModel>();
        
        logger.LogInformation("All services registered successfully!");
        logger.LogInformation("Audio Service: {AudioService}", audioService.GetType().Name);
        logger.LogInformation("Proxy API Client: {ProxyApiClient}", proxyApiClient.GetType().Name);
        logger.LogInformation("OpenAI Service: {OpenAIService}", openAIService.GetType().Name);
        logger.LogInformation("Voice ViewModel: {VoiceViewModel}", voiceViewModel.GetType().Name);
        
        // Check if Vue UI files exist
        var webappPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Raw", "webapp", "index.html");
        if (File.Exists(webappPath))
        {
            logger.LogInformation("Vue UI files found at: {WebappPath}", webappPath);
        }
        else
        {
            logger.LogWarning("Vue UI files not found. Run ./build-vue.sh to build the Vue UI.");
        }
        
        logger.LogInformation("This console app demonstrates the MauiVoiceAssistant architecture.");
        logger.LogInformation("For the full mobile experience, deploy this as a proper MAUI app.");
        logger.LogInformation("Make sure the ProxyApi is running for full functionality.");
        
        await host.RunAsync();
    }
    
    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                // Register services (simplified versions for console demo)
                services.AddSingleton<AudioService>();
                services.AddSingleton<ProxyApiClient>();
                services.AddSingleton<OpenAIService>();
                services.AddTransient<VoiceViewModel>();
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            });
}