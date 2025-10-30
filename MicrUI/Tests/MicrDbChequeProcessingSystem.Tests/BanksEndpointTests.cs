using System.Linq;
using System.Text;
using System.Text.Json;

namespace MicrDbChequeProcessingSystem.Tests;

public class BanksEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public BanksEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task BanksEndpoint_ReturnsSeededBank()
    {
        var response = await _client.GetAsync("/api/v1/banks");

        var payload = await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode, payload);

        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(payload));
        using var json = await JsonDocument.ParseAsync(stream);

        var banks = json.RootElement;
        Assert.True(banks.GetArrayLength() >= 1);

        var bank = banks.EnumerateArray().First();
        Assert.Equal("Apex Test Bank", bank.GetProperty("name").GetString());
        Assert.Equal("123456", bank.GetProperty("sortCode").GetString());
        Assert.Equal("Greater Accra", bank.GetProperty("regionName").GetString());
    }

    [Fact]
    public async Task BankById_ReturnsNotFoundForMissingRecord()
    {
        var response = await _client.GetAsync("/api/v1/banks/9999");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}
