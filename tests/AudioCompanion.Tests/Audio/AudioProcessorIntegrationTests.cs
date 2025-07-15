using AudioCompanion.Shared.Audio;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Shouldly;

namespace AudioCompanion.Tests.Audio;

public class AudioProcessorIntegrationTests
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
        audioProcessor.ShouldNotBeNull();
        audioProcessor.ShouldBeOfType<RealTimeAudioProcessor>();
    }
    
    [Fact]
    public void RealTimeAudioProcessor_ShouldImplementCorrectInterface()
    {
        // Arrange & Act
        var processor = new RealTimeAudioProcessor();
        
        // Assert
        processor.ShouldBeAssignableTo<IAudioProcessor>();
        processor.ShouldBeAssignableTo<IDisposable>();
        
        // Cleanup
        processor.Dispose();
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
        spectrum.ShouldNotBeNull();
        spectrum.Length.ShouldBeGreaterThan(0, "Spectrum should have data points");
        level.Peak.ShouldBeLessThanOrEqualTo(0, "Peak should be in dB (negative or zero)");
        level.Rms.ShouldBeLessThanOrEqualTo(0, "RMS should be in dB (negative or zero)");
        
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
