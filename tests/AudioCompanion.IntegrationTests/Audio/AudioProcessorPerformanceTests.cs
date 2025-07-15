using AudioCompanion.Shared.Audio;
using System.Diagnostics;
using Xunit.Abstractions;

namespace AudioCompanion.IntegrationTests.Audio;

/// <summary>
/// Performance tests for audio processing components
/// </summary>
public class AudioProcessorPerformanceTests
{
    private readonly ITestOutputHelper _output;
    
    public AudioProcessorPerformanceTests(ITestOutputHelper output)
    {
        _output = output;
    }
    
    [Fact]
    [Trait("Category", "Performance")]
    public void RealTimeAudioProcessor_GetSpectrum_ShouldBeFast()
    {
        // Arrange
        using var processor = new RealTimeAudioProcessor();
        const int iterations = 1000;
        const double maxAcceptableMs = 10.0; // 10ms max for real-time processing
        
        // Warm up
        for (int i = 0; i < 10; i++)
        {
            processor.GetSpectrum();
        }
        
        // Act
        var stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            var spectrum = processor.GetSpectrum();
            spectrum.ShouldNotBeNull();
        }
        stopwatch.Stop();
        
        // Assert
        var averageMs = (double)stopwatch.ElapsedMilliseconds / iterations;
        _output.WriteLine($"Average GetSpectrum() time: {averageMs:F3}ms per call");
        
        averageMs.ShouldBeLessThan(maxAcceptableMs,
            $"GetSpectrum() too slow: {averageMs:F3}ms average (max: {maxAcceptableMs}ms)");
    }
    
    [Fact]
    [Trait("Category", "Performance")]
    public void RealTimeAudioProcessor_GetLevel_ShouldBeFast()
    {
        // Arrange
        using var processor = new RealTimeAudioProcessor();
        const int iterations = 1000;
        const double maxAcceptableMs = 1.0; // 1ms max for level calculation
        
        // Warm up
        for (int i = 0; i < 10; i++)
        {
            processor.GetLevel();
        }
        
        // Act
        var stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            var level = processor.GetLevel();
            level.Peak.ShouldBeLessThanOrEqualTo(0); // dB values should be negative or zero
            level.Rms.ShouldBeLessThanOrEqualTo(0);
        }
        stopwatch.Stop();
        
        // Assert
        var averageMs = (double)stopwatch.ElapsedMilliseconds / iterations;
        _output.WriteLine($"Average GetLevel() time: {averageMs:F3}ms per call");
        
        averageMs.ShouldBeLessThan(maxAcceptableMs,
            $"GetLevel() too slow: {averageMs:F3}ms average (max: {maxAcceptableMs}ms)");
    }
    
    [Fact]
    [Trait("Category", "Performance")]
    public void RealTimeAudioProcessor_StartStop_ShouldBeReasonablyFast()
    {
        // Arrange
        using var processor = new RealTimeAudioProcessor();
        const double maxAcceptableMs = 100.0; // 100ms max for start/stop operations
        
        // Act & Assert - Test multiple start/stop cycles
        for (int i = 0; i < 5; i++)
        {
            var stopwatch = Stopwatch.StartNew();
            
            processor.StartProcessing();
            var startTime = stopwatch.ElapsedMilliseconds;
            
            processor.StopProcessing();
            stopwatch.Stop();
            
            _output.WriteLine($"Cycle {i + 1}: Start={startTime}ms, Total={stopwatch.ElapsedMilliseconds}ms");
            
            ((double)stopwatch.ElapsedMilliseconds).ShouldBeLessThan(maxAcceptableMs,
                $"Start/Stop cycle too slow: {stopwatch.ElapsedMilliseconds}ms (max: {maxAcceptableMs}ms)");
        }
    }
    
    [Fact]
    [Trait("Category", "Performance")]
    public void MockAudioProcessor_ShouldBeEvenFaster()
    {
        // Arrange
        var processor = new MockAudioProcessor();
        const int iterations = 10000;
        const double maxAcceptableMs = 1.0; // Mock should be very fast
        
        // Act
        var stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            var spectrum = processor.GetSpectrum();
            var level = processor.GetLevel();
            
            spectrum.ShouldNotBeNull();
        }
        stopwatch.Stop();
        
        // Assert
        var averageMs = (double)stopwatch.ElapsedMilliseconds / iterations;
        _output.WriteLine($"Average Mock operations time: {averageMs:F6}ms per call");
        
        averageMs.ShouldBeLessThan(maxAcceptableMs,
            $"Mock processor too slow: {averageMs:F6}ms average (max: {maxAcceptableMs}ms)");
    }
    
    [Fact]
    [Trait("Category", "Performance")]
    public void RealTimeAudioProcessor_WithMocks_ShouldBeFast()
    {
        // Arrange
        var mockAudioEngine = Substitute.For<ICoreAudioEngine>();
        var mockDeviceProvider = Substitute.For<IAudioDeviceProvider>();
        
        using var processor = new RealTimeAudioProcessor(mockAudioEngine, mockDeviceProvider);
        const int iterations = 1000;
        const double maxAcceptableMs = 5.0;
        
        // Act
        var stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            var spectrum = processor.GetSpectrum();
            var level = processor.GetLevel();
            
            spectrum.ShouldNotBeNull();
        }
        stopwatch.Stop();
        
        // Assert
        var averageMs = (double)stopwatch.ElapsedMilliseconds / iterations;
        _output.WriteLine($"Average mocked processor time: {averageMs:F3}ms per call");
        
        averageMs.ShouldBeLessThan(maxAcceptableMs,
            $"Mocked processor too slow: {averageMs:F3}ms average (max: {maxAcceptableMs}ms)");
    }
}

/// <summary>
/// Simple mock audio processor for performance testing
/// </summary>
public class MockAudioProcessor : IAudioProcessor
{
    private bool _isProcessing;
    
    public void StartProcessing()
    {
        _isProcessing = true;
    }

    public void StopProcessing()
    {
        _isProcessing = false;
    }

    public float[] GetSpectrum()
    {
        // Return mock spectrum data
        return new float[1024];
    }

    public (float Peak, float Rms) GetLevel()
    {
        // Return mock level data
        return (-20f, -25f);
    }

    public Task SelectDeviceAsync(string deviceId)
    {
        return Task.CompletedTask;
    }

    public bool IsProcessing => _isProcessing;
}
