using AudioCompanion.Shared.Audio;

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
    
    [Fact]
    public void RealTimeAudioProcessor_WithMockedDependencies_ShouldWork()
    {
        // Arrange
        var mockAudioEngine = Substitute.For<ICoreAudioEngine>();
        var mockDeviceProvider = Substitute.For<IAudioDeviceProvider>();
        
        var processor = new RealTimeAudioProcessor(mockAudioEngine, mockDeviceProvider);
        
        // Act & Assert
        processor.ShouldBeAssignableTo<IAudioProcessor>();
        processor.ShouldBeAssignableTo<IDisposable>();
        
        // Should provide default values when not processing
        var spectrum = processor.GetSpectrum();
        var level = processor.GetLevel();
        
        spectrum.ShouldNotBeNull();
        spectrum.Length.ShouldBeGreaterThan(0);
        level.Peak.ShouldBeLessThanOrEqualTo(0f);
        level.Rms.ShouldBeLessThanOrEqualTo(0f);
        
        // Cleanup
        processor.Dispose();
    }
    
    [Fact]
    public async Task RealTimeAudioProcessor_ShouldDelegateDeviceSelection()
    {
        // Arrange
        var mockAudioEngine = Substitute.For<ICoreAudioEngine>();
        var mockDeviceProvider = Substitute.For<IAudioDeviceProvider>();
        
        mockAudioEngine.SelectDeviceAsync(Arg.Any<string>()).Returns(true);
        
        var processor = new RealTimeAudioProcessor(mockAudioEngine, mockDeviceProvider);
        
        // Act
        await processor.SelectDeviceAsync("test-device");
        
        // Assert
        await mockAudioEngine.Received(1).SelectDeviceAsync("test-device");
        
        // Cleanup
        processor.Dispose();
    }
    
    [Fact]
    public void RealTimeAudioProcessor_StartProcessing_ShouldConfigureAudioEngine()
    {
        // Arrange
        var mockAudioEngine = Substitute.For<ICoreAudioEngine>();
        var mockDeviceProvider = Substitute.For<IAudioDeviceProvider>();
        
        mockAudioEngine.StartAsync().Returns(true);
        
        var processor = new RealTimeAudioProcessor(mockAudioEngine, mockDeviceProvider);
        
        // Act
        processor.StartProcessing();
        
        // Assert
        mockAudioEngine.Received(1).InstallTap(Arg.Any<uint>(), Arg.Any<Action<float[], uint>>());
        mockAudioEngine.Received(1).StartAsync();
        
        // Cleanup
        processor.Dispose();
    }
    
    [Fact]
    public void RealTimeAudioProcessor_StopProcessing_ShouldCleanupAudioEngine()
    {
        // Arrange
        var mockAudioEngine = Substitute.For<ICoreAudioEngine>();
        var mockDeviceProvider = Substitute.For<IAudioDeviceProvider>();
        
        var processor = new RealTimeAudioProcessor(mockAudioEngine, mockDeviceProvider);
        
        // Start first
        processor.StartProcessing();
        
        // Act
        processor.StopProcessing();
        
        // Assert
        mockAudioEngine.Received(1).RemoveTap();
        mockAudioEngine.Received(1).Stop();
        
        // Cleanup
        processor.Dispose();
    }
}
