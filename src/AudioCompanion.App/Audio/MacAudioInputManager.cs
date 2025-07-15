#if MACCATALYST || MACOS
using System.Collections.Generic;
using System.Threading.Tasks;
using AudioCompanion.Shared.Audio;
using AVFoundation;
using Foundation;

namespace AudioCompanion.App.Audio
{
    public class MacAudioInputManager : IAudioInputManager
    {
        public async Task<List<AudioInputDevice>> EnumerateDevicesAsync()
        {
            var devices = new List<AudioInputDevice>();
            
            try
            {
                // Request microphone permission first
                var mediaType = AVMediaTypes.Audio.GetConstant();
                if (mediaType != null)
                {
                    var authStatus = AVCaptureDevice.GetAuthorizationStatus(mediaType);
                    
                    if (authStatus == AVAuthorizationStatus.NotDetermined)
                    {
                        await AVCaptureDevice.RequestAccessForMediaTypeAsync(mediaType);
                    }
                }
                
                // Get all audio input devices
                var discoverySession = AVCaptureDeviceDiscoverySession.Create(
                    new AVCaptureDeviceType[] 
                    { 
                        AVCaptureDeviceType.BuiltInMicrophone,
                        AVCaptureDeviceType.ExternalUnknown
                    },
                    AVMediaTypes.Audio,
                    AVCaptureDevicePosition.Unspecified);
                
                if (discoverySession?.Devices != null)
                {
                    foreach (var device in discoverySession.Devices)
                    {
                        // Get device capabilities
                        var format = device.ActiveFormat;
                        var channels = 1; // Default
                        var sampleRate = 48000; // Default
                        
                        if (format?.FormatDescription != null)
                        {
                            // Try to get actual format info
                            var description = format.FormatDescription;
                            var streamDescription = description.AudioStreamBasicDescription;
                            
                            if (streamDescription != null)
                            {
                                var desc = streamDescription.Value;
                                channels = (int)desc.ChannelsPerFrame;
                                sampleRate = (int)desc.SampleRate;
                            }
                        }
                        
                        var deviceName = device.LocalizedName ?? device.ModelID ?? "Unknown Device";
                        
                        devices.Add(new AudioInputDevice(
                            device.UniqueID,
                            deviceName,
                            channels,
                            sampleRate
                        ));
                    }
                }
                
                // Add a default device if none found
                if (devices.Count == 0)
                {
                    devices.Add(new AudioInputDevice(
                        "default",
                        "Default Microphone",
                        1,
                        48000
                    ));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error enumerating audio devices: {ex.Message}");
                
                // Fallback device
                devices.Add(new AudioInputDevice(
                    "default",
                    "Default Microphone",
                    1,
                    48000
                ));
            }
            
            return devices;
        }
    }
}
#endif
