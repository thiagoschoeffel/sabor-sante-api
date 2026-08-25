using System.Net;
using System.Net.Http.Json;

namespace SaborSante.Api.Tests;

public class HealthEndpointTests
{
    private record HealthResponse(string Status);

    [Fact]
    public async Task GetHealth_deve_retornar_ok()
    {
        await using var factory =
            new ApiWebApplicationFactory();

        var client = factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode
        );

        var body =
            await response.Content.ReadFromJsonAsync<HealthResponse>();

        Assert.NotNull(body);
        Assert.Equal("ok", body.Status);
    }
}