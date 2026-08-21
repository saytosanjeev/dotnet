using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Customer.API.Models;
using CustomerModel = Customer.API.Models.Customer;

namespace Customer.API.Tests;

public class CustomersApiTests : IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly CustomerWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public CustomersApiTests()
    {
        _factory = new CustomerWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_WhenEmpty_ReturnsEmptyList()
    {
        var response = await _client.GetAsync("/api/customers");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var customers = await response.Content.ReadFromJsonAsync<List<CustomerModel>>(JsonOptions);
        Assert.NotNull(customers);
        Assert.Empty(customers);
    }

    [Fact]
    public async Task Create_ReturnsCreatedCustomer()
    {
        var response = await _client.PostAsJsonAsync("/api/customers", new CreateCustomerRequest
        {
            Name = "Ada Lovelace",
            Email = "ada@example.com",
            Phone = "+1-555-0100"
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var customer = await response.Content.ReadFromJsonAsync<CustomerModel>(JsonOptions);
        Assert.NotNull(customer);
        Assert.NotEqual(Guid.Empty, customer.Id);
        Assert.Equal("Ada Lovelace", customer.Name);
        Assert.Equal("ada@example.com", customer.Email);
        Assert.Equal("+1-555-0100", customer.Phone);
        Assert.EndsWith($"/api/Customers/{customer.Id}", response.Headers.Location?.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Create_WithInvalidEmail_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/customers", new
        {
            name = "Ada Lovelace",
            email = "not-an-email"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenMissing_ReturnsNotFound()
    {
        var response = await _client.GetAsync($"/api/customers/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetById_AfterCreate_ReturnsCustomer()
    {
        var created = await CreateCustomerAsync("Grace Hopper", "grace@example.com");

        var response = await _client.GetAsync($"/api/customers/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var customer = await response.Content.ReadFromJsonAsync<CustomerModel>(JsonOptions);
        Assert.NotNull(customer);
        Assert.Equal(created.Id, customer.Id);
        Assert.Equal("Grace Hopper", customer.Name);
    }

    [Fact]
    public async Task Update_ChangesCustomerFields()
    {
        var created = await CreateCustomerAsync("Alan Turing", "alan@example.com");

        var response = await _client.PutAsJsonAsync($"/api/customers/{created.Id}", new UpdateCustomerRequest
        {
            Name = "Alan M. Turing",
            Email = "turing@example.com",
            Phone = "+1-555-0199"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<CustomerModel>(JsonOptions);
        Assert.NotNull(updated);
        Assert.Equal("Alan M. Turing", updated.Name);
        Assert.Equal("turing@example.com", updated.Email);
        Assert.Equal("+1-555-0199", updated.Phone);
        Assert.NotNull(updated.UpdatedAt);
    }

    [Fact]
    public async Task Update_WhenMissing_ReturnsNotFound()
    {
        var response = await _client.PutAsJsonAsync($"/api/customers/{Guid.NewGuid()}", new UpdateCustomerRequest
        {
            Name = "Missing Person",
            Email = "missing@example.com"
        });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_RemovesCustomer()
    {
        var created = await CreateCustomerAsync("Katherine Johnson", "katherine@example.com");

        var deleteResponse = await _client.DeleteAsync($"/api/customers/{created.Id}");
        var getResponse = await _client.GetAsync($"/api/customers/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenMissing_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync($"/api/customers/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<CustomerModel> CreateCustomerAsync(string name, string email)
    {
        var response = await _client.PostAsJsonAsync("/api/customers", new CreateCustomerRequest
        {
            Name = name,
            Email = email
        });
        response.EnsureSuccessStatusCode();
        var customer = await response.Content.ReadFromJsonAsync<CustomerModel>(JsonOptions);
        Assert.NotNull(customer);
        return customer;
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
