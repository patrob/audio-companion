using AudioCompanion.Shared.Audio;

namespace AudioCompanion.IntegrationTests.Audio;

public class TestRealTimeProcessor : IAudioProcessor
{
    private readonly ICoreAudioEngine? _audioEngine;
    private bool _isProcessing;

    public TestRealTimeProcessor(ICoreAudioEngine? audioEngine = null)
    {
        _audioEngine = audioEngine;
    }

    public async Task SelectDeviceAsync(string deviceId)
    {
        if (_audioEngine != null)
        {
            await _audioEngine.SelectDeviceAsync(deviceId);
        }
    }

    public void StartProcessing()
    {
        if (_isProcessing) return;
        if (_audioEngine == null) return;

        _audioEngine.InstallTap(2048, (buffer, frameCount) => { });
        _isProcessing = true;
        _ = _audioEngine.StartAsync();
    }

    public void StopProcessing()
    {
        if (!_isProcessing) return;
        if (_audioEngine == null) return;

        _audioEngine.RemoveTap();
        _audioEngine.Stop();
        _isProcessing = false;
    }

    public float[] GetSpectrum() => new float[1024];

    public (float Peak, float Rms) GetLevel() => (-60f, -60f);

    public void Dispose()
    {
        StopProcessing();
    }
}
