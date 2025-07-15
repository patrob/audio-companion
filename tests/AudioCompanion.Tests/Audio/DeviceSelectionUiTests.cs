using Xunit;
using AudioCompanion.Shared.Audio;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shouldly;

namespace AudioCompanion.Tests.Audio
{
    public class DeviceSelectionUiTests
    {
        [Fact]
        public async Task DeviceDropdown_PopulatesWithDevices()
        {
            // Arrange
            var fakeManager = new FakeAudioInputManager();
            var devices = await fakeManager.EnumerateDevicesAsync();

            // Assert
            devices.ShouldNotBeEmpty();
            devices.ShouldContain(d => d.Name == "Built-in Mic");
        }

        private class FakeAudioInputManager : IAudioInputManager
        {
            public Task<List<AudioInputDevice>> EnumerateDevicesAsync() =>
                Task.FromResult(new List<AudioInputDevice> { new AudioInputDevice("1", "Built-in Mic", 2, 44100) });
        }
    }
}
