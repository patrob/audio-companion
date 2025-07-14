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
        public Task<List<AudioInputDevice>> EnumerateDevicesAsync()
        {
            var devices = new List<AudioInputDevice>();
            var discoverySession = AVCaptureDeviceDiscoverySession.Create(
                new AVCaptureDeviceType[] { AVCaptureDeviceType.BuiltInMicrophone },
                AVMediaTypes.Audio,
                AVCaptureDevicePosition.Unspecified);
            foreach (var device in discoverySession.Devices)
            {
                devices.Add(new AudioInputDevice(
                    device.UniqueID,
                    device.LocalizedName,
                    1, // Channel count not directly available
                    44100 // Sample rate not directly available
                ));
            }
            return Task.FromResult(devices);
        }
    }
}
#endif
