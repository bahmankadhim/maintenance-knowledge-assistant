using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Mka.Api.Auth;

namespace Mka.Api.Tests;

public class WorkOrderApiTests(MkaApiFactory factory) : IClassFixture<MkaApiFactory>
{
    private async Task<HttpClient> LoginAsTechnician()
    {
        var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/auth/login",
            new { username = "tech1", password = "Tech#1234" });
        response.EnsureSuccessStatusCode();

        var token = await response.Content.ReadFromJsonAsync<TokenResponse>();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token!.AccessToken);
        return client;
    }

    [Fact]
    public async Task GetAll_WithoutToken_Returns401()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/workorder");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Create_ForExistingEquipment_Returns201AsOpen()
    {
        var client = await LoginAsTechnician();

        var response = await client.PostAsJsonAsync("/api/workorder", new
        {
            title = "Replace mechanical seal",
            description = "Seal leaking on drive end",
            equipmentId = 1,
            priority = "High",
            assignedTo = "tech1"
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"status\":\"Open\"", body);
    }

    [Fact]
    public async Task Create_ForMissingEquipment_Returns400()
    {
        var client = await LoginAsTechnician();

        var response = await client.PostAsJsonAsync("/api/workorder", new
        {
            title = "Inspect unknown pump",
            description = "Equipment id does not exist",
            equipmentId = 9999,
            priority = "Low"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithTooShortTitle_Returns400()
    {
        var client = await LoginAsTechnician();

        var response = await client.PostAsJsonAsync("/api/workorder", new
        {
            title = "Fix",
            description = "Pump is noisy",
            equipmentId = 1,
            priority = "Low"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_FilteredByEquipment_ReturnsOnlyThatEquipment()
    {
        var client = await LoginAsTechnician();
        await client.PostAsJsonAsync("/api/workorder", new
        {
            title = "Compressor vibration check", description = "High vibration on bearing", equipmentId = 2, priority = "Medium"
        });

        var response = await client.GetAsync("/api/workorder?equipmentId=2");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var items = await response.Content.ReadFromJsonAsync<List<WorkOrderItem>>();
        Assert.NotEmpty(items!);
        Assert.All(items!, w => Assert.Equal(2, w.EquipmentId));
    }

    // Only the field this test needs; extra JSON fields are ignored.
    private record WorkOrderItem(int EquipmentId);
}
