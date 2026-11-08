using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Mka.Api.Auth;

namespace Mka.Api.Tests;

public class EquipmentApiTests(MkaApiFactory factory) : IClassFixture<MkaApiFactory>
{
    private async Task<HttpClient> LoginAs(string username, string password)
    {
        var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/auth/login", new { username, password });
        response.EnsureSuccessStatusCode();

        var token = await response.Content.ReadFromJsonAsync<TokenResponse>();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token!.AccessToken);
        return client;
    }

    [Fact]
    public async Task Login_WithWrongPassword_Returns401()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login",
            new { username = "tech1", password = "wrong" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_WithoutToken_Returns401()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/equipment");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_AsTechnician_ReturnsSeededEquipment()
    {
        var client = await LoginAs("tech1", "Tech#1234");

        var response = await client.GetAsync("/api/equipment");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("P-101", body);
    }

    [Fact]
    public async Task GetById_WhenMissing_Returns404()
    {
        var client = await LoginAs("tech1", "Tech#1234");

        var response = await client.GetAsync("/api/equipment/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_AsTechnician_Returns403()
    {
        var client = await LoginAs("tech1", "Tech#1234");

        var response = await client.PostAsJsonAsync("/api/equipment", new
        {
            tag = "V-901", name = "Test Vessel", type = "Vessel",
            location = "CPF", status = "Operational", installedOn = "2024-01-01"
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Create_AsSupervisor_Returns201WithLocation()
    {
        var client = await LoginAs("super1", "Super#1234");

        var response = await client.PostAsJsonAsync("/api/equipment", new
        {
            tag = "V-902", name = "Test Separator", type = "Vessel",
            location = "CPF", status = "Operational", installedOn = "2024-01-01"
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        // The Location header must point to an endpoint that returns the new item.
        var getResponse = await client.GetAsync(response.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.Contains("V-902", await getResponse.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Create_WithInvalidTag_Returns400()
    {
        var client = await LoginAs("super1", "Super#1234");

        var response = await client.PostAsJsonAsync("/api/equipment", new
        {
            tag = "pump1", name = "Bad Tag Pump", type = "Pump",
            location = "CPF", status = "Operational", installedOn = "2024-01-01"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Tag", await response.Content.ReadAsStringAsync());
    }
}
