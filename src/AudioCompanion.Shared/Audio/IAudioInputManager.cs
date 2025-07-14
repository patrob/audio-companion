using System.Collections.Generic;
using System.Threading.Tasks;

namespace AudioCompanion.Shared.Audio
{
    // Interface for device enumeration (for TDD and platform-specific implementations)
    public interface IAudioInputManager
    {
        Task<List<AudioInputDevice>> EnumerateDevicesAsync();
    }
}
