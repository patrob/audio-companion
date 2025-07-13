
# Audio Engineering Blazor MAUI Hybrid Instructions

Instructions for building high-performance audio engineering applications with .NET MAUI, Blazor Hybrid, and C#. Optimized for low-latency audio analysis, mixer integration, and professional-grade audio workflows on desktop and mobile.

## Project Context

• .NET MAUI with Blazor Hybrid for cross-platform desktop/mobile audio engineering UI
• C# for real-time audio processing and analysis
• Performance-critical audio processing with <10ms latency requirements
• Integration with professional audio mixers via OSC protocol (M32/X32)
• Real-time spectrum analysis, harshness detection, and AI-powered coaching
• Multi-channel audio recording and playback capabilities
• Core Audio/ASIO/WASAPI integration for professional audio interfaces

## Development Standards

### Audio Processing Performance

• **Async/Await for Audio I/O**: Use async patterns for all audio device operations to avoid blocking the UI thread
• **Background Services for DSP**: Offload CPU-intensive audio analysis (FFT, spectrum analysis) to background threads or Task.Run
• **Optimized Audio Buffers**: Use `Span<float>`/`Memory<float>` for audio data to minimize allocations and maximize performance
• **Circular Buffers**: Implement ring buffers for real-time audio streaming to minimize garbage collection
• **Audio Context Management**: Reuse audio engine instances and properly dispose/suspend to manage resources
• **Sample Rate Optimization**: Match processing sample rates to hardware to avoid expensive resampling

### C# Audio-Specific Types

• **Audio Parameter Interfaces**: Define strict interfaces/classes for all audio parameters (gain, frequency, Q-factor)

```csharp
public interface IAudioChannel
{
    int Id { get; }
    DecibelValue Gain { get; set; } // Branded type for type safety
    List<EQBand> Eq { get; set; }
    bool Muted { get; set; }
    bool Solo { get; set; }
}
```

• **Branded Types for Audio Units**: Use custom structs for frequency (Hz), decibels (dB), and time (ms) to prevent unit confusion
• **Generic Audio Processing**: Implement generic types for audio processing pipelines and effects chains
• **OSC Protocol Types**: Define exact types for mixer OSC commands and responses
• **Audio Format Validation**: Use type-safe validation for audio sample rates, bit depths, and channel configurations

### Real-Time Audio UI Components

• **Memoized Spectrum Components**: Use `@key` and efficient rendering patterns for spectrum analyzers and level meters that update at 60fps
• **Optimized Event Handlers**: Use `EventCallback` and `Action` delegates for audio parameter change handlers to prevent unnecessary re-renders
• **Audio-Specific Custom Components**: Create reusable components like `<AudioLevelMeter>`, `<SpectrumDisplay>`, `<MixerConnection>`
• **Debounced Audio Controls**: Implement debouncing for continuous controls (faders, knobs) to reduce processing overhead
• **Canvas-Based Visualizations**: Use SkiaSharp or MAUI.Graphics for high-frequency audio visualizations
• **Virtualized Audio Lists**: Use virtualization for channel strips and large mixer surfaces

### Memory Management for Audio

• **Audio Buffer Pooling**: Reuse audio buffer objects to minimize garbage collection during real-time processing
• **Cleanup Audio Resources**: Always dispose of audio engines, streams, and background threads on component disposal
• **WeakReference for Audio References**: Use `WeakReference<T>` to store audio node references without preventing garbage collection
• **Monitor Audio Memory Usage**: Track heap usage during audio processing and implement memory pressure handling
• **Streaming Audio I/O**: Use streams for large audio file operations to avoid loading entire files into memory

### Mixer Integration (OSC Protocol)

• **Connection Management**: Implement robust OSC connection handling with automatic reconnection
• **Command Queuing**: Queue mixer commands to prevent overwhelming the mixer with rapid parameter changes
• **Parameter Synchronization**: Maintain local state synchronized with mixer hardware state
• **Error Recovery**: Handle mixer disconnections gracefully and provide clear feedback to users
• **Command Validation**: Validate all OSC commands before sending to prevent mixer errors

