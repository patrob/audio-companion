using AudioCompanion.Shared.Audio;

namespace AudioCompanion.IntegrationTests.MacOS;

/// <summary>
/// Integration tests specific to macOS Core Audio functionality
/// </summary>
public class MacAudioIntegrationTests
{
    [Fact]
    [Trait("Category", "MacOS")]
    [Trait("Category", "Integration")]
    public async Task MacAudioDeviceProvider_ShouldEnumerateDevices()
    {
        // Skip if not running on macOS
        if (!OperatingSystem.IsMacOS() && !OperatingSystem.IsMacCatalyst())
        {
            return;
        }

#if MACCATALYST || MACOS
        // Arrange
        var provider = new MacAudioDeviceProvider();
        
        // Act
        var devices = await provider.GetAvailableDevicesAsync();
        
        // Assert
        devices.ShouldNotBeNull();
        devices.ShouldNotBeEmpty();
        
        foreach (var device in devices)
        {
            device.Id.ShouldNotBeNullOrEmpty();
            device.Name.ShouldNotBeNullOrEmpty();
            device.Channels.ShouldBeGreaterThanOrEqualTo(0);
            device.SampleRate.ShouldBeGreaterThanOrEqualTo(0);
        }
        
        // Should have at least one default device
        devices.ShouldContain(d => d.IsDefault);
#endif
    }
    
    [Fact]
    [Trait("Category", "MacOS")]
    [Trait("Category", "Integration")]
    public async Task MacAudioDeviceProvider_ShouldRequestPermissions()
    {
        // Skip if not running on macOS
        if (!OperatingSystem.IsMacOS() && !OperatingSystem.IsMacCatalyst())
        {
            return;
        }

#if MACCATALYST || MACOS
        // Arrange
        var provider = new MacAudioDeviceProvider();
        
        // Act
        var hasPermission = await provider.RequestMicrophonePermissionAsync();
        
        // Assert
        // We can't assert the specific value since it depends on user interaction
        // But we can verify the method completes without throwing
        hasPermission.ShouldBeOfType<bool>();
#endif
    }
    
    [Fact]
    [Trait("Category", "MacOS")]
    [Trait("Category", "Integration")]
    public async Task CoreAudioEngineWrapper_ShouldStartAndStop()
    {
        // Skip if not running on macOS
        if (!OperatingSystem.IsMacOS() && !OperatingSystem.IsMacCatalyst())
        {
            return;
        }

#if MACCATALYST || MACOS
        // Arrange
        using var engine = new CoreAudioEngineWrapper();
        
        // Act & Assert
        var started = await engine.StartAsync();
        
        if (started)
        {
            engine.IsRunning.ShouldBeTrue();
            
            engine.Stop();
            
            // Note: IsRunning might still be true immediately after Stop() due to async nature
            // so we don't assert it here
        }
        else
        {
            // If we can't start (no permission, no device), that's okay for tests
            engine.IsRunning.ShouldBeFalse();
        }
#endif
    }
    
    [Fact]
    [Trait("Category", "MacOS")]
    [Trait("Category", "Integration")]
    public async Task CoreAudioEngineWrapper_ShouldHandleDeviceSelection()
    {
        // Skip if not running on macOS
        if (!OperatingSystem.IsMacOS() && !OperatingSystem.IsMacCatalyst())
        {
            return;
        }

#if MACCATALYST || MACOS
        // Arrange
        using var engine = new CoreAudioEngineWrapper();
        
        // Act & Assert - Should not throw
        var result = await engine.SelectDeviceAsync("test-device-id");
        
        // Device selection currently returns true (uses default device)
        result.ShouldBeTrue();
#endif
    }
    
    [Fact]
    [Trait("Category", "MacOS")]
    [Trait("Category", "Integration")]
    public void CoreAudioEngineWrapper_ShouldHandleTapInstallation()
    {
        // Skip if not running on macOS
        if (!OperatingSystem.IsMacOS() && !OperatingSystem.IsMacCatalyst())
        {
            return;
        }

#if MACCATALYST || MACOS
        // Arrange
        using var engine = new CoreAudioEngineWrapper();
        var callbackInvoked = false;
        
        void AudioCallback(float[] buffer, uint frameCount)
        {
            callbackInvoked = true;
        }
        
        // Act & Assert - Should not throw
        engine.InstallTap(2048, AudioCallback);
        engine.RemoveTap();
        
        // We can't easily test if the callback is actually invoked without starting the engine
        // and having a real audio source, so we just verify no exceptions are thrown
#endif
    }
}
