using MicrDbChequeProcessingSystem.Models;

Console.Title = "Domain.Host";
Console.WriteLine("[Domain.Host] Starting...");

// Touch a type to ensure the Domain assembly is loaded
_ = typeof(AccountType);

Console.WriteLine("[Domain.Host] Domain models loaded. Running.");
await Task.Delay(Timeout.Infinite);