### AI Audio Analysis

• **Feature Extraction**: Extract audio features (spectrum, RMS, peak levels) for AI analysis rather than sending raw audio
• **Rate Limiting**: Implement intelligent rate limiting for AI API calls to balance responsiveness and cost
• **Caching Analysis Results**: Cache AI coaching suggestions for similar audio characteristics
• **Batch Processing**: Group audio analysis requests when possible to improve AI API efficiency
• **Offline Fallback**: Provide rule-based audio analysis when AI services are unavailable

### Audio File Management

• **Streaming Recording**: Use .NET streams for multi-track recording to handle large files efficiently
• **Audio Format Support**: Support professional formats (WAV, AIFF, BWF) with proper metadata handling
• **Metadata Preservation**: Maintain audio metadata (timecode, track names, markers) throughout processing
• **Concurrent File Operations**: Handle multiple simultaneous recordings without blocking
• **Audio File Validation**: Validate audio file integrity and format compatibility before processing

### Performance Monitoring

• **Audio Latency Tracking**: Monitor and log audio processing latency to ensure <10ms performance
• **Buffer Underrun Detection**: Detect and handle audio buffer underruns with appropriate recovery
• **CPU Usage Monitoring**: Track CPU usage during audio processing and warn when approaching limits
• **Memory Leak Detection**: Implement audio-specific memory leak detection for long-running sessions
• **Real-Time Performance Metrics**: Display real-time performance metrics for audio engineers

## Framework-Specific Guidelines

### .NET MAUI Audio Processing

• **Avoid Blocking Operations**: Never use synchronous file operations in audio processing paths
• **Use Native Modules**: Leverage native audio libraries (NAudio, CoreAudio, WASAPI, ASIO) for optimal performance
• **Task-Based Parallelism**: Use `Task.Run` or background services for parallel audio analysis tasks
• **UI Thread Safety**: Use `MainThread.BeginInvokeOnMainThread` for UI updates from background audio threads
• **Process Priority**: Set higher process priority for audio applications when possible

### Blazor Audio Components

• **Audio State Management**: Use Cascading Parameters or Dependency Injection for global audio state (mixer settings, recording status)
• **Component Optimization**: Use `@key` and efficient rendering for all audio visualization components
• **Effect Cleanup**: Always cleanup audio-related effects and event listeners
• **Error Boundaries**: Implement audio-specific error boundaries for graceful failure handling
• **Suspense for Audio Loading**: Use loading indicators for audio resources and mixer connections

### MAUI/Blazor Audio Integration

• **Main Process Audio**: Handle audio device enumeration and low-level operations in the main process or platform-specific services
• **IPC Audio Communication**: Use efficient messaging patterns for audio data between .NET MAUI and Blazor components
• **Native Menu Integration**: Provide audio controls in native menus for better user experience
• **System Audio Integration**: Integrate with system audio routing and device management
• **Auto-Updater Considerations**: Handle audio processing during application updates gracefully

## Audio-Specific Error Handling

• **Device Error Recovery**: Handle audio device disconnections with automatic device re-scanning
• **Buffer Overflow Protection**: Implement protection against audio buffer overflows
• **Sample Rate Mismatch**: Handle sample rate mismatches between devices gracefully
• **Mixer Communication Errors**: Provide clear feedback for mixer connection and command errors
• **Audio Processing Failures**: Implement fallback processing when primary audio analysis fails

## Testing Audio Applications

• **Audio Buffer Testing**: Test audio processing with various buffer sizes and sample rates
• **Latency Testing**: Implement automated latency testing for audio processing chains
• **Device Simulation**: Mock audio devices for testing without hardware dependencies
• **Load Testing**: Test application performance under high audio processing loads
• **Integration Testing**: Test full audio pipeline from input to output with real hardware

## Common Audio Pitfalls

