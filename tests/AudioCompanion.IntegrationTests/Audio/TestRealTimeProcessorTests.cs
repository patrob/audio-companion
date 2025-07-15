using AudioCompanion.Shared.Audio;
using NSubstitute;
using Shouldly;

namespace AudioCompanion.IntegrationTests.Audio;

public class TestRealTimeProcessorTests
{
    [Fact]
    public void TestProcessor_StartProcessing_ShouldConfigureAudioEngine()
    {
        // Arrange
        var mockAudioEngine = Substitute.For<ICoreAudioEngine>();
        mockAudioEngine.StartAsync().Returns(true);
        
        var processor = new TestRealTimeProcessor(mockAudioEngine);
        
        // Act
        processor.StartProcessing();
        
        // Assert
        mockAudioEngine.Received(1).InstallTap(Arg.Any<uint>(), Arg.Any<Action<float[], uint>>());
        mockAudioEngine.Received(1).StartAsync();
        
        // Cleanup
        processor.Dispose();
    }
}
