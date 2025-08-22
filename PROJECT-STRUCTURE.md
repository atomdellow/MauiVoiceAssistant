# MauiVoiceAssistant Project Structure

## Overview
This project demonstrates a complete voice assistant implementation using .NET 8 MAUI, Vue 3, and OpenAI API integration.

## Current Implementation Status

### ✅ Completed Components

1. **ProxyApi** (Fully Functional)
   - .NET 8 Minimal API with OpenAI integration
   - `/stt` endpoint for speech-to-text (Whisper)
   - `/chat` endpoint for chat completions (GPT-4o-mini)
   - `/tts` endpoint for text-to-speech (TTS-1 with "alloy" voice)
   - CORS configuration for MAUI app communication
   - Configuration management for OpenAI API key

2. **VueUI** (Fully Functional)
   - Vue 3 single-page application (no TypeScript)
   - Beautiful responsive UI with mic button
   - Recording states and animations
   - Bridge communication ready for MAUI integration
   - Builds successfully to `dist/` folder

3. **Console Demo App** (Functional)
   - Demonstrates the service architecture
   - Shows dependency injection setup
   - Validates all services can be instantiated
   - Includes Vue UI files in build output

### 🔧 Architecture Components (Reference Implementation)

4. **MAUI Services** (Reference Code)
   - `AudioService.cs` - Simplified for console demo, full implementation in `.example` file
   - `ProxyApiClient.cs` - HTTP client for API communication
   - `OpenAIService.cs` - Orchestrates transcription, chat, and TTS
   - `VoiceViewModel.cs` - MVVM pattern implementation

5. **MAUI Pages** (Example Files)
   - `VoicePage.cs.example` - Shows HybridWebView integration
   - `VoicePage.xaml.example` - UI layout for voice interface
   - Bridge implementation for JavaScript ↔ .NET communication

## File Structure

```
MauiVoiceAssistant/
├── ProxyApi/                           # ✅ .NET 8 Minimal API
│   ├── Program.cs                      # OpenAI endpoints implementation
│   ├── ProxyApi.csproj                 # Project with OpenAI package
│   └── appsettings.Development.json.template
│
├── VueUI/                              # ✅ Vue 3 Frontend
│   ├── src/
│   │   ├── App.vue                     # Main voice interface
│   │   └── main.js                     # Vue app entry point
│   ├── index.html                      # App HTML template
│   ├── vite.config.js                  # Build configuration
│   ├── package.json                    # Dependencies and scripts
│   └── dist/                           # Build output (copied to MAUI)
│
├── MauiApp/                            # 🔧 Console Demo + References
│   ├── Program.cs                      # Console app demonstrating services
│   ├── Services/
│   │   ├── AudioService.cs             # Simplified console version
│   │   ├── AudioService.cs.example     # Full MAUI implementation
│   │   ├── ProxyApiClient.cs           # HTTP client for API calls
│   │   └── OpenAIService.cs            # Orchestration service
│   ├── ViewModels/
│   │   └── VoiceViewModel.cs           # MVVM implementation
│   ├── Pages/
│   │   ├── VoicePage.cs.example        # HybridWebView implementation
│   │   └── VoicePage.xaml.example      # XAML layout
│   └── Resources/Raw/webapp/           # Vue build output
│
├── build-vue.sh                       # ✅ Build script
├── README.md                           # ✅ Comprehensive documentation
└── MauiVoiceAssistant.sln             # ✅ Solution file
```

## Development Workflow

### Working Components
1. **Build Vue UI**: `./build-vue.sh` or `cd VueUI && npm run build`
2. **Run ProxyApi**: `cd ProxyApi && dotnet run`
3. **Run Console Demo**: `cd MauiApp && dotnet run`

### For Full MAUI Implementation
To convert this to a full MAUI app:

1. **Install MAUI Workload**: `dotnet workload install maui`
2. **Update MauiApp.csproj**: Use the MAUI SDK and packages
3. **Replace AudioService.cs**: Use the `.example` version with Plugin.Maui.Audio
4. **Add VoicePage**: Use the `.example` files for HybridWebView integration
5. **Configure Platforms**: Android and iOS specific configurations are included

## Key Features Demonstrated

### ✅ Working Features
- OpenAI API integration (STT, Chat, TTS)
- Vue 3 responsive UI with animations
- Service-oriented architecture
- Dependency injection
- Build automation
- Cross-platform compatibility

### 🔧 Reference Implementations
- Native audio recording/playback
- JavaScript ↔ .NET bridge communication
- HybridWebView integration
- Mobile permissions handling
- MVVM pattern with data binding

## Testing

### Current Testing Capabilities
1. **ProxyApi**: Run with `dotnet run` and test endpoints via Swagger UI
2. **Vue UI**: Run with `npm run dev` for standalone testing
3. **Console Demo**: Validates service architecture and dependency injection
4. **Integration**: Vue files are properly copied to MAUI resources

### Production Deployment
For a production MAUI app, replace the console Program.cs with the full MAUI implementation using the `.example` files as templates.

## API Key Configuration

The ProxyApi requires an OpenAI API key. Configure it in:
- `ProxyApi/appsettings.Development.json` (copy from template)
- Or set the `OPENAI_API_KEY` environment variable

## Dependencies

### ProxyApi
- OpenAI SDK 2.3.0
- ASP.NET Core 8.0

### VueUI
- Vue 3.5.19
- Vite 7.1.3 (build tool)

### MauiApp (Console Demo)
- Microsoft.Extensions.Hosting 8.0.0
- Microsoft.Extensions.Logging.Console 8.0.0

### MauiApp (Full Implementation)
- Microsoft.Maui.Controls 8.0.3
- CommunityToolkit.Maui 7.0.1
- Plugin.Maui.Audio 2.1.0
- Microsoft.AspNetCore.Components.WebView.Maui 8.0.3