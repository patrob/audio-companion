using AudioCompanion.Shared.Audio;

namespace AudioCompanion.Tests.Mocks;

/// <summary>
/// Mock audio processor for testing purposes
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
