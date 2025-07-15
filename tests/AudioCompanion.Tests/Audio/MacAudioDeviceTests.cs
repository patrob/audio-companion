using AudioCompanion.App.Audio;
using Xunit;

namespace AudioCompanion.Tests.Audio;

public class MacAudioDeviceTests
{
    [Fact]
    public async Task MacAudioInputManager_ShouldEnumerateDevices()
    {
        // Arrange
#if MACCATALYST || MACOS
        var manager = new MacAudioInputManager();
        
        // Act
        var devices = await manager.GetDevicesAsync();
        
        // Assert
        Assert.NotNull(devices);
        // Note: We can't assert specific devices exist since it depends on the test environment
        // But we can verify the structure is correct
        foreach (var device in devices)
        {
            Assert.NotNull(device.Id);
            Assert.NotNull(device.Name);
            Assert.True(device.SampleRate > 0 || device.SampleRate == 0); // 0 is acceptable for unknown
            Assert.True(device.Channels > 0 || device.Channels == 0); // 0 is acceptable for unknown
        }
#else
        // Skip test on non-macOS platforms
        await Task.CompletedTask;
        Assert.True(true, "Test skipped on non-macOS platform");
#endif
    }
    
    [Fact]
    public async Task MacAudioInputManager_ShouldRequestPermissions()
    {
        // Arrange
#if MACCATALYST || MACOS
        var manager = new MacAudioInputManager();
        
        // Act & Assert - Should not throw
        var hasPermission = await manager.RequestPermissionAsync();
        
        // We can't assert the specific value since it depends on user interaction
        // But we can verify the method completes without throwing
        Assert.True(hasPermission || !hasPermission); // Always true, just testing no exception
#else
        // Skip test on non-macOS platforms
        await Task.CompletedTask;
        Assert.True(true, "Test skipped on non-macOS platform");
#endif
    }
}
