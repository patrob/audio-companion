using AudioCompanion.Shared.Audio;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Shouldly;

namespace AudioCompanion.Tests.Audio;

public class RealTimeAudioIntegrationTests
{
    [Fact]
    public void RealTimeAudioProcessor_ShouldBeRegisteredCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Act - Register the real-time processor like in MauiProgram
        services.AddSingleton<IAudioProcessor, RealTimeAudioProcessor>();
        var serviceProvider = services.BuildServiceProvider();
        
        // Assert
        var audioProcessor = serviceProvider.GetService<IAudioProcessor>();
        Assert.NotNull(audioProcessor);
        Assert.IsType<RealTimeAudioProcessor>(audioProcessor);
    }
    
    [Fact]
    public void RealTimeAudioProcessor_ShouldImplementCorrectInterface()
    {
        // Arrange & Act
        var processor = new RealTimeAudioProcessor();
        
        // Assert
        Assert.IsAssignableFrom<IAudioProcessor>(processor);
        Assert.IsAssignableFrom<IDisposable>(processor);
    }
    
    [Fact]
    public void RealTimeAudioProcessor_ShouldProvideDefaultValues_WhenNotProcessing()
    {
        // Arrange
        var processor = new RealTimeAudioProcessor();
        
        // Act
        var spectrum = processor.GetSpectrum();
        var level = processor.GetLevel();
        
        // Assert
        Assert.NotNull(spectrum);
        Assert.True(spectrum.Length > 0, "Spectrum should have data points");
        Assert.True(level.Peak <= 0, "Peak should be in dB (negative or zero)");
        Assert.True(level.Rms <= 0, "RMS should be in dB (negative or zero)");
        
        // Cleanup
        processor.Dispose();
    }
    
    [Fact]
    public async Task RealTimeAudioProcessor_ShouldHandleDeviceSelection()
    {
        // Arrange
        var processor = new RealTimeAudioProcessor();
        
        // Act & Assert - Should not throw
        await processor.SelectDeviceAsync("test-device-id");
        
        // Cleanup
        processor.Dispose();
    }
    
    [Fact]
    public void RealTimeAudioProcessor_ShouldHandleStartStopProcessing()
    {
        // Arrange
        var processor = new RealTimeAudioProcessor();
        
        // Act & Assert - Should not throw
        processor.StartProcessing();
        processor.StopProcessing();
        
        // Cleanup
        processor.Dispose();
    }
    
    [Fact]
    public void RealTimeAudioProcessor_ShouldDisposeCleanly()
    {
        // Arrange
        var processor = new RealTimeAudioProcessor();
        
        // Act & Assert - Should not throw
        processor.StartProcessing();
        processor.Dispose();
        
        // Multiple dispose calls should be safe
        processor.Dispose();
    }
}

public class RealTimeAudioIntegrationTests
{
    [Fact]
    public void RealTimeAudioProcessor_CanBeResolvedFromDI()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        
#if MACCATALYST || MACOS
        services.AddSingleton<IAudioProcessor, RealTimeAudioProcessor>();
        services.AddSingleton<IAudioInputManager, MacAudioInputManager>();
#else
        services.AddSingleton<IAudioProcessor, RealTimeAudioProcessor>();
        services.AddSingleton<IAudioInputManager, MockAudioInputManager>();
#endif
        
        var serviceProvider = services.BuildServiceProvider();
        
        // Act
        var audioProcessor = serviceProvider.GetService<IAudioProcessor>();
        var audioInputManager = serviceProvider.GetService<IAudioInputManager>();
        
        // Assert
        Assert.NotNull(audioProcessor);
        Assert.NotNull(audioInputManager);
        Assert.IsType<RealTimeAudioProcessor>(audioProcessor);
    }
    
    [Fact]
    public async Task RealTimeAudioProcessor_ImplementsIAudioProcessorInterface()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        
#if MACCATALYST || MACOS
        services.AddSingleton<IAudioProcessor, RealTimeAudioProcessor>();
        services.AddSingleton<IAudioInputManager, MacAudioInputManager>();
#else
        services.AddSingleton<IAudioProcessor, RealTimeAudioProcessor>();
        services.AddSingleton<IAudioInputManager, MockAudioInputManager>();
