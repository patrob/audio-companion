# Audio Companion MVP - Product Requirements Document

## 1. Product Overview

### Vision

A real-time audio analysis tool that helps audio engineers identify vocal harshness, monitor frequency spectrum, and achieve balanced mixes through visual feedback and template-based mixing guidance.

### Target Users

- Audio engineers (mixing & mastering)
- Music producers
- Podcasters and content creators
- Live sound engineers

## 2. Core Features (MVP)

### 2.1 Audio Input Management

- **Audio Device Selection**: Dropdown to select from macOS Core Audio input devices
- **Input Monitoring**: Real-time level monitoring with peak/RMS meters
- **Sample Rate Support**: 44.1kHz, 48kHz, 96kHz
- **Buffer Size Control**: Adjustable latency (64-512 samples)

### 2.2 Real-Time Spectrum Analysis

- **FFT-Based Spectrum Analyzer**:
  - Frequency range: 20Hz - 20kHz
  - Resolution: 1024-4096 bins
  - Update rate: 30-60 FPS
- **EQ-Style Visualization**:
  - 31-band or octave-based display
  - Color-coded frequency bands
  - Peak hold and averaging options

### 2.3 Decibel Monitoring

- **Real-time dB Levels**: Peak, RMS, and LUFS measurement
- **Headroom Indicator**: Visual warning for approaching 0dBFS
- **Historical Metering**: Short-term level history graph

### 2.4 Vocal Harshness Detection

- **Frequency Analysis**: Focus on 2-8kHz range where harshness occurs
- **Sibilance Detection**: Automated detection of excessive 4-8kHz energy
- **Visual Alerts**: Color-coded warnings for harsh frequencies
- **Threshold Settings**: User-adjustable sensitivity

### 2.5 Solo/Monitoring Features

- **Audio Passthrough**: Monitor input audio with minimal latency
- **Solo Mode**: Isolate specific frequency ranges for detailed analysis
- **Frequency Filtering**: High-pass/low-pass filters for isolation

## 3. Technical Architecture

### 3.1 Technology Stack

- **Runtime**: .NET 9 with C#
- **App Framework**: .NET MAUI with Blazor Hybrid (cross-platform desktop app)
- **Audio Processing**:
  - Native Core Audio integration via .NET bindings (e.g., CoreAudioKit, NAudio, or custom interop)
  - FFT and DSP using C# libraries (e.g., Math.NET Numerics, custom DSP)
- **UI Framework**: Blazor Hybrid (Razor components) with MAUI for native controls
- **Visualization**:
  - SkiaSharp or Microsoft.Maui.Graphics for spectrum and meters
  - Custom Blazor components for interactive UI
- **Real-time Processing**: Background threads/tasks in .NET for audio analysis

### 3.2 Core Audio Integration

```csharp
public record AudioInputDevice(string Id, string Name, int Channels, int SampleRate);

public interface IAudioProcessor
{
    Task SelectDeviceAsync(string deviceId);
    void StartProcessing();
    void StopProcessing();
    float[] GetSpectrum();
    (float Peak, float Rms) GetLevel();
}
```

### 3.3 Signal Processing Pipeline

```
Audio Input → Core Audio → Buffer → FFT Analysis → Spectrum Data → UI Visualization
                                 → Level Analysis → dB Meters → UI Display
                                 → Harshness Detection → Alerts → UI Warnings
```

## 4. User Interface Design

### 4.1 Main Dashboard Layout

- **Top Bar**: Device selection, sample rate, buffer size
- **Center Panel**: Large spectrum analyzer with EQ-style bands
- **Right Panel**:
  - dB meters (Peak/RMS/LUFS)
  - Harshness detection alerts
  - Solo controls
- **Bottom Panel**: Transport controls, settings

### 4.2 Visual Design Requirements

- **Color Scheme**: Dark theme optimized for studio environments
- **Spectrum Colors**:
  - Green: Normal levels (-∞ to -20dB)
  - Yellow: Caution (-20 to -6dB)
  - Red: Peak/harsh frequencies (-6 to 0dB)
- **Responsive Design**: Scalable UI for different screen sizes

## 5. Mixing Templates (Future Enhancement)

### 5.1 Template System

- **Preset EQ Curves**: Rock, Pop, Jazz, Classical, Podcast
- **Target Frequency Response**: Visual overlay on spectrum
- **Deviation Indicators**: Show how current mix differs from template
- **Template Matching**: Suggest adjustments to match target curve

