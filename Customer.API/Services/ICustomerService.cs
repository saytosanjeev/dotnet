using Customer.API.Models;

namespace Customer.API.Services;

public interface ICustomerService
{
    IReadOnlyList<Models.Customer> GetAll();
    Models.Customer? GetById(Guid id);
    Models.Customer Create(CreateCustomerRequest request);
    Models.Customer? Update(Guid id, UpdateCustomerRequest request);
    bool Delete(Guid id);
}
