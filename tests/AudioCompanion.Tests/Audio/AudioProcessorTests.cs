using AudioCompanion.Shared.Audio;
using Xunit;

namespace AudioCompanion.Tests.Audio;

public class AudioProcessorTests
{
    [Fact]
    public void AudioProcessor_CanBeCreated()
    {
        // Arrange & Act
        using var processor = new AudioProcessor();
        
        // Assert
        Assert.NotNull(processor);
    }

    [Fact]
    public async Task AudioProcessor_CanSelectDevice()
    {
        // Arrange
        using var processor = new AudioProcessor();
        var deviceId = "test-device";
        
        // Act
        await processor.SelectDeviceAsync(deviceId);
        
        // Assert - No exception should be thrown
        Assert.True(true);
    }

    [Fact]
    public async Task AudioProcessor_ProvidesFreshSpectrumData()
    {
        // Arrange
        using var processor = new AudioProcessor();
        await processor.SelectDeviceAsync("test-device");
        processor.StartProcessing();
        
        // Wait for simulation to generate data
        Thread.Sleep(100);
        
        // Act
        var spectrum1 = processor.GetSpectrum();
        Thread.Sleep(50); // Wait for simulation to update
        var spectrum2 = processor.GetSpectrum();
        
        // Assert
        Assert.NotNull(spectrum1);
        Assert.NotNull(spectrum2);
        Assert.Equal(1024, spectrum1.Length); // FFT size / 2
        Assert.Equal(1024, spectrum2.Length);
        
        // Verify the data has some variation (not all zeros)
        Assert.Contains(spectrum1, x => x > 0);
        Assert.Contains(spectrum2, x => x > 0);
        
        processor.StopProcessing();
    }

    [Fact]
    public void AudioProcessor_ProvidesLevelData()
    {
        // Arrange
        using var processor = new AudioProcessor();
        
        // Act
        var levels = processor.GetLevel();
        
        // Assert
        Assert.True(levels.Peak >= -60f);
        Assert.True(levels.Rms >= -60f);
        Assert.True(levels.Peak <= 0f);
        Assert.True(levels.Rms <= 0f);
    }

    [Fact]
    public async Task AudioProcessor_CanStartAndStopProcessing()
    {
        // Arrange
        using var processor = new AudioProcessor();
        await processor.SelectDeviceAsync("test-device");
        
        // Act & Assert - No exceptions should be thrown
        processor.StartProcessing();
        processor.StopProcessing();
        
        Assert.True(true);
    }
}
