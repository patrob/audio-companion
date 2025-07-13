# Audio Engineering Node.js TypeScript Instructions

Instructions for building high-performance audio engineering applications with Node.js, TypeScript, and real-time audio processing capabilities. Optimized for low-latency audio analysis, mixer integration, and AI-powered coaching systems.

## Project Context

• Node.js with TypeScript for real-time audio processing and analysis
• Electron + React for professional audio engineering desktop interface
• Performance-critical audio processing with <10ms latency requirements
• Integration with professional audio mixers via OSC protocol (M32/X32)
• Real-time spectrum analysis, harshness detection, and AI-powered coaching
• Multi-channel audio recording and playback capabilities
• DANTE/Core Audio integration for professional audio interfaces

## Development Standards

### Audio Processing Performance

• **Async/Await for Audio I/O**: Use async patterns for all audio device operations to avoid blocking the main thread
• **Web Workers for DSP**: Offload CPU-intensive audio analysis (FFT, spectrum analysis) to Web Workers
• **Optimized Audio Buffers**: Use Float32Array for audio data - it's optimized for WebAudio API and reduces memory allocation
• **Circular Buffers**: Implement ring buffers for real-time audio streaming to minimize garbage collection
• **Audio Context Management**: Reuse AudioContext instances and properly suspend/resume to manage resources
• **Sample Rate Optimization**: Match processing sample rates to hardware to avoid expensive resampling

### TypeScript Audio-Specific Types

• **Audio Parameter Interfaces**: Define strict interfaces for all audio parameters (gain, frequency, Q-factor)

```typescript
interface AudioChannel {
  readonly id: number;
  gain: DecibelValue; // Branded type for type safety
  eq: EQBand[];
  muted: boolean;
  solo: boolean;
}
```

• **Branded Types for Audio Units**: Use branded types for frequency (Hz), decibels (dB), and time (ms) to prevent unit confusion
• **Generic Audio Processing**: Implement generic types for audio processing pipelines and effects chains
• **OSC Protocol Types**: Define exact types for mixer OSC commands and responses
• **Audio Format Validation**: Use type guards for validating audio sample rates, bit depths, and channel configurations

### Real-Time Audio UI Components

• **Memoized Spectrum Components**: Use React.memo for spectrum analyzers and level meters that update at 60fps
• **Optimized Event Handlers**: Use useCallback for audio parameter change handlers to prevent unnecessary re-renders
• **Audio-Specific Custom Hooks**: Create hooks like `useAudioLevel`, `useSpectrumData`, `useMixerConnection`
• **Debounced Audio Controls**: Implement debouncing for continuous controls (faders, knobs) to reduce processing overhead
• **Canvas-Based Visualizations**: Use Canvas API instead of DOM elements for high-frequency audio visualizations
• **Virtualized Audio Lists**: Use virtualization for channel strips and large mixer surfaces

### Memory Management for Audio

• **Audio Buffer Pooling**: Reuse audio buffer objects to minimize garbage collection during real-time processing
• **Cleanup Audio Resources**: Always dispose of AudioNodes, MediaStreams, and Worker threads on component unmount
• **WeakMap for Audio References**: Use WeakMap to store audio node references without preventing garbage collection
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

• **Streaming Recording**: Use Node.js streams for multi-track recording to handle large files efficiently
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

### Node.js Audio Processing

• **Avoid Blocking Operations**: Never use synchronous file operations (readFileSync) in audio processing paths
• **Use Native Modules**: Leverage native audio modules (node-portaudio, node-speaker) for optimal performance
• **Worker Thread Pool**: Implement a worker thread pool for parallel audio analysis tasks
• **Event Loop Monitoring**: Monitor event loop lag during audio processing and adjust workload accordingly
• **Process Priority**: Set higher process priority for audio applications when possible

### React Audio Components

• **Audio State Management**: Use Context API for global audio state (mixer settings, recording status)
• **Component Optimization**: Use React.memo for all audio visualization components
• **Effect Cleanup**: Always cleanup audio-related effects and event listeners
• **Error Boundaries**: Implement audio-specific error boundaries for graceful failure handling
• **Suspense for Audio Loading**: Use Suspense for loading audio resources and mixer connections

### Electron Audio Integration

• **Main Process Audio**: Handle audio device enumeration and low-level operations in the main process
• **IPC Audio Communication**: Use efficient IPC patterns for audio data between main and renderer processes
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

• Using DOM updates for high-frequency audio visualizations (use Canvas instead)
• Not cleaning up AudioContext and MediaStream resources
• Blocking the main thread with synchronous audio operations
• Insufficient error handling for audio device changes
• Not implementing proper audio buffer management
• Ignoring audio latency and jitter in processing chains

## Audio Performance Troubleshooting

• Use Chrome DevTools' Performance tab to identify audio processing bottlenecks
• Monitor audio thread priority and CPU affinity
• Use audio-specific profiling tools (Web Audio Inspector, audio buffer analyzers)
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
□ Is audio processing happening off the main thread?
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

```typescript
// BAD: Synchronous processing blocking main thread
function processAudio(inputBuffer: Float32Array): Float32Array {
  return performFFT(inputBuffer); // Blocks main thread
}

// GOOD: Worker-based processing
class AudioProcessor {
  private worker: Worker;

  async processAudio(inputBuffer: Float32Array): Promise<Float32Array> {
    return new Promise((resolve) => {
      this.worker.postMessage({ buffer: inputBuffer });
      this.worker.onmessage = (e) => resolve(e.data.result);
    });
  }
}
```

### Example 2: Efficient Mixer Parameter Updates

```typescript
// BAD: Immediate OSC commands for every parameter change
slider.onChange = (value) => {
  mixer.sendOSC(`/ch/01/mix/fader ${value}`);
};

// GOOD: Debounced parameter updates
const debouncedUpdate = useMemo(
  () =>
    debounce((value: number) => {
      mixer.sendOSC(`/ch/01/mix/fader ${value}`);
    }, 50),
  [mixer]
);
```

### Example 3: Memory-Efficient Audio Visualization

```typescript
// BAD: Creating new canvas elements for each frame
function updateSpectrum(data: Float32Array) {
  const canvas = document.createElement("canvas");
  // Process and render...
}

// GOOD: Reusing canvas context with efficient rendering
class SpectrumDisplay {
  private canvas: HTMLCanvasElement;
  private ctx: CanvasRenderingContext2D;

  updateSpectrum(data: Float32Array) {
    this.ctx.clearRect(0, 0, this.canvas.width, this.canvas.height);
    // Efficient rendering using existing context...
  }
}
```

## References and Further Reading

• [Web Audio API Specification](https://webaudio.github.io/web-audio-api/)
• [Node.js Audio Processing Best Practices](https://nodejs.org/en/docs/guides/simple-profiling/)
• [OSC Protocol Specification](http://opensoundcontrol.org/spec-1_0)
• [Professional Audio Development Guidelines](https://www.aes.org/standards/)
• [Real-Time Audio Programming](https://developer.mozilla.org/en-US/docs/Web/API/Web_Audio_API/Advanced_techniques)
• [TypeScript Audio Type Definitions](https://github.com/DefinitelyTyped/DefinitelyTyped)
• [Electron Audio Integration](https://www.electronjs.org/docs/latest/api/desktop-capturer)
• [React Performance Optimization](https://react.dev/learn/render-and-commit)

## Conclusion

Audio engineering applications demand the highest performance standards. Always prioritize real-time performance, implement comprehensive error handling, and maintain professional audio standards. Use these guidelines to build responsive, reliable, and professional-grade audio applications that audio engineers can depend on in critical situations.

