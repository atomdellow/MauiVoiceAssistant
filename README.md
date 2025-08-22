# MauiVoiceAssistant

A Voice Assistant using OpenAI API integrated with .NET 8 MAUI and Vue 3 UI.

## Features

- **Voice Recording**: Native microphone recording using Plugin.Maui.Audio
- **Speech-to-Text**: OpenAI Whisper integration via ProxyApi
- **Chat**: OpenAI GPT-4o-mini chat completions 
- **Text-to-Speech**: OpenAI TTS with "alloy" voice
- **Vue 3 UI**: Beautiful, responsive interface with mic button and reply display
- **Cross-Platform**: Works on Android and iOS

## Project Structure

```
MauiVoiceAssistant/
├── MauiApp/                    # .NET 8 MAUI project
│   ├── Services/               # Audio, API client, OpenAI services
│   ├── ViewModels/             # MVVM ViewModels
│   ├── Pages/                  # XAML pages with HybridWebView
│   └── Resources/Raw/webapp/   # Vue build output (auto-generated)
├── ProxyApi/                   # .NET 8 Minimal API server
├── VueUI/                      # Vue 3 frontend (no TypeScript)
└── build-vue.sh               # Build script
```

## Prerequisites

- .NET 8 SDK
- Node.js 18+ and npm
- Android SDK (for Android development)
- Xcode (for iOS development on macOS)
- OpenAI API key

## Setup Instructions

### 1. Clone and Setup

```bash
git clone <repository-url>
cd MauiVoiceAssistant
```

### 2. Configure OpenAI API Key

Create the development configuration file:

```bash
cp ProxyApi/appsettings.Development.json.template ProxyApi/appsettings.Development.json
```

Edit `ProxyApi/appsettings.Development.json` and add your OpenAI API key:

```json
{
  "OPENAI_API_KEY": "sk-your-actual-openai-api-key-here"
}
```

**Alternative**: Set environment variable:
```bash
export OPENAI_API_KEY="sk-your-actual-openai-api-key-here"
```

### 3. Install Dependencies

Install Vue dependencies:

```bash
cd VueUI
npm install
cd ..
```

Restore .NET packages:

```bash
dotnet restore
```

### 4. Build Vue UI

```bash
./build-vue.sh
```

Or manually:

```bash
cd VueUI
npm run build
cd ..
cp -r VueUI/dist/* MauiApp/Resources/Raw/webapp/
```

### 5. Run ProxyApi

```bash
cd ProxyApi
dotnet run
```

The API will be available at `http://localhost:5000` with Swagger UI at `http://localhost:5000/swagger`.

### 6. Run MAUI App

**For Android Emulator:**

```bash
cd MauiApp
dotnet build -f net8.0-android
dotnet run -f net8.0-android
```

**For iOS Simulator (macOS only):**

```bash
cd MauiApp
dotnet build -f net8.0-ios
dotnet run -f net8.0-ios
```

## API Endpoints

The ProxyApi exposes three endpoints:

- `POST /stt` - Speech-to-text (Whisper)
- `POST /chat` - Chat completion (GPT-4o-mini)  
- `POST /tts` - Text-to-speech (TTS-1 with "alloy" voice)

## Development Workflow

1. **Make Vue UI changes**: Edit files in `VueUI/`
2. **Rebuild Vue**: Run `./build-vue.sh` or `npm run build` in VueUI
3. **MAUI changes**: The webapp files are automatically included in the MAUI build
4. **Test**: Restart the MAUI app to see changes

## Architecture

### MAUI App
- **VoicePage**: Contains HybridWebView loading Vue UI
- **VoiceViewModel**: MVVM pattern for voice operations
- **AudioService**: Native audio recording/playback
- **ProxyApiClient**: HTTP client for API communication
- **OpenAIService**: Orchestrates transcription, chat, and TTS

### Vue UI
- **App.vue**: Single-page voice interface
- **Bridge Communication**: JavaScript ↔ .NET via `window.hybridBridge`
- **Responsive Design**: Works on mobile and desktop

### ProxyApi
- **Minimal API**: Lightweight .NET 8 web API
- **OpenAI Integration**: Direct integration with OpenAI SDK
- **CORS Enabled**: Allows MAUI app communication

## Permissions

### Android
- `RECORD_AUDIO` - For microphone access
- `INTERNET` - For API communication

### iOS  
- `NSMicrophoneUsageDescription` - Microphone usage description

Permissions are requested automatically when needed.

## Troubleshooting

### Common Issues

1. **"OPENAI_API_KEY not configured"**
   - Ensure the API key is set in `appsettings.Development.json` or environment variable

2. **"Failed to transcribe audio"**  
   - Check that ProxyApi is running and accessible
   - Verify API key is valid
   - Check network connectivity

3. **Vue UI not loading**
   - Run `./build-vue.sh` to rebuild Vue assets
   - Ensure files are in `MauiApp/Resources/Raw/webapp/`

4. **Microphone permission denied**
   - Grant microphone permission in device settings
   - On Android emulator, ensure virtual microphone is enabled

### Network Configuration

For Android Emulator, the ProxyApi URL is set to `http://10.0.2.2:5000` (emulator host).
For physical devices, update the IP address in `ProxyApiClient.cs` to your development machine's IP.

### Development Tips

- Use `dotnet watch run` in ProxyApi for hot reload during development
- Vue changes require rebuilding with `./build-vue.sh`
- Check console logs in both MAUI debugger and browser dev tools (if testing Vue separately)

## License

MIT License - see [LICENSE](LICENSE) file for details.