using System.Collections.Concurrent;
using Customer.API.Models;

namespace Customer.API.Services;

public class CustomerService : ICustomerService
{
    private readonly ConcurrentDictionary<Guid, Models.Customer> _customers = new();

    public IReadOnlyList<Models.Customer> GetAll() =>
        _customers.Values.OrderBy(c => c.CreatedAt).ToList();

    public Models.Customer? GetById(Guid id) =>
        _customers.TryGetValue(id, out var customer) ? customer : null;

    public Models.Customer Create(CreateCustomerRequest request)
    {
        var customer = new Models.Customer
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Email = request.Email.Trim(),
            Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _customers[customer.Id] = customer;
        return customer;
    }

    public Models.Customer? Update(Guid id, UpdateCustomerRequest request)
    {
        if (!_customers.TryGetValue(id, out var existing))
        {
            return null;
        }

        existing.Name = request.Name.Trim();
        existing.Email = request.Email.Trim();
        existing.Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim();
        existing.UpdatedAt = DateTime.UtcNow;
        return existing;
    }

    public bool Delete(Guid id) => _customers.TryRemove(id, out _);
}
