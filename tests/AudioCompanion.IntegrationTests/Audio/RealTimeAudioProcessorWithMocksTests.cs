using AudioCompanion.Shared.Audio;
using Xunit;
using Shouldly;
using NSubstitute;

namespace AudioCompanion.IntegrationTests.Audio;

[Trait("Category", "Integration")]
public class RealTimeAudioProcessorWithMocksTests
{
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
        
        // Debug: Check if the engine was set properly using reflection
        var audioEngineField = typeof(RealTimeAudioProcessor).GetField("_audioEngine", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var internalEngine = audioEngineField.GetValue(processor);
        Assert.NotNull(internalEngine);
        Assert.Equal(mockAudioEngine, internalEngine);
        
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
    
    [Fact]
    public void RealTimeAudioProcessor_DebugInternalEngineState()
    {
        // Arrange
        var mockAudioEngine = Substitute.For<ICoreAudioEngine>();
        var mockDeviceProvider = Substitute.For<IAudioDeviceProvider>();
        
        var processor = new RealTimeAudioProcessor(mockAudioEngine, mockDeviceProvider);
        
        // Act & Assert - Try to understand what's happening with internal state
        Assert.NotNull(mockAudioEngine);
        // processor.AudioEngineForTesting is not available due to compilation issues
        // Let's test by trying to use reflection
        var audioEngineField = typeof(RealTimeAudioProcessor).GetField("_audioEngine", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        Assert.NotNull(audioEngineField);
        var internalEngine = audioEngineField.GetValue(processor);
        Assert.NotNull(internalEngine);
        Assert.Equal(mockAudioEngine, internalEngine);
        
        // Cleanup
        processor.Dispose();
    }
}
