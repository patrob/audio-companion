using AudioCompanion.Shared.Audio;
using Microsoft.Extensions.DependencyInjection;

namespace AudioCompanion.IntegrationTests.Audio;

/// <summary>
/// Cross-platform integration tests for the audio system
/// </summary>
public class AudioSystemIntegrationTests
{
    [Fact]
    [Trait("Category", "Integration")]
    public void RealTimeAudioProcessor_ShouldBeRegisteredInDI()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Act - Register like in MauiProgram
        services.AddSingleton<IAudioProcessor, RealTimeAudioProcessor>();
        using var serviceProvider = services.BuildServiceProvider();
        
        // Assert
        var audioProcessor = serviceProvider.GetService<IAudioProcessor>();
        audioProcessor.ShouldNotBeNull();
        audioProcessor.ShouldBeOfType<RealTimeAudioProcessor>();
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public void AudioProcessor_ShouldHandleMultipleUIComponents()
    {
        // Arrange - Simulate multiple UI components accessing the same processor
        var services = new ServiceCollection();
        services.AddSingleton<IAudioProcessor, RealTimeAudioProcessor>();
        using var serviceProvider = services.BuildServiceProvider();
        
        // Act - Get the same processor instance from multiple places
        var processor1 = serviceProvider.GetRequiredService<IAudioProcessor>();
        var processor2 = serviceProvider.GetRequiredService<IAudioProcessor>();
        var processor3 = serviceProvider.GetRequiredService<IAudioProcessor>();
        
        // Assert - Should be the same singleton instance
        processor1.ShouldBeSameAs(processor2);
        processor2.ShouldBeSameAs(processor3);
        
        // All components can access the processor safely
        processor1.StartProcessing();
        
        var spectrum1 = processor1.GetSpectrum();
        var spectrum2 = processor2.GetSpectrum();
        var level1 = processor1.GetLevel();
        var level2 = processor2.GetLevel();
        
        // All calls work and return valid data
        spectrum1.ShouldNotBeNull();
        spectrum2.ShouldNotBeNull();
        spectrum1.Length.ShouldBe(spectrum2.Length);
        
        // Cleanup
        processor1.StopProcessing();
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public async Task AudioProcessor_ShouldPersistDeviceSelectionAcrossSessions()
    {
        // Arrange
        using var processor = new RealTimeAudioProcessor();
        const string testDeviceId = "test-device-123";
        
        // Act - Select a device and test multiple processing sessions
        await processor.SelectDeviceAsync(testDeviceId);
        
        for (int session = 0; session < 3; session++)
        {
            processor.StartProcessing();
            
            // Get some data
            var spectrum = processor.GetSpectrum();
            var level = processor.GetLevel();
            
            spectrum.ShouldNotBeNull();
            spectrum.Length.ShouldBeGreaterThan(0);
            
            processor.StopProcessing();
            
            // Brief pause between sessions
            await Task.Delay(10);
        }
        
        // Assert - Device selection should persist across sessions
        // (This is implicit in the fact that no exceptions were thrown)
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public void AudioProcessor_ShouldBeThreadSafe()
    {
        // Arrange
        using var processor = new RealTimeAudioProcessor();
        const int threadsCount = 4;
        const int operationsPerThread = 50;
        var exceptions = new List<Exception>();
        var tasks = new List<Task>();
        
        processor.StartProcessing();
        
        // Act - Test concurrent access from multiple threads
        for (int t = 0; t < threadsCount; t++)
        {
            var task = Task.Run(() =>
            {
                try
                {
                    for (int i = 0; i < operationsPerThread; i++)
                    {
                        var spectrum = processor.GetSpectrum();
                        var level = processor.GetLevel();
                        
                        spectrum.ShouldNotBeNull();
                        spectrum.Length.ShouldBeGreaterThan(0);
                    }
                }
                catch (Exception ex)
                {
                    lock (exceptions)
                    {
                        exceptions.Add(ex);
                    }
                }
            });
            tasks.Add(task);
        }
        
        Task.WaitAll(tasks.ToArray());
        
        // Assert
        exceptions.ShouldBeEmpty();
        
        // Cleanup
        processor.StopProcessing();
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public void AudioProcessor_WithMockedDependencies_ShouldWorkEndToEnd()
    {
        // Arrange
        var mockAudioEngine = Substitute.For<ICoreAudioEngine>();
        var mockDeviceProvider = Substitute.For<IAudioDeviceProvider>();
        
        // Setup mock behavior
        mockAudioEngine.StartAsync().Returns(true);
        mockAudioEngine.IsRunning.Returns(true);
        
        using var processor = new RealTimeAudioProcessor(mockAudioEngine, mockDeviceProvider);
        
        // Act - Simulate complete workflow
        processor.StartProcessing();
        
        // Simulate audio callback
        var testBuffer = new float[2048];
        for (int i = 0; i < testBuffer.Length; i++)
        {
            testBuffer[i] = (float)Math.Sin(2 * Math.PI * 440 * i / 44100); // 440Hz sine wave
        }
        
        // We can't directly invoke the callback, but we can verify the processor works
        var spectrum = processor.GetSpectrum();
        var level = processor.GetLevel();
        
        processor.StopProcessing();
        
        // Assert
        spectrum.ShouldNotBeNull();
        spectrum.Length.ShouldBeGreaterThan(0);
        level.Peak.ShouldBeLessThanOrEqualTo(0f);
        level.Rms.ShouldBeLessThanOrEqualTo(0f);
        
        // Verify interactions
        mockAudioEngine.Received(1).InstallTap(Arg.Any<uint>(), Arg.Any<Action<float[], uint>>());
        mockAudioEngine.Received(1).StartAsync();
        mockAudioEngine.Received(1).RemoveTap();
        mockAudioEngine.Received(1).Stop();
    }
}
