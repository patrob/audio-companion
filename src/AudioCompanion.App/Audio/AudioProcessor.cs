using AudioCompanion.Shared.Audio;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using System.Numerics;

namespace AudioCompanion.App.Audio;

public class AudioProcessor : IAudioProcessor
{
    private string? _currentDeviceId;
    private bool _isProcessing;
    private readonly int _fftSize = 2048;
    private readonly float[] _audioBuffer;
    private readonly Complex[] _fftBuffer;
    private readonly float[] _spectrumData;
    private readonly object _lockObject = new();
    private float _peakLevel = -60f;
    private float _rmsLevel = -60f;
    
    // Audio simulation for now - in real implementation this would be replaced with actual audio capture
    private readonly Random _random = new();
    private readonly Timer? _simulationTimer;

    public AudioProcessor()
    {
        _audioBuffer = new float[_fftSize];
        _fftBuffer = new Complex[_fftSize];
        _spectrumData = new float[_fftSize / 2]; // Only positive frequencies
        
        // Simulate audio data for testing
        _simulationTimer = new Timer(SimulateAudioData, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(16)); // ~60 FPS
    }

    public Task SelectDeviceAsync(string deviceId)
    {
        _currentDeviceId = deviceId;
        return Task.CompletedTask;
    }

    public void StartProcessing()
    {
        if (string.IsNullOrEmpty(_currentDeviceId))
            throw new InvalidOperationException("No device selected");

        _isProcessing = true;
    }

    public void StopProcessing()
    {
        _isProcessing = false;
    }

    public float[] GetSpectrum()
    {
        lock (_lockObject)
        {
            return _spectrumData.ToArray();
        }
    }

    public (float Peak, float Rms) GetLevel()
    {
        lock (_lockObject)
        {
            return (_peakLevel, _rmsLevel);
        }
    }

    private void SimulateAudioData(object? state)
    {
        if (!_isProcessing) return;

        lock (_lockObject)
        {
            // Generate simulated audio data with some spectral content
            for (int i = 0; i < _fftSize; i++)
            {
                // Create a mix of frequencies to simulate audio
                float sample = 0;
                
                // Add some fundamental frequency around 200Hz
                sample += 0.3f * MathF.Sin(2 * MathF.PI * 200 * i / 48000f);
                
                // Add harmonics
                sample += 0.2f * MathF.Sin(2 * MathF.PI * 400 * i / 48000f);
                sample += 0.15f * MathF.Sin(2 * MathF.PI * 800 * i / 48000f);
                
                // Add some high frequency content
                sample += 0.1f * MathF.Sin(2 * MathF.PI * 2000 * i / 48000f);
                sample += 0.05f * MathF.Sin(2 * MathF.PI * 4000 * i / 48000f);
                
                // Add noise
                sample += 0.02f * (float)(_random.NextDouble() * 2 - 1);
                
                // Apply random amplitude variation
                sample *= 0.5f + 0.5f * (float)_random.NextDouble();
                
                _audioBuffer[i] = sample;
                _fftBuffer[i] = new Complex(sample, 0);
            }

            // Calculate levels
            CalculateLevels();
            
            // Perform FFT
            Fourier.Forward(_fftBuffer, FourierOptions.Default);
            
            // Convert to magnitude spectrum
            for (int i = 0; i < _spectrumData.Length; i++)
            {
                _spectrumData[i] = (float)_fftBuffer[i].Magnitude / _fftSize;
            }
        }
    }

    private void CalculateLevels()
    {
        float peak = 0f;
        float sumSquares = 0f;

        for (int i = 0; i < _audioBuffer.Length; i++)
        {
            float abs = MathF.Abs(_audioBuffer[i]);
            if (abs > peak)
                peak = abs;
            
            sumSquares += _audioBuffer[i] * _audioBuffer[i];
        }

        _peakLevel = peak > 0 ? 20 * MathF.Log10(peak) : -60f;
        
        float rms = MathF.Sqrt(sumSquares / _audioBuffer.Length);
        _rmsLevel = rms > 0 ? 20 * MathF.Log10(rms) : -60f;
        
        // Clamp values
        _peakLevel = MathF.Max(_peakLevel, -60f);
        _rmsLevel = MathF.Max(_rmsLevel, -60f);
    }

    public void Dispose()
    {
        _simulationTimer?.Dispose();
        StopProcessing();
    }
}