• Using UI thread for high-frequency audio visualizations (use SkiaSharp/Canvas instead)
• Not cleaning up audio engine and stream resources
• Blocking the UI thread with synchronous audio operations
• Insufficient error handling for audio device changes
• Not implementing proper audio buffer management
• Ignoring audio latency and jitter in processing chains

## Audio Performance Troubleshooting

• Use Visual Studio Profiler or dotTrace to identify audio processing bottlenecks
• Monitor audio thread priority and CPU affinity
• Use audio-specific profiling tools (NAudio analyzers, custom logging)
• Implement real-time audio performance dashboards
• Log audio processing timing for offline analysis

## Professional Audio Best Practices

• **Sample Accurate Timing**: Ensure sample-accurate timing for all audio operations
• **Professional Metering**: Implement proper peak, RMS, and LUFS metering standards
• **Audio Standards Compliance**: Follow professional audio standards (AES, SMPTE, EBU)
• **Metadata Standards**: Support BWF and other professional metadata standards
• **Automation Support**: Implement mixer automation recording and playback
• **Session Management**: Provide comprehensive session save/restore functionality

## Code Review Checklist for Audio Applications

□ Are audio buffers properly sized and managed?
□ Is audio processing happening off the UI thread?
□ Are all audio resources cleaned up properly?
□ Is error handling comprehensive for device disconnections?
□ Are audio parameters properly typed and validated?
□ Is latency monitoring implemented and within acceptable limits?
□ Are mixer commands properly queued and validated?
□ Is memory usage monitored during long audio sessions?
□ Are audio file operations using streaming where appropriate?
□ Is the audio processing pipeline optimized for real-time performance?

## Practical Audio Examples

### Example 1: Optimized Audio Buffer Processing

```csharp
// BAD: Synchronous processing blocking UI thread
public float[] ProcessAudio(float[] inputBuffer)
{
    return PerformFFT(inputBuffer); // Blocks UI thread
}

// GOOD: Background thread-based processing
public class AudioProcessor
{
    public async Task<float[]> ProcessAudioAsync(float[] inputBuffer)
    {
        return await Task.Run(() => PerformFFT(inputBuffer));
    }
}
```

### Example 2: Efficient Mixer Parameter Updates

```csharp
// BAD: Immediate OSC commands for every parameter change
slider.ValueChanged += (s, e) =>
{
    mixer.SendOSC($"/ch/01/mix/fader {e.NewValue}");
};

// GOOD: Debounced parameter updates
private readonly Debouncer _debouncer = new Debouncer(50);
slider.ValueChanged += (s, e) =>
{
    _debouncer.Debounce(() => mixer.SendOSC($"/ch/01/mix/fader {e.NewValue}"));
};
```

### Example 3: Memory-Efficient Audio Visualization

```csharp
// BAD: Creating new canvas elements for each frame
void UpdateSpectrum(float[] data)
{
    var canvas = new SKCanvas(new SKBitmap(800, 200));
    // Process and render...
}

// GOOD: Reusing canvas context with efficient rendering
class SpectrumDisplay
{
    private SKCanvas _canvas;
    private SKBitmap _bitmap;

    public void UpdateSpectrum(float[] data)
    {
        _canvas.Clear();
        // Efficient rendering using existing context...
    }
}
```

## References and Further Reading

• [.NET MAUI Documentation](https://learn.microsoft.com/en-us/dotnet/maui/)
• [Blazor Hybrid Documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/hybrid/)
• [NAudio Library](https://github.com/naudio/NAudio)
• [OSC Protocol Specification](http://opensoundcontrol.org/spec-1_0)
• [Professional Audio Development Guidelines](https://www.aes.org/standards/)
• [Real-Time Audio Programming in .NET](https://github.com/naudio/NAudio)
• [SkiaSharp for Graphics](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/graphics/skia)

## Conclusion

Audio engineering applications demand the highest performance standards. Always prioritize real-time performance, implement comprehensive error handling, and maintain professional audio standards. Use these guidelines to build responsive, reliable, and professional-grade audio applications that audio engineers can depend on in critical situations.

