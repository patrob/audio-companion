# Audio Companion MVP

A real-time audio analysis tool that helps audio engineers identify vocal harshness, monitor frequency spectrum, and achieve balanced mixes through visual feedback and template-based mixing guidance.

## 🎯 Project Overview

Audio Companion is a cross-platform desktop application built with .NET 9 MAUI and Blazor Hybrid, designed for:

- **Audio engineers** (mixing & mastering)
- **Music producers** 
- **Podcasters and content creators**
- **Live sound engineers**

## ✨ Core Features (MVP)

### Current Implementation
- ✅ **Audio Device Selection**: Dropdown to select from macOS Core Audio input devices
- ✅ **Clean Architecture**: SOLID principles with dependency injection
- ✅ **Cross-Platform**: .NET MAUI Blazor Hybrid (macOS, Windows, iOS, Android)
- ✅ **Test-Driven Development**: Comprehensive unit test coverage

### Planned Features
- 🚧 **Real-Time Spectrum Analysis**: FFT-based analyzer (20Hz - 20kHz, 30-60 FPS)
- 🚧 **Decibel Monitoring**: Peak, RMS, and LUFS measurement with headroom indicators
- 🚧 **Vocal Harshness Detection**: Focus on 2-8kHz range with visual alerts
- 🚧 **Solo/Monitoring**: Audio passthrough and frequency isolation

## 🏗️ Architecture

```
AudioCompanion/
├── src/
│   ├── AudioCompanion.App/           # .NET MAUI Blazor Hybrid app
│   │   ├── Audio/                    # Platform-specific implementations
│   │   ├── Components/               # Blazor UI components
│   │   └── Platforms/                # Platform-specific code
│   └── AudioCompanion.Shared/        # Shared domain logic & interfaces
│       └── Audio/                    # Core audio interfaces
├── tests/
│   └── AudioCompanion.Tests/         # Unit tests (xUnit)
└── prd.md                           # Product Requirements Document
```

### Key Design Patterns
- **Interface Segregation**: Clean separation between domain and platform code
- **Dependency Injection**: Platform-specific implementations registered via DI
- **Record Types**: Immutable data structures for audio device representation
- **Test-Driven Development**: Red-Green-Refactor cycle with comprehensive coverage

## 🚀 Getting Started

### Prerequisites

- **macOS 12+** (for Core Audio and .NET MAUI support)
- **.NET 9 SDK**
- **Visual Studio 2022+ with MAUI workload** or **VS Code**
- **Xcode Command Line Tools** (for macOS building)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/patrob/audio-companion.git
   cd audio-companion
   ```

2. **Install .NET MAUI workload** (if not already installed)
   ```bash
   dotnet workload install maui
   ```

3. **Restore dependencies**
   ```bash
   dotnet restore
   ```

4. **Run tests**
   ```bash
   dotnet test
   ```

5. **Build for macOS**
   ```bash
   dotnet build src/AudioCompanion.App/AudioCompanion.App.csproj -f net9.0-maccatalyst
   ```

6. **Run the application**
   ```bash
   dotnet run --project src/AudioCompanion.App/AudioCompanion.App.csproj -f net9.0-maccatalyst
   ```

### Platform Setup

#### For iOS/macOS Development
```bash
sudo xcodebuild -runFirstLaunch
```

#### For Android Development
```bash
dotnet workload install android
```

## 🧪 Testing

The project follows TDD principles with comprehensive test coverage:

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/AudioCompanion.Tests/AudioCompanion.Tests.csproj

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

Current test coverage includes:
- Audio device enumeration
- Device selection UI logic
- Platform-specific implementations

## 🛠️ Development

### Core Interfaces

```csharp
// Audio device representation
public record AudioInputDevice(string Id, string Name, int Channels, int SampleRate);

// Device enumeration
public interface IAudioInputManager
{
    Task<List<AudioInputDevice>> EnumerateDevicesAsync();
}

// Audio processing pipeline
public interface IAudioProcessor
{
    Task SelectDeviceAsync(string deviceId);
    void StartProcessing();
    void StopProcessing();
    float[] GetSpectrum();
    (float Peak, float Rms) GetLevel();
}
```

### Platform-Specific Implementation

The `MacAudioInputManager` demonstrates how to implement platform-specific audio device enumeration using AVFoundation:

```csharp
#if MACCATALYST || MACOS
public class MacAudioInputManager : IAudioInputManager
{
    public Task<List<AudioInputDevice>> EnumerateDevicesAsync()
    {
        // AVFoundation implementation for macOS
    }
}
#endif
```

## 📊 Technical Specifications

### Performance Targets
- **Latency**: < 10ms input to display
- **CPU Usage**: < 15% on modern MacBook Pro  
- **Memory**: < 200MB RAM usage
- **Update Rate**: 60fps for smooth visualization

### Technology Stack
- **.NET 9** - Runtime and base framework
- **.NET MAUI** - Cross-platform application framework
- **Blazor Hybrid** - Web UI in native container
- **AVFoundation** - macOS audio device access
- **xUnit** - Unit testing framework

## 🎨 UI Design

The application features a studio-optimized dark theme with:

- **Top Bar**: Device selection, sample rate, buffer size controls
- **Center Panel**: Large spectrum analyzer with EQ-style bands  
- **Right Panel**: dB meters (Peak/RMS/LUFS) and harshness detection alerts
- **Color Coding**: Green (normal), Yellow (caution), Red (peak/harsh)

## 📈 Roadmap

### Phase 1 (Current - MVP)
- [x] Basic project structure and architecture
- [x] Audio device selection
- [x] Platform-specific implementations
- [x] Unit test framework
- [ ] Real-time spectrum analyzer
- [ ] Simple dB metering

### Phase 2 (Enhancement)
- [ ] Harshness detection algorithms
- [ ] Solo/monitoring features  
- [ ] Advanced UI/UX
- [ ] Settings persistence

### Phase 3 (Templates)
- [ ] Mixing template system
- [ ] Preset EQ curves (Rock, Pop, Jazz, Classical, Podcast)
- [ ] Template matching and deviation indicators
- [ ] Export/import functionality

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Write tests for your changes (TDD approach)
4. Implement your feature
5. Ensure all tests pass (`dotnet test`)
6. Commit your changes (`git commit -m 'Add amazing feature'`)
7. Push to the branch (`git push origin feature/amazing-feature`)
8. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Built following principles from **Clean Code**, **The Pragmatic Programmer**, and **Refactoring**
- Inspired by professional audio engineering workflows
- Uses modern .NET development practices and SOLID principles

---

**Status**: 🚧 MVP in active development  
**Last Updated**: July 2025
