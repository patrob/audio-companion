using AudioCompanion.Shared.Audio;
using AudioCompanion.Tests.Mocks;

namespace AudioCompanion.Tests.Audio;

public class AudioProcessorTests
{
    [Fact]
    public void MockAudioProcessor_ShouldImplementInterface()
    {
        // Arrange & Act
        var processor = new MockAudioProcessor();
        
        // Assert
        processor.ShouldBeAssignableTo<IAudioProcessor>();
    }
    
    [Fact]
    public void MockAudioProcessor_ShouldProvideDefaultSpectrum()
    {
        // Arrange
        var processor = new MockAudioProcessor();
        
        // Act
        var spectrum = processor.GetSpectrum();
        
        // Assert
        spectrum.ShouldNotBeNull();
        spectrum.Length.ShouldBe(1024);
        spectrum.ShouldAllBe(x => x >= -60f && x <= 0f);
    }
    
    [Fact]
    public void MockAudioProcessor_ShouldProvideDefaultLevels()
    {
        // Arrange
        var processor = new MockAudioProcessor();
        
        // Act
        var (peak, rms) = processor.GetLevel();
        
        // Assert
        peak.ShouldBeLessThanOrEqualTo(0f);
        rms.ShouldBeLessThanOrEqualTo(0f);
        peak.ShouldBeGreaterThanOrEqualTo(-60f);
        rms.ShouldBeGreaterThanOrEqualTo(-60f);
    }
    
    [Fact]
    public async Task MockAudioProcessor_ShouldHandleDeviceSelection()
    {
        // Arrange
        var processor = new MockAudioProcessor();
        
        // Act & Assert - Should not throw
        await processor.SelectDeviceAsync("test-device-id");
    }
    
    [Fact]
    public void MockAudioProcessor_ShouldHandleStartStopProcessing()
    {
        // Arrange
        var processor = new MockAudioProcessor();
        
        // Act & Assert - Should not throw
        processor.StartProcessing();
        processor.StopProcessing();
    }
}
