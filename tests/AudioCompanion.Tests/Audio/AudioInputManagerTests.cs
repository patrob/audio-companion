using Xunit;
using AudioCompanion.Shared.Audio;
using System.Threading.Tasks;
using System.Collections.Generic;
using Shouldly;

namespace AudioCompanion.Tests.Audio
{
    public class AudioInputManagerTests
    {
        [Fact]
        public async Task EnumerateAudioDevices_ReturnsAtLeastOneDevice()
        {
            // Arrange
            var manager = new DummyAudioInputManager();

            // Act
            var devices = await manager.EnumerateDevicesAsync();

            // Assert
            devices.ShouldNotBeNull();
            devices.ShouldNotBeEmpty();
        }
    }

    // Dummy implementation for TDD (replace with real CoreAudio integration)
    public class DummyAudioInputManager
    {
        public Task<List<AudioInputDevice>> EnumerateDevicesAsync()
        {
            return Task.FromResult(new List<AudioInputDevice>
            {
                new AudioInputDevice("1", "Built-in Mic", 2, 44100)
            });
        }
    }
}
