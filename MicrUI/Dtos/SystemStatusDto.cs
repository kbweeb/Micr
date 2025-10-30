namespace MicrDbChequeProcessingSystem.Dtos;

public record SystemStatusDto(
    DateTime GeneratedAt,
    IReadOnlyCollection<SystemStatusMetricDto> Metrics,
    IReadOnlyCollection<SystemStatusComponentDto> Components,
    IReadOnlyCollection<SystemIncidentDto> Incidents
);

public record SystemStatusMetricDto(string Label, double Value);

public record SystemStatusComponentDto(string Name, string Status, string Summary, DateTime LastUpdatedUtc);

public record SystemIncidentDto(DateTime LoggedAtUtc, string Severity, string Title, string Description);
