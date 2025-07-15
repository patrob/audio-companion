using AudioCompanion.Shared.Audio;

Console.WriteLine("Testing RealTimeAudioProcessor...");

// Try to create an instance
var processor = new RealTimeAudioProcessor();
Console.WriteLine($"Created processor: {processor.GetType().Name}");

processor.Dispose();
Console.WriteLine("Test completed successfully!");
