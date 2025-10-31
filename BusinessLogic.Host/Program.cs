using System.Reflection;

Console.Title = "BusinessLogic.Host";
Console.WriteLine("[BusinessLogic.Host] Starting...");

// Ensure the BusinessLogic assembly is loaded by name
var asm = Assembly.Load("BusinessLogic");
Console.WriteLine($"[BusinessLogic.Host] Loaded {asm.FullName}");

await Task.Delay(Timeout.Infinite);
