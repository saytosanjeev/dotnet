using Customer.API.Models;
using Customer.API.Services;

namespace Customer.API.Tests;

public class CustomerServiceTests
{
    private readonly CustomerService _sut = new();

    [Fact]
    public void Create_AssignsIdAndTimestamps()
    {
        var customer = _sut.Create(new CreateCustomerRequest
        {
            Name = "  Ada Lovelace  ",
            Email = " ada@example.com ",
            Phone = " "
        });

        Assert.NotEqual(Guid.Empty, customer.Id);
        Assert.Equal("Ada Lovelace", customer.Name);
        Assert.Equal("ada@example.com", customer.Email);
        Assert.Null(customer.Phone);
        Assert.True(customer.CreatedAt <= DateTime.UtcNow);
        Assert.Null(customer.UpdatedAt);
    }

    [Fact]
    public void GetAll_ReturnsCreatedCustomers()
    {
        _sut.Create(new CreateCustomerRequest { Name = "One", Email = "one@example.com" });
        _sut.Create(new CreateCustomerRequest { Name = "Two", Email = "two@example.com" });

        var customers = _sut.GetAll();

        Assert.Equal(2, customers.Count);
    }

    [Fact]
    public void Update_WhenMissing_ReturnsNull()
    {
        var result = _sut.Update(Guid.NewGuid(), new UpdateCustomerRequest
        {
            Name = "Nope",
            Email = "nope@example.com"
        });

        Assert.Null(result);
    }

    [Fact]
    public void Delete_ReturnsFalseWhenMissing()
    {
        Assert.False(_sut.Delete(Guid.NewGuid()));
    }
}
