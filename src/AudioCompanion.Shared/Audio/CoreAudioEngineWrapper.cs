#if MACCATALYST || MACOS
using AVFoundation;
using Foundation;

namespace AudioCompanion.Shared.Audio;

/// <summary>
/// Wrapper for AVAudioEngine to enable mocking and testing
/// </summary>
public class CoreAudioEngineWrapper : ICoreAudioEngine
{
    private AVAudioEngine? _audioEngine;
    private AVAudioInputNode? _inputNode;
    private Action<float[], uint>? _audioCallback;
    private bool _disposed;

    public bool IsRunning => _audioEngine?.Running ?? false;

    public async Task<bool> StartAsync()
    {
        try
        {
            _audioEngine = new AVAudioEngine();
            _inputNode = _audioEngine.InputNode;
            
            if (_inputNode == null) return false;

            var success = _audioEngine.StartAndReturnError(out var error);
            return success && error == null;
        }
        catch
        {
            return false;
        }
    }

    public void Stop()
    {
        try
        {
            _audioEngine?.Stop();
        }
        catch
        {
            // Log error in production
        }
    }

    public void InstallTap(uint bufferSize, Action<float[], uint> audioCallback)
    {
        if (_inputNode == null) return;

        _audioCallback = audioCallback;
        var format = _inputNode.GetBusOutputFormat(0);
        
        _inputNode.InstallTapOnBus(0, bufferSize, format, (buffer, when) =>
        {
            ProcessAudioBuffer(buffer, audioCallback);
        });
    }

    public void RemoveTap()
    {
        _inputNode?.RemoveTapOnBus(0);
        _audioCallback = null;
    }

    public async Task<bool> SelectDeviceAsync(string deviceId)
    {
        // Device selection will be implemented when we have specific device requirements
        // For now, we'll use the default input device
        await Task.CompletedTask;
        return true;
    }

    private void ProcessAudioBuffer(AVAudioPcmBuffer buffer, Action<float[], uint> callback)
    {
        if (buffer?.FrameLength == 0 || buffer?.AudioBufferList == null) return;

        unsafe
        {
            var audioBufferList = buffer.AudioBufferList;
            if (audioBufferList->Count == 0) return;

            var audioBuffer = audioBufferList->Buffers[0];
            var frameCount = (int)buffer.FrameLength;
            var channelCount = (int)audioBuffer.NumberChannels;
            
            if (audioBuffer.Data == IntPtr.Zero || frameCount == 0) return;

            var samples = (float*)audioBuffer.Data.ToPointer();
            var processedSamples = new float[frameCount];
            
            if (channelCount == 1)
            {
                // Mono audio - direct copy
                for (int i = 0; i < frameCount; i++)
                {
                    processedSamples[i] = samples[i];
                }
            }
            else
            {
                // Multi-channel audio - convert to mono by averaging
                for (int i = 0; i < frameCount; i++)
                {
                    float sum = 0;
                    for (int ch = 0; ch < channelCount; ch++)
                    {
                        sum += samples[i * channelCount + ch];
                    }
                    processedSamples[i] = sum / channelCount;
                }
            }
            
            callback(processedSamples, (uint)frameCount);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        RemoveTap();
        Stop();
        _audioEngine?.Dispose();
        _disposed = true;
    }
}
#endif
