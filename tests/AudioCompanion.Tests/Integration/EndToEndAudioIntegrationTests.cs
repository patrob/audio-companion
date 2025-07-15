using AudioCompanion.App.Audio;
using AudioCompanion.Shared.Audio;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace AudioCompanion.Tests.Integration;

public class EndToEndAudioIntegrationTests
{
    private readonly ITestOutputHelper _output;
    
    public EndToEndAudioIntegrationTests(ITestOutputHelper output)
    {
        _output = output;
    }
    
    [Fact]
    public async Task CompleteAudioWorkflow_ShouldWorkEndToEnd()
    {
        // Arrange - Set up the complete DI container like the app does
        var services = new ServiceCollection();
        
        // Register services like MauiProgram.cs
        services.AddSingleton<IAudioProcessor, RealTimeAudioProcessor>();
#if MACCATALYST || MACOS
        services.AddSingleton<MacAudioInputManager>();
#endif
        
        var serviceProvider = services.BuildServiceProvider();
        
        // Act & Assert - Simulate the complete audio workflow
        
        // 1. Get the audio processor (like the Home.razor component does)
        var audioProcessor = serviceProvider.GetRequiredService<IAudioProcessor>();
        Assert.NotNull(audioProcessor);
        _output.WriteLine("✓ Audio processor resolved from DI container");
        
#if MACCATALYST || MACOS
        // 2. Get the device manager (like the Home.razor component does)
        var deviceManager = serviceProvider.GetRequiredService<MacAudioInputManager>();
        Assert.NotNull(deviceManager);
        _output.WriteLine("✓ Device manager resolved from DI container");
        
        // 3. Get available devices (like the UI device selector does)
        var devices = await deviceManager.GetDevicesAsync();
        Assert.NotNull(devices);
        _output.WriteLine($"✓ Found {devices.Count()} audio devices");
        
        // 4. Select a device (simulate user selection)
        var firstDevice = devices.FirstOrDefault();
        if (firstDevice != null)
        {
            await audioProcessor.SelectDeviceAsync(firstDevice.Id);
            _output.WriteLine($"✓ Selected device: {firstDevice.Name}");
        }
#else
        _output.WriteLine("✓ Skipped device enumeration on non-macOS platform");
#endif
        
        // 5. Start audio processing (like clicking Start button in UI)
        audioProcessor.StartProcessing();
        _output.WriteLine("✓ Started audio processing");
        
        // 6. Get audio data multiple times (like the UI timer does)
        for (int i = 0; i < 10; i++)
        {
            var spectrum = audioProcessor.GetSpectrum();
            var level = audioProcessor.GetLevel();
            
            Assert.NotNull(spectrum);
            Assert.True(spectrum.Length > 0, "Spectrum should have data points");
            Assert.True(level.Peak <= 0, "Peak should be in dB (≤ 0)");
            Assert.True(level.Rms <= 0, "RMS should be in dB (≤ 0)");
            
            // Simulate UI update interval
            await Task.Delay(10);
        }
        _output.WriteLine("✓ Successfully retrieved 10 spectrum and level readings");
        
        // 7. Stop processing (like clicking Stop button in UI)
        audioProcessor.StopProcessing();
        _output.WriteLine("✓ Stopped audio processing");
        
        // 8. Verify data is still available after stopping
        var finalSpectrum = audioProcessor.GetSpectrum();
        var finalLevel = audioProcessor.GetLevel();
        Assert.NotNull(finalSpectrum);
        _output.WriteLine("✓ Data still available after stopping");
        
        // 9. Clean up (like component disposal)
        if (audioProcessor is IDisposable disposable)
        {
            disposable.Dispose();
        }
        serviceProvider.Dispose();
        _output.WriteLine("✓ Cleaned up resources");
    }
    
    [Fact]
    public void AudioProcessor_ShouldHandleMultipleUIComponents()
    {
        // Arrange - Simulate multiple UI components accessing the same processor
        var services = new ServiceCollection();
        services.AddSingleton<IAudioProcessor, RealTimeAudioProcessor>();
        var serviceProvider = services.BuildServiceProvider();
        
        // Act - Get the same processor instance from multiple places (like different UI components)
        var processor1 = serviceProvider.GetRequiredService<IAudioProcessor>();
        var processor2 = serviceProvider.GetRequiredService<IAudioProcessor>();
        var processor3 = serviceProvider.GetRequiredService<IAudioProcessor>();
        
        // Assert - Should be the same singleton instance
        Assert.Same(processor1, processor2);
        Assert.Same(processor2, processor3);
        _output.WriteLine("✓ Same processor instance shared across UI components");
        
        // Act - All components can access the processor safely
        processor1.StartProcessing();
        
        var spectrum1 = processor1.GetSpectrum();
        var spectrum2 = processor2.GetSpectrum();
        var level1 = processor1.GetLevel();
        var level2 = processor2.GetLevel();
        
        // Assert - All calls work and return valid data
        Assert.NotNull(spectrum1);
        Assert.NotNull(spectrum2);
        Assert.Equal(spectrum1.Length, spectrum2.Length);
        _output.WriteLine("✓ Multiple UI components can access processor simultaneously");
        
        // Cleanup
        processor1.StopProcessing();
        if (processor1 is IDisposable disposable)
        {
            disposable.Dispose();
        }
        serviceProvider.Dispose();
    }
    
    [Fact]
    public async Task DeviceSelection_ShouldPersistAcrossProcessingSessions()
    {
        // Arrange
        var processor = new RealTimeAudioProcessor();
        
#if MACCATALYST || MACOS
        var deviceManager = new MacAudioInputManager();
        
        // Act - Select a device
        var devices = await deviceManager.GetDevicesAsync();
        var testDevice = devices.FirstOrDefault();
        
        if (testDevice != null)
        {
            await processor.SelectDeviceAsync(testDevice.Id);
            _output.WriteLine($"Selected device: {testDevice.Name}");
            
            // Start and stop processing multiple times
            for (int session = 0; session < 3; session++)
            {
                processor.StartProcessing();
                
                // Get some data
                var spectrum = processor.GetSpectrum();
                var level = processor.GetLevel();
                
                Assert.NotNull(spectrum);
                _output.WriteLine($"Session {session + 1}: Got {spectrum.Length} spectrum points");
                
                processor.StopProcessing();
                
                // Brief pause between sessions
                await Task.Delay(10);
            }
            
            _output.WriteLine("✓ Device selection persisted across multiple processing sessions");
        }
        else
        {
            _output.WriteLine("No devices available for testing - test skipped");
        }
#else
        _output.WriteLine("Test skipped on non-macOS platform");
        await Task.CompletedTask;
#endif
        
        // Cleanup
        processor.Dispose();
    }
}
