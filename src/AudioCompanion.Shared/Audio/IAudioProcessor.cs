using System.Threading.Tasks;

namespace AudioCompanion.Shared.Audio
{
    // Represents an audio input device (see PRD)
    public record AudioInputDevice(string Id, string Name, int Channels, int SampleRate);

    // Interface for audio processing (see PRD)
    public interface IAudioProcessor
    {
        Task SelectDeviceAsync(string deviceId);
        void StartProcessing();
        void StopProcessing();
        float[] GetSpectrum();
        (float Peak, float Rms) GetLevel();
    }
}