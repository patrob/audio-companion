namespace AudioCompanion.Shared.Audio;

/// <summary>
/// Interface for Core Audio engine functionality to enable mocking and testing
/// </summary>
public interface ICoreAudioEngine : IDisposable
{
    /// <summary>
    /// Starts the audio engine
    /// </summary>
    /// <returns>True if started successfully</returns>
    Task<bool> StartAsync();
    
    /// <summary>
    /// Stops the audio engine
    /// </summary>
    void Stop();
    
    /// <summary>
    /// Installs a tap on the input node for audio processing
    /// </summary>
    /// <param name="bufferSize">Size of the audio buffer</param>
    /// <param name="audioCallback">Callback for processing audio data</param>
    void InstallTap(uint bufferSize, Action<float[], uint> audioCallback);
    
    /// <summary>
    /// Removes the tap from the input node
    /// </summary>
    void RemoveTap();
    
    /// <summary>
    /// Selects an audio input device
    /// </summary>
    /// <param name="deviceId">Device identifier</param>
    /// <returns>True if device was selected successfully</returns>
    Task<bool> SelectDeviceAsync(string deviceId);
    
    /// <summary>
    /// Gets whether the engine is currently running
    /// </summary>
    bool IsRunning { get; }
}
