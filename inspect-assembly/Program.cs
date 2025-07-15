using System.Reflection;

var assemblyPath = "../src/AudioCompanion.Shared/bin/Debug/net9.0/AudioCompanion.Shared.dll";

if (File.Exists(assemblyPath))
{
    var assembly = Assembly.LoadFrom(assemblyPath);
    
    Console.WriteLine($"Assembly: {assembly.FullName}");
    Console.WriteLine("Types in assembly:");
    
    foreach (var type in assembly.GetTypes())
    {
        Console.WriteLine($"  - {type.FullName}");
    }
    
    var processor = assembly.GetTypes().FirstOrDefault(t => t.Name == "RealTimeAudioProcessor");
    if (processor != null)
    {
        Console.WriteLine($"\nFound RealTimeAudioProcessor: {processor.FullName}");
        Console.WriteLine($"Namespace: {processor.Namespace}");
        Console.WriteLine($"Is Public: {processor.IsPublic}");
    }
    else
    {
        Console.WriteLine("\nRealTimeAudioProcessor NOT found!");
    }
}
else
{
    Console.WriteLine($"Assembly not found at: {assemblyPath}");
}