### 5.2 Harshness Detection Algorithms

- **Frequency Band Monitoring**:
  - 2-4kHz: Nasal/boxy frequencies
  - 4-6kHz: Presence/harshness
  - 6-8kHz: Sibilance
- **Temporal Analysis**: Detect sudden spikes in harsh frequencies
- **Comparative Analysis**: Compare against "ideal" vocal spectrum

## 6. Technical Specifications

### 6.1 Performance Requirements

- **Latency**: < 10ms input to display
- **CPU Usage**: < 15% on modern MacBook Pro
- **Memory**: < 200MB RAM usage
- **Update Rate**: 60fps for smooth visualization

### 6.2 File Structure

```
audio-companion/
├── AudioCompanion.sln
├── src/
│   ├── AudioCompanion.App/           # .NET MAUI Blazor Hybrid app project
│   │   ├── Audio/
│   │   │   ├── InputManager.cs
│   │   │   ├── SpectrumAnalyzer.cs
│   │   │   ├── LevelMeter.cs
│   │   │   └── HarshnessDetector.cs
│   │   ├── UI/
│   │   │   ├── Components/
│   │   │   ├── Spectrum/
│   │   │   └── Meters/
│   │   ├── Templates/
│   │   │   └── MixingPresets.cs
│   │   ├── Main.razor
│   │   └── ...
├── tests/
│   └── AudioCompanion.Tests/         # xUnit/NUnit/MSTest test project
└── README.md
```

## 7. Dependencies

### 7.1 Core Dependencies

- **.NET 9 SDK**
- **.NET MAUI** (for cross-platform UI)
- **Blazor Hybrid** (for web/native UI integration)
- **SkiaSharp** or **Microsoft.Maui.Graphics** (for custom drawing/visualization)
- **Math.NET Numerics** (for FFT and DSP)
- **CoreAudioKit** or custom Core Audio interop (for macOS audio device access)
- **xUnit/NUnit/MSTest** (for unit testing)

### 7.2 Audio Processing Libraries

- **FFT & DSP**: Math.NET Numerics or custom C# implementation
- **Audio Buffer Management**: Custom C# circular buffer
- **Core Audio Bindings**: CoreAudioKit, NAudio, or custom P/Invoke for macOS

## 8. Development Phases

### Phase 1 (MVP - 2-3 weeks)

- [ ] Basic audio input selection
- [ ] Real-time spectrum analyzer
- [ ] Simple dB metering
- [ ] Basic UI framework

### Phase 2 (Enhancement - 2-3 weeks)

- [ ] Harshness detection algorithms
- [ ] Solo/monitoring features
- [ ] Improved UI/UX
- [ ] Settings persistence

### Phase 3 (Templates - 3-4 weeks)

- [ ] Mixing template system
- [ ] Preset EQ curves
- [ ] Advanced analysis features
- [ ] Export/import functionality

## 9. Success Metrics

### 9.1 Technical Metrics

- Audio latency < 10ms
- Spectrum accuracy within 1dB
- 60fps visualization performance
- 99.9% uptime (no crashes)

### 9.2 User Experience Metrics

- Time to identify harshness < 5 seconds
- Mixing template matching accuracy > 85%
- User satisfaction with visual feedback

## 10. Future Enhancements

- Multi-channel support (stereo/surround)
- Plugin architecture for custom processors
- Cloud-based template sharing
- Integration with DAW automation
- Mobile companion app
- AI-powered mixing suggestions

---

## Getting Started


### Prerequisites

- macOS 12+ (for Core Audio and .NET MAUI support)
- .NET 9 SDK
- Visual Studio 2022+ (with MAUI workload)
- Xcode Command Line Tools (for building on macOS)

### Initial Setup

```bash

# Install .NET 9 SDK and MAUI workload
dotnet workload install maui

# Clone the repo and restore dependencies
git clone <repo-url>
cd audio-companion
dotnet restore

# Build and run the app
dotnet build src/AudioCompanion.App/AudioCompanion.App.csproj
dotnet run --project src/AudioCompanion.App/AudioCompanion.App.csproj
```

This MVP focuses on core functionality that audio engineers need most: real-time spectrum analysis, harshness detection, and visual feedback for better mixing decisions.

