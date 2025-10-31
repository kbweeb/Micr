using MicrDbChequeProcessingSystem.Data;

Console.Title = "DataAccess.Host";
Console.WriteLine("[DataAccess.Host] Starting...");

// Touch type so assembly is loaded
_ = typeof(MicrDbContext);

Console.WriteLine("[DataAccess.Host] DataAccess types loaded. Running.");
await Task.Delay(Timeout.Infinite);
