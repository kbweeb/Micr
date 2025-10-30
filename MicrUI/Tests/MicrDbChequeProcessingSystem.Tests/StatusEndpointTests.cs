using System.Linq;
using System.Text;
using System.Text.Json;

namespace MicrDbChequeProcessingSystem.Tests;

public class StatusEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public StatusEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task StatusEndpoint_ReturnsSnapshotWithMetrics()
    {
        var response = await _client.GetAsync("/api/v1/status");
        var payload = await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode, payload);

        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(payload));
        using var json = await JsonDocument.ParseAsync(stream);

        Assert.True(json.RootElement.TryGetProperty("generatedAt", out var generatedAt));
        Assert.Equal(JsonValueKind.String, generatedAt.ValueKind);

        var metrics = json.RootElement.GetProperty("metrics");
        Assert.True(metrics.GetArrayLength() >= 4);

        var banksMetric = metrics.EnumerateArray().FirstOrDefault(m => m.GetProperty("label").GetString() == "Banks");
        Assert.Equal(1, banksMetric.GetProperty("value").GetDouble());

        var components = json.RootElement.GetProperty("components");
        Assert.NotEmpty(components.EnumerateArray());
    }
}
