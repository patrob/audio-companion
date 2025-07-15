namespace AudioCompanion.Shared.Audio;

/// <summary>
/// Interface for audio device discovery and permission management
/// </summary>
public interface IAudioDeviceProvider
{
    /// <summary>
    /// Requests microphone permission from the system
    /// </summary>
    /// <returns>True if permission was granted</returns>
    Task<bool> RequestMicrophonePermissionAsync();
    
    /// <summary>
    /// Gets all available audio input devices
    /// </summary>
    /// <returns>Collection of available audio devices</returns>
    Task<IEnumerable<AudioDeviceInfo>> GetAvailableDevicesAsync();
}

/// <summary>
/// Information about an audio device
/// </summary>
public record AudioDeviceInfo(
    string Id,
    string Name,
    int Channels,
    int SampleRate,
    bool IsDefault = false
);
