#if MACCATALYST || MACOS
using AVFoundation;
using Foundation;

namespace AudioCompanion.Shared.Audio;

/// <summary>
/// macOS implementation of audio device provider
/// </summary>
public class MacAudioDeviceProvider : IAudioDeviceProvider
{
    public async Task<bool> RequestMicrophonePermissionAsync()
    {
        try
        {
            var authStatus = AVCaptureDevice.GetAuthorizationStatus(AVMediaTypes.Audio);
            
            if (authStatus == AVAuthorizationStatus.Authorized)
                return true;
                
            if (authStatus == AVAuthorizationStatus.NotDetermined)
            {
                var tcs = new TaskCompletionSource<bool>();
                
                AVCaptureDevice.RequestAccessForMediaType(AVMediaTypes.Audio, granted =>
                {
                    tcs.SetResult(granted);
                });
                
                return await tcs.Task;
            }
            
            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<IEnumerable<AudioDeviceInfo>> GetAvailableDevicesAsync()
    {
        await Task.CompletedTask;
        
        try
        {
            var devices = new List<AudioDeviceInfo>();
            var discoverySession = AVCaptureDeviceDiscoverySession.Create(
                new[] { AVCaptureDeviceType.BuiltInMicrophone, AVCaptureDeviceType.ExternalUnknown },
                AVMediaTypes.Audio,
                AVCaptureDevicePosition.Unspecified);

            if (discoverySession?.Devices != null)
            {
                foreach (var device in discoverySession.Devices)
                {
                    var deviceName = device.LocalizedName ?? device.ModelID ?? "Unknown Device";
                    var channels = 1; // Default to mono
                    var sampleRate = 44100; // Default sample rate
                    
                    // Try to get more detailed format information
                    if (device.Formats?.Length > 0)
                    {
                        var format = device.Formats[0];
                        if (format?.FormatDescription != null)
                        {
                            var description = format.FormatDescription;
                            var streamDescription = description.AudioStreamBasicDescription;
                            
                            if (streamDescription != null)
                            {
                                channels = (int)(streamDescription.Value.ChannelsPerFrame);
                                sampleRate = (int)(streamDescription.Value.SampleRate);
                            }
                        }
                    }
                    
                    devices.Add(new AudioDeviceInfo(
                        Id: device.UniqueID ?? Guid.NewGuid().ToString(),
                        Name: deviceName,
                        Channels: channels,
                        SampleRate: sampleRate,
                        IsDefault: device.Position == AVCaptureDevicePosition.Unspecified
                    ));
                }
            }
            
            return devices;
        }
        catch
        {
            // Return at least a default device on error
            return new[]
            {
                new AudioDeviceInfo("default", "Default Microphone", 1, 44100, true)
            };
        }
    }
}
#endif
