using Customer.API.Models;
using Customer.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Customer.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customers;

    public CustomersController(ICustomerService customers)
    {
        _customers = customers;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<Models.Customer>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyList<Models.Customer>> GetAll() =>
        Ok(_customers.GetAll());

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Models.Customer), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Models.Customer> GetById(Guid id)
    {
        var customer = _customers.GetById(id);
        return customer is null ? NotFound() : Ok(customer);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Models.Customer), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Models.Customer> Create(CreateCustomerRequest request)
    {
        var customer = _customers.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Models.Customer), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Models.Customer> Update(Guid id, UpdateCustomerRequest request)
    {
        var customer = _customers.Update(id, request);
        return customer is null ? NotFound() : Ok(customer);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id) =>
        _customers.Delete(id) ? NoContent() : NotFound();
}