#endif
        
        var serviceProvider = services.BuildServiceProvider();
        var audioProcessor = serviceProvider.GetRequiredService<IAudioProcessor>();
        
        // Act & Assert - Test interface methods exist and can be called
        var spectrum = audioProcessor.GetSpectrum();
        Assert.NotNull(spectrum);
        Assert.True(spectrum.Length > 0);
        
        var levels = audioProcessor.GetLevel();
        Assert.True(levels.Peak <= 0); // Should be negative dB or 0
        Assert.True(levels.Rms <= 0);  // Should be negative dB or 0
        
        // Test async device selection (should not throw)
        await audioProcessor.SelectDeviceAsync("test-device");
        
        // Test start/stop (should not throw)
        audioProcessor.StartProcessing();
        audioProcessor.StopProcessing();
    }
    
    [Fact]
    public async Task AudioInputManager_CanEnumerateDevices()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        
#if MACCATALYST || MACOS
        services.AddSingleton<IAudioInputManager, MacAudioInputManager>();
#else
        services.AddSingleton<IAudioInputManager, MockAudioInputManager>();
#endif
        
        var serviceProvider = services.BuildServiceProvider();
        var audioInputManager = serviceProvider.GetRequiredService<IAudioInputManager>();
        
        // Act
        var devices = await audioInputManager.GetDevicesAsync();
        
        // Assert
        Assert.NotNull(devices);
        // Should have at least one device (even if it's a mock/default device)
        Assert.True(devices.Any());
        
        // Verify each device has required properties
        foreach (var device in devices)
        {
            Assert.False(string.IsNullOrEmpty(device.Id));
            Assert.False(string.IsNullOrEmpty(device.Name));
            Assert.True(device.Channels > 0);
            Assert.True(device.SampleRate > 0);
        }
    }
    
    [Fact]
    public void RealTimeAudioProcessor_ReturnsValidSpectrumData()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        
#if MACCATALYST || MACOS
        services.AddSingleton<IAudioProcessor, RealTimeAudioProcessor>();
        services.AddSingleton<IAudioInputManager, MacAudioInputManager>();
#else
        services.AddSingleton<IAudioProcessor, RealTimeAudioProcessor>();
        services.AddSingleton<IAudioInputManager, MockAudioInputManager>();
#endif
        
        var serviceProvider = services.BuildServiceProvider();
        var audioProcessor = serviceProvider.GetRequiredService<IAudioProcessor>();
        
        // Act
        var spectrum = audioProcessor.GetSpectrum();
        
        // Assert
        Assert.NotNull(spectrum);
        Assert.True(spectrum.Length > 0);
        
        // Spectrum should be in a reasonable range
        foreach (var value in spectrum)
        {
            Assert.True(value >= 0.0f);    // Magnitude should be positive
            Assert.True(value <= 1.0f);    // Normalized magnitude should be <= 1
        }
    }
    
    [Fact]
    public void RealTimeAudioProcessor_ReturnsValidLevels()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        
#if MACCATALYST || MACOS
        services.AddSingleton<IAudioProcessor, RealTimeAudioProcessor>();
        services.AddSingleton<IAudioInputManager, MacAudioInputManager>();
#else
        services.AddSingleton<IAudioProcessor, RealTimeAudioProcessor>();
        services.AddSingleton<IAudioInputManager, MockAudioInputManager>();
#endif
        
        var serviceProvider = services.BuildServiceProvider();
        var audioProcessor = serviceProvider.GetRequiredService<IAudioProcessor>();
        
        // Act
        var levels = audioProcessor.GetLevel();
        
        // Assert
        // Levels should be in dB range (typically negative values)
        Assert.True(levels.Peak <= 0);     // Peak should be <= 0 dB
        Assert.True(levels.Peak >= -120);  // But not unreasonably low
        Assert.True(levels.Rms <= 0);      // RMS should be <= 0 dB  
        Assert.True(levels.Rms >= -120);   // But not unreasonably low
        Assert.True(levels.Rms <= levels.Peak); // RMS should be <= Peak
    }

#if !MACCATALYST && !MACOS
    // Mock implementation for testing on other platforms
    public class MockAudioInputManager : IAudioInputManager
    {
        public async Task<IEnumerable<AudioDevice>> GetDevicesAsync()
        {
            await Task.Delay(1); // Simulate async operation
            return new[]
            {
                new AudioDevice("mock-device-1", "Mock Device 1", 2, 44100),
                new AudioDevice("mock-device-2", "Mock Device 2", 1, 48000)
            };
        }
    }
#endif
}
